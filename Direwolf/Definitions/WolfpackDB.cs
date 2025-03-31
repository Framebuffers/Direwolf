using Direwolf.Contracts;
using Npgsql;
using NpgsqlTypes;
using System.Diagnostics;
using System.Text.Json;

namespace Direwolf.Definitions
{
    public readonly record struct DbConnectionString(string Host, int Port, string Username, string Password, string Database);
    public class WolfpackDB : Stack<Wolfpack>, IWolfpackDB
    {

        private readonly string Desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public event EventHandler DatabaseConnectedEventHandler;
        private readonly DbConnectionString _str;

        public WolfpackDB(DbConnectionString db)
        {
            _str = db;
            DatabaseConnectedEventHandler += WolfpackDB_DatabaseConnectedEventHandler;
        }

        private void WolfpackDB_DatabaseConnectedEventHandler(object? sender, EventArgs e) => Debug.Print("Database Connected");

        public virtual async Task Send()
        {
            DatabaseConnectedEventHandler?.Invoke(this, new EventArgs());
            string sqlQuery =
                """INSERT INTO "Wolfpack" ("documentName", "fileOrigin", "wasCompleted", "timeTaken", "createdAt", "guid", "resultCount", "testName", "results", "documentVersion") VALUES (@docName, @origin, @completed, @time, @creation, @id, @resCount, @name, @result, @docVersion)""";

            try
            {
                using NpgsqlConnection c = new($"Host={_str.Host};Port={_str.Port};Username={_str.Username};Password={_str.Password};Database={_str.Database}");
                DatabaseConnectedEventHandler?.Invoke(this, new EventArgs());
                c.StateChange += C_StateChange;
                c.Notice += C_Notice;
                while (Count > 0)
                {
                    string fileName = Path.Combine(Desktop, $"Queries.json");
                    Wolfpack wolfpack = Pop();
                    File.WriteAllText(fileName, wolfpack.Results);

                    Console.WriteLine(c.ConnectionString);
                    c.Open();
                    await using var cmd = new NpgsqlCommand(sqlQuery, c);
                    {
                        var resultJson = cmd.CreateParameter();
                        resultJson.ParameterName = "result";
                        resultJson.NpgsqlDbType = NpgsqlDbType.Json;
                        resultJson.Value = wolfpack.Results;
                        cmd.Parameters.AddWithValue("docName", wolfpack.DocumentName);
                        cmd.Parameters.AddWithValue("origin", wolfpack.FileOrigin);
                        cmd.Parameters.AddWithValue("docVersion", wolfpack.DocumentVersion);
                        cmd.Parameters.AddWithValue("completed", wolfpack.WasCompleted);
                        cmd.Parameters.AddWithValue("time", wolfpack.TimeTaken);
                        cmd.Parameters.AddWithValue("creation", wolfpack.CreatedAt);
                        cmd.Parameters.AddWithValue("id", wolfpack.GUID);
                        cmd.Parameters.AddWithValue("resCount", wolfpack.ResultCount);
                        cmd.Parameters.AddWithValue("name", wolfpack.TestName);
                        cmd.Parameters.Add(resultJson);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        Debug.Print($"Executed query. Added {rowsAffected} rows");
                    }
                    c.Dispose();
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        private void C_Notice(object sender, NpgsqlNoticeEventArgs e) => Debug.Print($"Postgres Notice: {e.Notice.Severity}; {e.Notice.MessageText}");
        private void C_StateChange(object sender, System.Data.StateChangeEventArgs e) => Debug.Print($"Connection status: {e.CurrentState}");
    }
}
