using Godot;
using MemoryPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;

namespace Microgravity.util
{
	/// <summary>
	/// Struct to store high score information.
	/// </summary>
	public struct HighScore
	{
		/// <value><c>Id</c>: Unique identifier for this high score (primary key in SQLite)</value>
		public int Id;
		public string PlayerName;
		public int Score;
	}

	/// <summary>
	/// Class for accessing all database features.
	/// All members are static so there's no need to worry about keeping an object around.
	/// They are just grouped in DB, like with Godot's GD class.
	/// </summary>
	public partial class DB
	{
		private static readonly string _dbPath = ProjectSettings.GlobalizePath("user://game_db.db");
		private static readonly string _connectionString = $"Data Source={_dbPath};Version=3;";

		private static readonly string _highScoresLayout = "CREATE TABLE IF NOT EXISTS high_scores (id INTEGER PRIMARY KEY, player_name TEXT, score INTEGER)";
		private static readonly string _settingsLayout = "CREATE TABLE IF NOT EXISTS settings (id INTEGER PRIMARY KEY, key TEXT, value TEXT, UNIQUE(key))";
		private static readonly string _savesLayout = "CREATE TABLE IF NOT EXISTS saves (id INTEGER PRIMARY KEY, name TEXT, data BLOB)";

		/// <summary>
		/// Open a new connection to SQLite.
		/// For the sake of simplicity, a new connection is established every time a public member of <c>DB</c> is called,
		/// which is fine for the purpose of storing only a few values that aren't frequently accessed.
		///
		/// After usage, Close() should be called on the <c>SQLiteConnection</c> object.
		/// </summary>
		/// <returns>Object of <c>SQLiteConnection</c></returns>
		private static SQLiteConnection Connect()
		{
			SQLiteConnection connection = new(_connectionString);

			try
			{
				connection.Open();
				return connection;
			}
			catch (Exception ex)
			{
				GD.Print($"DB: Could not connect to database: {ex.Message}");
				return null;
			}
		}

		/// <summary>
		/// Add a new high score for a given player.
		/// </summary>
		/// <param name="playerName">Name of the player</param>
		/// <param name="score">Score integer</param>
		public static bool AddHighScore(string playerName, int score)
		{
			playerName = playerName?.Trim();
			if (string.IsNullOrEmpty(playerName))
			{
				return false;
			}

			SQLiteConnection connection = Connect();
			if (connection == null)
			{
				return false;
			}

			bool tableCreated = CreateTable(connection, _highScoresLayout);
			if (!tableCreated)
			{
				return false;
			}

			try
			{
				string insertSql = "INSERT INTO high_scores (player_name, score) VALUES (@player_name, @score)";
				SQLiteCommand insertCommand = new(insertSql, connection);
				insertCommand.Parameters.AddWithValue("@player_name", playerName);
				insertCommand.Parameters.AddWithValue("@score", Math.Max(0, score));
				insertCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				GD.Print($"DB: Failed to write to table: {ex.Message}");
				connection.Close();
				return false;
			}

			connection.Close();
			return true;
		}

		/// <summary>
		/// Get a list of <c>HighScore</c> objects for all saved high scores in the database.
		/// </summary>
		/// <param name="playerName">(Optional) Return only this player's high scores.</param>
		/// <param name="count">(Optional) Maximum amount of high scores to return. 0 for no limit.</param>
		/// <returns>List of <c>HighScore</c> sorted from highest to lowest</returns>
		public static List<HighScore> GetHighScores(string playerName = null, int count = 0)
		{
			SQLiteConnection connection = Connect();
			if (connection == null)
			{
				return [];
			}

			if (!CreateTable(connection, _highScoresLayout))
			{
				return [];
			}

			string selectSql = "SELECT * FROM high_scores"
								+ (playerName != null ? " WHERE player_name IS @player_name" : "")
								+ " ORDER BY score DESC"
								+ (count > 0 ? " LIMIT @count" : "");

			SQLiteCommand selectCommand = new(selectSql, connection);
			if (playerName != null)
			{
				selectCommand.Parameters.AddWithValue("@player_name", playerName);
			}

			if (count > 0)
			{
				selectCommand.Parameters.AddWithValue("@count", count);
			}

			List<HighScore> ret = [];
			try
			{
				var reader = selectCommand.ExecuteReader();

				while (reader.Read())
				{
					ret.Add(new HighScore { Id = reader.GetInt32(0), PlayerName = reader.GetString(1), Score = reader.GetInt32(2) });
				}

				reader.Close();
			}
			catch (Exception ex)
			{
				GD.Print($"DB: Failed to query database: {ex.Message}");
			}

			connection.Close();
			return ret;
		}

