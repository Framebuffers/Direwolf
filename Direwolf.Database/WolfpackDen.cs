using Autodesk.Revit.DB;
using Direwolf;
using Direwolf.Definitions;
using Npgsql;
using System.Threading.Tasks;

namespace Direwolf.Database
{
    public class WolfpackDen
    {
        private WolfpackDen? Den => this;
        private NpgsqlConnection? _connect;

        public async Task<WolfpackDen> Connect(string connectionString = null)
        {
            try
            {
                var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
                var datasource = dataSourceBuilder.Build();
                _connect = await datasource.OpenConnectionAsync();
                return this;
            }
            catch (Exception e)
            {
                throw new Exception($"No connection has been made: connection has failed. Reason: {e.Message}");
            }

        }

        public async Task<WolfpackDen> CreateDen(Dictionary<string, Wolfpack> queries, string? connection = null)
        {
            try
            {
                if (_connect is null && connection is not null)
                {
                    await using var dataSource = NpgsqlDataSource.Create(connection);

                }
                if (_connect is null && connection is null)
                {
                    throw new Exception("No connection has been made, and no connection string has been provided.");
                }
                return this;
            }
            catch (Exception e)
            {
                throw new Exception($"No connection has been made: connection has failed. Reason: {e.Message}");
            }
        }
    }
}
