//using Autodesk.Revit.DB;
//using Autodesk.Revit.Exceptions;
//using Direwolf.Contracts;
//using Direwolf.Definitions;
//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Direwolf.Revit.Definitions.Legacy
//{
//    public readonly record struct L_RevitWolfpack : IWolfpack
//    {
//        public L_RevitWolfpack(Document doc,
//            IHowler howler,
//            string fileOrigin = "",
//            string testName = "",
//            double timeTaken = 0,
//            bool wasCompleted = false)
//        {
//            CreatedAt = DateTime.UtcNow;
//            FileOrigin = fileOrigin;
//            GUID = new Guid().ToString();
//            ResultCount = howler.Den.Count;
//            Name = testName;
//            TimeTaken = timeTaken;
//            WasCompleted = wasCompleted;
//            //Document = new DocumentIntrospection(doc);
//            ProjectInformation = new L_ProjectInformationIntrospection(doc);
//            Site = new L_ProjectSiteIntrospection(doc);
//            Units = new L_ProjectUnitsIntrospection(doc);
//            foreach (var w in doc.GetWarnings())
//            {
//                Warnings.Add(new WarningRecord(w));
//            }
//            Results = howler.ToString();

//            BasicFileInfo b = BasicFileInfo.Extract(doc.PathName);
//            DocumentSessionId = b.GetDocumentVersion().VersionGUID.ToString();
//            DocumentCreationId = doc.CreationGUID.ToString();
//            ChangedElements = [.. doc.GetChangedElements(b.GetDocumentVersion().VersionGUID).GetCreatedElementIds().Select(x => x.Value)];
//        }
//        public DateTime CreatedAt { get; init; }
//        public string FileOrigin { get; init; }
//        public string GUID { get; init; }
//        public string DocumentSessionId { get; init; }
//        public string DocumentCreationId { get; init; }
//        public List<long> ChangedElements { get; init; }
//        public int ResultCount { get; init; }
//        public string? Results { get; init; }
//        public string Name { get; init; }
//        public double TimeTaken { get; init; }
//        public bool WasCompleted { get; init; }
//        //public DocumentIntrospection Document { get; init; }
//        public L_ProjectInformationIntrospection ProjectInformation { get; init; }
//        public L_ProjectSiteIntrospection Site { get; init; }
//        public L_ProjectUnitsIntrospection Units { get; init; }
//        public List<WarningRecord> Warnings { get; init; } = [];

//        public static string ToSql()
//        {
//            return """
//                INSERT INTO "Wolfpack"(
//                "projectInformation",
//                "documentInformation",
//                "siteInformation",
//                "unitsInformation",
//                "warnings",
//                "documentSessionId",
//                "documentCreationId",
//                "changedElements",
//                "fileOrigin",
//                "wasCompleted",
//                "timeTaken",
//                "createdAt",
//                "guid",
//                "resultCount",
//                "testName",
//                "results",
//                "elementInfo"
//                ) VALUES (
//                @projectInformation,
//                @documentInformation,
//                @siteInformation,
//                @warnings,
//                @documentSessionId,
//                @changedElements,
//                @fileOrigin,
//                @wasCompleted,
//                @timeTaken,
//                @createdAt,
//                @guid,
//                @resultCount,
//                @testName,
//                @results,
//                @elementInfo)
//                ON CONFLICT (guid) DO NOTHING;
//                """;
//        }
//    }
//}