		/// <summary>
		/// Load all settings, merging the defaults with database entries.
		/// If ran for the first time, create the settings table.
		/// </summary>
		/// <returns> Dictionary of settings entries addressable by key </returns>
		public static Dictionary<string, SettingsEntry> GetSettings()
		{
			SQLiteConnection connection = Connect();
			if (connection == null)
			{
				return [];
			}

			// if the table does not exist yet, create it
			if (!CreateTable(connection, _settingsLayout))
			{
				return [];
			}

			// initialise return value to defaults
			Dictionary<string, SettingsEntry> ret = new(SettingsEntry.DefaultSettings);

			string selectSql = "SELECT * FROM settings";
			var reader = new SQLiteCommand(selectSql, connection).ExecuteReader();

			while (reader.Read())
			{
				string entryKey = reader.GetString(1);
				// ignore additional rows not declared in DefaultSettings
				if (!ret.ContainsKey(entryKey))
				{
					continue;
				}

				try
				{
					// convert entries in the DB into their respective type in the settings Dictionary and update them there		
					ret[entryKey] = ret[entryKey] switch
					{
						SettingsEntry.Keybind k => k with { Value = reader.GetString(2) },
						SettingsEntry.Float f => f with { Value = float.Parse(reader.GetString(2), CultureInfo.InvariantCulture) },
						SettingsEntry.Bool b => b with { Value = bool.Parse(reader.GetString(2)) },
						_ => throw new UnreachableException()
					};
				}
				catch (FormatException ex)
				{
					GD.Print($"DB: Bad settings format, using the default value for {entryKey}: {ex.Message}");
				}
			}

			reader.Close();
			connection.Close();

			return ret;
		}

		/// <summary>
		/// Update a single settings entry with the given value or insert it if it wasn't there before.
		/// </summary>
		public static void UpdateSettingsEntry(string key, SettingsEntry value)
		{
			SQLiteConnection connection = Connect();
			if (connection == null)
			{
				return;
			}

			// make sure the table exists, though it should have been created already when this function is called
			if (!CreateTable(connection, _settingsLayout))
			{
				return;
			}

			try
			{
				string updateSql = "INSERT INTO settings (key, value) VALUES (@key, @value) ON CONFLICT(key) DO UPDATE SET value = EXCLUDED.value";

				SQLiteCommand updateCommand = new(updateSql, connection);
				updateCommand.Parameters.AddWithValue("@key", key);
				updateCommand.Parameters.AddWithValue("@value", value.ToString());

				updateCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				GD.Print($"DB: Failed to write settings entry {key}: {ex.Message}");
			}

			connection.Close();
		}

		/// <summary>
		/// Save a Stats object to disk by serialising it and putting that inside the DB as a binary blob.
		/// </summary>
		public static void SaveGame(string name, Stats playerStats)
		{
			SQLiteConnection connection = Connect();
			if (connection == null)
			{
				return;
			}

			if (!CreateTable(connection, _savesLayout))
			{
				return;
			}

			try
			{
				string insertSql = "INSERT INTO saves (name, data) VALUES (@name, @data)";

				SQLiteCommand insertCommand = new(insertSql, connection);
				insertCommand.Parameters.AddWithValue("@name", name);
				insertCommand.Parameters.Add("@data", DbType.Binary).Value = MemoryPackSerializer.Serialize(playerStats);

				insertCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				GD.Print($"DB: Failed to save game {name}: {ex.Message}");
			}

			connection.Close();
		}

