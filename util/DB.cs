using Godot;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace SpaceGame.util
{
	/// <summary>
	/// Struct to store high score information.
	/// </summary
	public struct HighScore
	{
		/// <value><c>Id</c>: Unique identifier for this high score (primary key in SQLite)</value>
		public int Id;
		public string PlayerName;
		public int Score;
	}

	public partial class DB
	{
		private static readonly string DBPath = ProjectSettings.GlobalizePath("user://game_db.db");
		private static readonly string ConnectionString = $"Data Source={DBPath};Version=3;";

		/// <summary>
		/// Open a new connection to SQLite.
		/// For the sake of simplicity, a new connection is established every time a member of <c>DB</c> is called,
		/// which is fine for the purpose of storing only a few values that aren't frequently accessed.
		///
		/// After usage, Close() should be called on the <c>SQLiteConnection</c> object.
		/// </summary>
		/// <returns>Object of <c>SQLiteConnection</c></returns>
		private static SQLiteConnection Connect()
		{
			SQLiteConnection connection = new(ConnectionString);

			try
			{
				connection.Open();
				return connection;
			}
			catch (Exception ex)
			{
				GD.Print($"DB: Error: {ex.Message}");
				return null;
			}
		}

		/// <summary>
		/// Add a new high score for a given player.
		/// </summary
		/// <param name="PlayerName">Name of the player</param>
		/// <param name="Score">Score integer</param>
		public static void AddHighScore(string PlayerName, int Score)
		{
			SQLiteConnection connection = Connect();
			if (connection == null)
			{
				return;
			}

			bool tableCreated = CreateHighScoreTable(connection);
			if (!tableCreated)
			{
				return;
			}

			try
			{
				string insertSql = "INSERT INTO high_scores (player_name, score) VALUES (@player_name, @score)";
				SQLiteCommand insertCommand = new(insertSql, connection);
				insertCommand.Parameters.AddWithValue("@player_name", PlayerName);
				insertCommand.Parameters.AddWithValue("@score", Score);
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
		/// <param name="PlayerName">(Optional) Return only this player's high scores.</param>
		/// <returns>List of <c>HighScore</c> sorted from highest to lowest</returns>
		public static List<HighScore> GetHighScores(string PlayerName = null)
		{
			SQLiteConnection connection = Connect();
			if (connection == null)
			{
				return [];
			}

			bool tableCreated = CreateHighScoreTable(connection);
			if (!tableCreated)
			{
				return [];
			}

			string selectSql = "SELECT * FROM high_scores"
								+ (PlayerName != null ? " WHERE player_name IS @player_name" : "")
								+ " ORDER BY score DESC";

			SQLiteCommand selectCommand = new(selectSql, connection);
			if (PlayerName != null)
			{
				selectCommand.Parameters.AddWithValue("@player_name", PlayerName);
			}

			List<HighScore> ret = [];
			try
			{
				var reader = selectCommand.ExecuteReader();

				while (reader.Read())
				{
					ret.Add(new HighScore { Id = reader.GetInt32(0), PlayerName = reader.GetString(1), Score = reader.GetInt32(2) });
				}
			}
			catch (Exception ex)
			{
				GD.Print($"DB: Failed to query database: {ex.Message}");
			}

			connection.Close();
			return ret;
		}

		/// <summary>
		/// Create the high_scores table if it does not exist yet.
		/// </summary
		/// <returns>Success value to check for</returns>
		private static bool CreateHighScoreTable(SQLiteConnection connection)
		{
			try
			{
				string createTableSql = "CREATE TABLE IF NOT EXISTS high_scores (id INTEGER PRIMARY KEY, player_name TEXT, score INTEGER)";
				SQLiteCommand createTableCommand = new(createTableSql, connection);
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
