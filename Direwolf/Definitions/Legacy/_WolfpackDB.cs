// using System.Data;
// using System.Diagnostics;
// using Npgsql;
// using NpgsqlTypes;
//
// namespace Direwolf.Definitions;
//
// /// <summary>
// ///     Postgres connection string.
// /// </summary>
// /// <param name="Host">Database host address</param>
// /// <param name="Port">Postgres database port</param>
// /// <param name="Username">Database username</param>
// /// <param name="Password">User password</param>
// /// <param name="Database">Database name</param>
// public readonly record struct DbConnectionString(
//     string Host,
//     int Port,
//     string Username,
//     string Password,
//     string Database);
//
// /// <summary>
// ///     Stack of <see cref="Wolfpack" /> queries, ready to be sent to a database.
// /// </summary>
// public class WolfpackDb(DbConnectionString db) : Stack<Wolfpack>
// {
//     public event EventHandler? DatabaseConnectedEventHandler;
//
//     /// <summary>
//     ///     Connects to a database with a schema following the <see cref="Wolfpack" /> structure, and pushes the results held
//     ///     inside up to the target DB.
//     /// </summary>
//     /// <returns></returns>
//     public async Task Send()
//     {
//         DatabaseConnectedEventHandler?.Invoke(this, EventArgs.Empty);
//         const string sqlQuery = """
//                                 INSERT INTO "Wolfpack" (
//                                             "documentName", 
//                                             "fileOrigin", 
//                                             "documentVersion", 
//                                             "wasCompleted", 
//                                             "timeTaken", 
//                                             "createdAt", 
//                                             "guid", 
//                                             "resultCount", 
//                                             "testName", 
//                                             "results"
//                                             ) VALUES (
//                                             @docName, 
//                                             @origin, 
//                                             @version, 
//                                             @completed, 
//                                             @time, 
//                                             @creation, 
//                                             @id, 
//                                             @resCount, 
//                                             @name, 
//                                             @result);
//                                 """;
//         try
//         {
//             await using NpgsqlConnection c =
//                 new(
//                     $"Host={db.Host};" +
//                     $"Port={db.Port};" +
//                     $"Username={db.Username};" +
//                     $"Password={db.Password};" +
//                     $"Database={db.Database}");
//
//             DatabaseConnectedEventHandler?.Invoke(this, EventArgs.Empty);
//             c.StateChange += C_StateChange;
//             c.Notice += C_Notice;
//
//             while (Count > 0)
//             {
//                 var wolfpack = Pop();
//                 c.Open();
//                 await using var cmd = new NpgsqlCommand(sqlQuery, c);
//                 {
//                     if (wolfpack.GenerateResults is not null)
//                     {
//                         var resultBson = cmd.CreateParameter();
//                         resultBson.ParameterName = "result";
//                         resultBson.NpgsqlDbType = NpgsqlDbType.Json;
//                         resultBson.Value = wolfpack.GenerateResults;
//                         cmd.Parameters.Add(resultBson);
//                     }
//                     cmd.Parameters.AddWithValue("docName", wolfpack.DocumentName);
//                     cmd.Parameters.AddWithValue("origin", wolfpack.FileOrigin);
//                     cmd.Parameters.AddWithValue("version", wolfpack.DocumentVersion);
//                     cmd.Parameters.AddWithValue("completed", wolfpack.WasCompleted);
//                     cmd.Parameters.AddWithValue("time", wolfpack.TimeTaken);
//                     cmd.Parameters.AddWithValue("creation", wolfpack.CreatedAt);
//                     cmd.Parameters.AddWithValue("id", wolfpack.Guid);
//                     cmd.Parameters.AddWithValue("resCount", wolfpack.ResultCount);
//                     cmd.Parameters.AddWithValue("name", wolfpack.Name);
//                     var rowsAffected = await cmd.ExecuteNonQueryAsync();
//                 }
//                 c.Dispose();
//             }
//         }
//         catch (Exception e)
//         {
//             Debug.Print(e.Message);
//         }
//     }
//
//     private static void C_Notice(object sender, NpgsqlNoticeEventArgs e)
//     {
//         Debug.Print($"Postgres Notice: {e.Notice.Severity}; {e.Notice.MessageText}");
//     }
//
//     private static void C_StateChange(object sender, StateChangeEventArgs e)
//     {
//         Debug.Print($"Connection status: {e.CurrentState}");
//     }
// }