		public static void DeleteGame(int id)
		{
			SQLiteConnection connection = Connect();
			if (connection == null)
			{
				return;
			}

			if (!CreateTable(connection, _savesLayout))
			{
				return;
			}

			try
			{
				string deleteSql = "DELETE FROM saves WHERE id = @id";

				SQLiteCommand deleteCommand = new(deleteSql, connection);
				deleteCommand.Parameters.AddWithValue("@id", id);

				deleteCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				GD.Print($"DB: Failed to delete game {id}: {ex.Message}");
			}

			connection.Close();
		}

		/// <summary>
		/// Enumerate all saves in the database.
		/// You can then use this information to load a game by its identifier using <c>LoadGame(int)</c>.
		/// </summary>
		/// <returns>Dictionary with the key being the save ID in the saves table and the value being its name</returns>
		public static Dictionary<int, string> GetSaves()
		{
			Dictionary<int, string> ret = [];
			SQLiteConnection connection = Connect();
			if (connection == null)
			{
				return ret;
			}

			if (!CreateTable(connection, _savesLayout))
			{
				return ret;
			}

			try
			{
				string selectSql = "SELECT id, name FROM saves ORDER BY id DESC";
				SQLiteCommand selectCommand = new(selectSql, connection);

				var reader = selectCommand.ExecuteReader();
				while (reader.Read())
				{
					ret.Add(reader.GetInt32(0), reader.GetString(1));
				}

				reader.Close();
			}
			catch (Exception ex)
			{
				GD.Print($"DB: Failed to query saves table: {ex.Message}");
			}
			
			connection.Close();
			return ret;
		}

		/// <summary>
		/// Load the game from the row with the specified ID.
		/// </summary>
		/// <returns>Stats object to be used as a member of PlayerVariables</returns>
		public static Stats LoadGame(int id)
		{
			Stats ret = new();
			
			SQLiteConnection connection = Connect();
			if (connection == null)
			{
				return ret;
			}

			if (!CreateTable(connection, _savesLayout))
			{
				return ret;
			}

			try
			{
				string selectSql = "SELECT id, data FROM saves WHERE id IS @id";

				SQLiteCommand selectCommand = new(selectSql, connection);
				selectCommand.Parameters.AddWithValue("@id", id);
				
				var reader = selectCommand.ExecuteReader(CommandBehavior.KeyInfo);

				// we are guaranteed to either have 1 or 0 results since we filter by id
				reader.Read();

				if (!reader.HasRows)
				{
					GD.Print($"DB: Attempting to load nonexistent game save #{id}");

					reader.Close();
					return ret;	
				}
				
				var blob = reader.GetBlob(1, false);
				byte[] buf = new byte[blob.GetCount()];
				blob.Read(buf, blob.GetCount(), 0);
				blob.Close();
				
				ret = MemoryPackSerializer.Deserialize<Stats>(buf);
				GD.Print(ret);

				reader.Close();
			}
			catch (Exception ex)
			{
				GD.Print($"DB: Failed to load save #{id}: {ex.Message}");
			}

			connection.Close();
			return ret;
		}

		/// <summary>
		/// Create a table using the given CREATE TABLE command.
		/// </summary>
		/// <param name="createSql">String containing the CREATE TABLE SQL command to execute</param>
		/// <returns>Success value to check for</returns>
		private static bool CreateTable(SQLiteConnection connection, string createSql)
		{
			try
			{
				SQLiteCommand createTableCommand = new(createSql, connection);
				createTableCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				GD.Print($"DB: Failed to create table: {ex.Message}");
				connection.Close();
				return false;
			}

			return true;
		}
	}
}
