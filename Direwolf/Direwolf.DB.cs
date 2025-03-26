using Direwolf.Contracts;
using Direwolf.Definitions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Direwolf
{
    public enum Target
    {
        Invalid,
        JSON,
        DB,
        Excel,
        Screen
    }

    public enum DatabaseChoice
    {
        Invalid,
        Postgres
    }



    public partial class Direwolf
    {
        public event EventHandler? DatabaseConnectionEventHandler;
        /// <summary>
        /// This is a proof of concept, not a production-ready solution. Please **CHANGE THIS** if you plan to deploy.
        /// </summary>
        private static readonly DbConnectionString _default = new("localhost", 5432, "wolf", "awoo", "direwolf");

        [JsonExtensionData] protected virtual WolfpackDB Queries { get; set; } = new WolfpackDB(_default);

        private void Queries_DatabaseConnectedEventHandler(object? sender, EventArgs e)
        {
            Debug.Print("Database connected!");
        }

        public async void SendAllToJson()
        {

        }

        public async void SendAllToDB()
        {
            try { await Queries.Send(); } catch (Exception e) { Debug.Print(e.Message); }
        }

        public async void SendAllToExcel()
        {

        }
        public async void SendAllToScreen()
        {

        }
        public async void SendToDynamo()
        {

        }
        public async void GetFromDynamo(IWolfpack wolfpack)
        {

        }
    }
}
