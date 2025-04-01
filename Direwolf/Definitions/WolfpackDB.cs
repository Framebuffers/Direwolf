using Npgsql;
using NpgsqlTypes;
using System.Diagnostics;

namespace Direwolf.Definitions
{
    public readonly record struct DbConnectionString(string Host, int Port, string Username, string Password, string Database) { }

    public class WolfpackDB : Stack<Wolfpack>
    {
        public event EventHandler DatabaseConnectedEventHandler;
        private readonly DbConnectionString _str;

        public WolfpackDB(DbConnectionString db)
        {
            _str = db;
            DatabaseConnectedEventHandler += WolfpackDB_DatabaseConnectedEventHandler;
        }

        private void WolfpackDB_DatabaseConnectedEventHandler(object? sender, EventArgs e)
        {
            Debug.Print("Database Connected");
        }

        private readonly string Desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);



        public async Task Send()
        {
            DatabaseConnectedEventHandler?.Invoke(this, new EventArgs());
            string sqlQuery =
                """INSERT INTO "Wolfpack" ("documentName", "fileOrigin", "documentVersion", "wasCompleted", "timeTaken", "createdAt", "guid", "resultCount", "testName", "results") VALUES (@docName, @origin, @version, @completed, @time, @creation, @id, @resCount, @name, @result)""";
            try
            {
                using NpgsqlConnection c = new($"Host={_str.Host};Port={_str.Port};Username={_str.Username};Password={_str.Password};Database={_str.Database}");

                if (c is not null) Debug.Print("connection is not null");
                
                DatabaseConnectedEventHandler?.Invoke(this, new EventArgs());
                c.StateChange += C_StateChange;
                c.Notice += C_Notice;
                
                while (Count > 0)
                {
                    Wolfpack wolfpack = Pop();
                    string fileName = Path.Combine(Desktop, $"Queries.json");
                    File.WriteAllText(fileName, wolfpack.Results.ToString());

                    c.Open();
                    await using var cmd = new NpgsqlCommand(sqlQuery, c);
                    {
                        var resultBson = cmd.CreateParameter();
                        resultBson.ParameterName = "result";
                        resultBson.NpgsqlDbType = NpgsqlDbType.Json;
                        resultBson.Value = wolfpack.Results.ToString();
    
                        cmd.Parameters.AddWithValue("docName", wolfpack.DocumentName);
                        cmd.Parameters.AddWithValue("origin", wolfpack.FileOrigin);
                        cmd.Parameters.AddWithValue("version", wolfpack.DocumentVersion);
                        cmd.Parameters.AddWithValue("completed", wolfpack.WasCompleted);
                        cmd.Parameters.AddWithValue("time", wolfpack.TimeTaken);
                        cmd.Parameters.AddWithValue("creation", wolfpack.CreatedAt);
                        cmd.Parameters.AddWithValue("id", wolfpack.GUID);
                        cmd.Parameters.AddWithValue("resCount", wolfpack.ResultCount);
                        cmd.Parameters.AddWithValue("name", wolfpack.TestName);
                        cmd.Parameters.Add(resultBson);

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
