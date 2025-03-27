using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Direwolf.Contracts;
using Direwolf.Definitions;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml;

namespace Direwolf.Revit.Definitions.Legacy
{
    public class L_RevitWolfpackDB : Stack<L_RevitWolfpack>, IWolfpackDB
    {
        public L_RevitWolfpackDB(DbConnectionString str, Document doc)
        {
            _str = str;
            _doc = doc;
            DatabaseConnectedEventHandler += WolfpackDB_DatabaseConnectedEventHandler;
        }

        private readonly DbConnectionString _str;
        private readonly Document _doc;
        public event EventHandler? DatabaseConnectedEventHandler;

        public async Task Send()
        {
            //string sqlQuery =
            //    """INSERT INTO "Wolfpack" ("documentName", "fileOrigin", "documentVersion", "wasCompleted", "timeTaken", "createdAt", "guid", "resultCount", "testName", "results") VALUES (@docName, @origin, @version, @completed, @time, @creation, @id, @resCount, @name, @result)""";

            //string sql = """INSERT INTO Wolfpack (projectInformation, documentInformation, siteInformation, unitsInformation, warnings, documentSessionId, documentCreationId, changedElements, fileOrigin,  wasCompleted,  timeTaken, createdAt, guid, resultCount, testName, results, elementInfo) VALUES(@projectInformation, @documentInformation, @siteInformation, @unitsInformation, @warnings, @documentSessionId, @documentCreationId, @changedElements, @fileOrigin, @wasCompleted, @timeTaken, @createdAt, @guid, @resultCount, @testName, @results, @elementInfo""";


            try
            {
                using NpgsqlConnection c = new($"Host={_str.Host};Port={_str.Port};Username={_str.Username};Password={_str.Password};Database={_str.Database}");
                DatabaseConnectedEventHandler?.Invoke(this, new EventArgs());
                while (Count > 0)
                {
                    // unpack the Wolfpack
                    L_RevitWolfpack rp = Pop();
                    //DocumentIntrospection introspection = rp.Document;
                    L_ProjectInformationIntrospection project = rp.ProjectInformation;
                    L_ProjectSiteIntrospection pjs = rp.Site;
                    L_ProjectUnitsIntrospection units = rp.Units;
                    List<WarningRecord> warn = rp.Warnings;


                    //string diSql = DocumentIntrospection.AsSql();
                    string piSql = L_ProjectInformationIntrospection.AsSql();
                    string psSql = L_ProjectSiteIntrospection.AsSql();
                    string puSql = L_ProjectUnitsIntrospection.AsSql();
                    //string waSql = WarningRecord.AsSql();

                    string sessionId = rp.DocumentSessionId;
                    string creationId = rp.DocumentCreationId;
                    List<long> changedElements = rp.ChangedElements;
                    string fileOrigin = rp.FileOrigin;
                    bool wasCompleted = rp.WasCompleted;
                    double timeTaken = rp.TimeTaken;
                    DateTime createdAt = rp.CreatedAt;
                    string guid = rp.GUID;
                    int resultCount = rp.ResultCount;
                    string testName = rp.TestName;
                    string results = rp.Results;



                    //Console.WriteLine(c.ConnectionString);
                    //c.Open();
                    //await using var cmd = new NpgsqlCommand(sqlQuery, c);
                    //{
                    //    var resultJson = cmd.CreateParameter();
                    //    resultJson.ParameterName = "result";
                    //    resultJson.NpgsqlDbType = NpgsqlDbType.Json;
                    //    resultJson.Value = wolfpack.Results;
                    //    cmd.Parameters.AddWithValue("docName", wolfpack.DocumentName);
                    //    cmd.Parameters.AddWithValue("origin", wolfpack.FileOrigin);
                    //    cmd.Parameters.AddWithValue("version", wolfpack.DocumentVersion);
                    //    cmd.Parameters.AddWithValue("completed", wolfpack.WasCompleted);
                    //    cmd.Parameters.AddWithValue("time", wolfpack.TimeTaken);
                    //    cmd.Parameters.AddWithValue("creation", wolfpack.CreatedAt);
                    //    cmd.Parameters.AddWithValue("id", wolfpack.GUID);
                    //    cmd.Parameters.AddWithValue("resCount", wolfpack.ResultCount);
                    //    cmd.Parameters.AddWithValue("name", wolfpack.TestName);
                    //    cmd.Parameters.Add(resultJson);

                    //    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    //    Debug.Print($"Executed query. Added {rowsAffected} rows");
                    //}
                    //c.Dispose();
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
        }
        private void WolfpackDB_DatabaseConnectedEventHandler(object? sender, EventArgs e) => Debug.Print("Database Connected");
    }
}
