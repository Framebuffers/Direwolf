using Direwolf.Definitions;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Direwolf.DB
{
    public readonly record struct DbConnectionString(string Host, string Username, string Password, string Database) { }

    public class DirewolfDB : Stack<Wolfpack>
    {
        private readonly DbConnectionString _str;
        private const string TABLE_NAME = "Wolfpack";
        private NpgsqlConnection _connection;

        public DirewolfDB(DbConnectionString db)
        {
            _str = db;
        }


        public Stack<Wolfpack> Queries { get; set; } = [];
        public async Task Add(Wolfpack wolfpack)
        {
            using NpgsqlConnection c = new(
                $"Host={_str.Host};" +
                $"Username={_str.Username};" +
                $"Password={_str.Password};" +
                $"Database={_str.Database}");

            string sqlQuery =
                $"INSERT INTO {TABLE_NAME} (documentName, fileOrigin, documentVersion, wasCompleted, timeTaken, createdAt, guid, resultCount, testName, results, testId) VALUES (@docName, @origin, @version, @completed, @time, @creation, @id, @resCount, @name, @result, @testNumber)";

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

                await cmd.ExecuteNonQueryAsync();
            }
            c.Dispose();
        }
        
        
    }
}
