using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Direwolf.Contracts;
using Direwolf.Definitions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct RevitWolfpack : IWolfpack
    {
        public RevitWolfpack(Document doc,
            IHowler howler,
            string fileOrigin = "",
            string testName = "",
            double timeTaken = 0,
            bool wasCompleted = false)
        {
            CreatedAt = DateTime.UtcNow;
            FileOrigin = fileOrigin;
            GUID = new Guid();
            ResultCount = howler.Den.Count;
            TestName = testName;
            TimeTaken = timeTaken;
            WasCompleted = wasCompleted;
            Document = new DocumentIntrospection(doc);
            ProjectInformation = new ProjectInformationIntrospection(doc);
            Site = new ProjectSiteIntrospection(doc);
            Units = new ProjectUnitsIntrospection(doc);
            foreach (var w in doc.GetWarnings())
            {
                Warnings.Add(new WarningIntrospection(w));
            }
            Results = howler.ToString();
        }
        public DateTime CreatedAt { get; init; }
        public string FileOrigin { get; init; }
        public Guid GUID { get; init; }
        public int ResultCount { get; init; }
        public string? Results { get; init; }
        public string TestName { get; init; }
        public double TimeTaken { get; init; }
        public bool WasCompleted { get; init; }
        public DocumentIntrospection Document { get; init; }
        public ProjectInformationIntrospection ProjectInformation { get; init; }
        public ProjectSiteIntrospection Site { get; init; }
        public ProjectUnitsIntrospection Units { get; init; }
        public List<WarningIntrospection> Warnings { get; init; } = [];
    }
}
