using Microsoft.EntityFrameworkCore.Update;
using Npgsql;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Direwolf.Definitions
{
    public readonly record struct DbConnectionString(string Host, string Username, string Password, string Database) { }

    public class WolfpackDB : Stack<Wolfpack>
    {
        public event EventHandler DatabaseConnectedEventHandler;
        private readonly DbConnectionString _str;
        private const string TABLE_NAME = "Wolfpack";

        public WolfpackDB(DbConnectionString db)
        {
            _str = db;
            DatabaseConnectedEventHandler += WolfpackDB_DatabaseConnectedEventHandler;


        }

        private void WolfpackDB_DatabaseConnectedEventHandler(object? sender, EventArgs e)
        {
            Debug.Print("Database Connected");
        }

        public async Task Send()
        {
            Wolfpack wolfpack = Pop();
            Debug.Print($"Pushing to DB: {wolfpack.GUID}");
            DatabaseConnectedEventHandler?.Invoke(this, new EventArgs());

            string sqlQuery =
                $"INSERT INTO {TABLE_NAME} (documentName, fileOrigin, documentVersion, wasCompleted, timeTaken, createdAt, guid, resultCount, testName, results, testId) VALUES (@docName, @origin, @version, @completed, @time, @creation, @id, @resCount, @name, @result, @testNumber)";

            try
            {
                using NpgsqlConnection c = new(
                $"Host={_str.Host};" +
                $"Username={_str.Username};" +
                $"Password={_str.Password};" +
                $"Database={_str.Database}");
                DatabaseConnectedEventHandler?.Invoke(this, new EventArgs());
                c.StateChange += C_StateChange;
                c.Notice += C_Notice;
                while (Count > 0)
                {
                    await c.OpenAsync();
                    await using var cmd = new NpgsqlCommand(sqlQuery, c);
                    {
                        cmd.Parameters.AddWithValue("docName", wolfpack.DocumentName);
                        cmd.Parameters.AddWithValue("origin", wolfpack.FileOrigin);
                        cmd.Parameters.AddWithValue("version", wolfpack.DocumentVersion);
                        cmd.Parameters.AddWithValue("completed", wolfpack.WasCompleted);
                        cmd.Parameters.AddWithValue("time", wolfpack.TimeTaken);
                        cmd.Parameters.AddWithValue("creation", wolfpack.CreatedAt);
                        cmd.Parameters.AddWithValue("id", wolfpack.GUID);
                        cmd.Parameters.AddWithValue("resCount", wolfpack.ResultCount);
                        cmd.Parameters.AddWithValue("name", wolfpack.Howler.GetType().ToString());
                        cmd.Parameters.AddWithValue("result", wolfpack.Results);
                        cmd.Parameters.AddWithValue("testNumber", RandomNumberGenerator.GetInt32(1000000));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        Debug.Print($"Executed query. Added {rowsAffected} rows");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        private void C_Notice(object sender, NpgsqlNoticeEventArgs e)
        {
            Debug.Print($"Postgres Notice: {e.Notice.Severity}; {e.Notice.MessageText}");
        }

        private void C_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            Debug.Print($"Connection status: {e.CurrentState}");
        }
    }
}
