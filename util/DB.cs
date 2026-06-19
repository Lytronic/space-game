using Godot;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;

namespace SpaceGame.util
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
	/// Wrapper for storing settings entries of varying types along with their descriptions in the same Dictionary.
	/// </summary>
	public abstract record SettingsEntry
	{
		public sealed record Keybind(string Value, string Description) : SettingsEntry
		{
			public override string ToString() => Value;
		}
		public sealed record Float(float Value, float Min, float Max, string Description) : SettingsEntry
		{
			public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
		}
		public sealed record Bool(bool Value, string Description) : SettingsEntry
		{
			public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
		}

		/// <value>
		/// These is the exhaustive list of all settings options with their names and defaults.
		/// Add new entries here!
		/// </value>
		public static readonly Dictionary<string, SettingsEntry> DefaultSettings = new()
		{
			["controls.forward"] = new Keybind("W", "Move forward"),
			["controls.backward"] = new Keybind("S", "Move backward"),
			["controls.left"] = new Keybind("A", "Strafe left"),
			["controls.right"] = new Keybind("D", "Strafe right"),
			["controls.test_value"] = new Bool(false, "Test Value"),
			["controls.test_slider"] = new Float(1.0f, -5.0f, 10.0f, "Example Slider")
		};
	}

	public partial class DB
	{
		private static readonly string _dbPath = ProjectSettings.GlobalizePath("user://game_db.db");
		private static readonly string _connectionString = $"Data Source={_dbPath};Version=3;";

		private static readonly string _highScoresLayout = "CREATE TABLE IF NOT EXISTS high_scores (id INTEGER PRIMARY KEY, player_name TEXT, score INTEGER)";
		private static readonly string _settingsLayout = "CREATE TABLE IF NOT EXISTS settings (id INTEGER PRIMARY KEY, key TEXT, value TEXT, UNIQUE(key))";

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
		public static void AddHighScore(string playerName, int score)
		{
			SQLiteConnection connection = Connect();
			if (connection == null)
			{
				return;
			}

			bool tableCreated = CreateTable(connection, _highScoresLayout);
			if (!tableCreated)
			{
				return;
			}

			try
			{
				string insertSql = "INSERT INTO high_scores (player_name, score) VALUES (@player_name, @score)";
				SQLiteCommand insertCommand = new(insertSql, connection);
				insertCommand.Parameters.AddWithValue("@player_name", playerName);
				insertCommand.Parameters.AddWithValue("@score", score);
				insertCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				GD.Print($"DB: Failed to write to table: {ex.Message}");
			}

			connection.Close();
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
				catch (FormatException e)
				{
					GD.Print($"DB: Bad settings format, using the default value for {entryKey}: {e.Message}");
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
			catch (Exception e)
			{
				GD.Print($"DB: Failed to write settings entry {key}: {e.Message}");
			}
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
