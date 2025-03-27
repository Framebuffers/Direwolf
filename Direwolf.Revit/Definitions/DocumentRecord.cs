using Autodesk.Revit.DB;
using Direwolf.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct DocumentRecord
    {
        public Guid RecordUniqueId { get; init; } = Guid.NewGuid();
        public string? DocumentName { get; init; }
        public string? DocumentPath { get; init; }
        public required string DocumentUniqueId { get; init; }
        public string? DocumentVersionId { get; init; }
        public int DocumentSaveCount { get; init; }
        public string? ProjectName { get; init; }
        public string? Client { get; init; }
        public string? Address { get; init; }
        public string? Author { get; init; }
        public string? BuildingName { get; init; }
        public string? IssueDate { get; init; }
        public string? Location { get; init; }
        public string? ProjectNumber { get; init; }
        public string? OrganizationDescription { get; init; }
        public string? OrganizationName { get; init; }
        public string? Status { get; init; }
        public string? PlaceName { get; init; }
        public double? Elevation { get; init; }
        public double? Latitude { get; init; }
        public double? Longitude { get; init; }
        public double? TimeZone { get; init; }
        public string? GeoCoordinateSystemId { get; init; }
        public string? GeoCoordinateSystemDefinition { get; init; }
        public string? LengthUnits { get; init; }
        public string? AreaUnits { get; init; }
        public string? Angle { get; init; }
        public string? Currency { get; init; }
        public string? Number { get; init; }
        public string? RotationAngle { get; init; }
        public string? SheetLength { get; init; }
        public string? SiteAngle { get; init; }
        public string? Slope { get; init; }
        public string? Speed { get; init; }
        public string? Time { get; init; }
        public string? Volume { get; init; }
        public WarningRecords? Warnings { get; init; }
        public DocumentRecord(Document document)
        {
            // Document
            DocumentName = document.Title;
            DocumentPath = document.PathName;
            DocumentUniqueId = document.CreationGUID.ToString();
            DocumentVersionId = Document.GetDocumentVersion(document).VersionGUID.ToString();
            DocumentSaveCount = Document.GetDocumentVersion(document).NumberOfSaves;

            // Warnings
            Warnings = new(document);

            // Project
            ProjectName = document.ProjectInformation.Name ?? string.Empty;
            Client = document.ProjectInformation.ClientName ?? string.Empty;
            Address = document.ProjectInformation.Address ?? string.Empty;
            Author = document.ProjectInformation.Author ?? string.Empty;
            BuildingName = document.ProjectInformation.BuildingName ?? string.Empty;
            IssueDate = document.ProjectInformation.IssueDate ?? string.Empty;
            Location = document.ProjectInformation.Location?.ToString() ?? string.Empty;
            ProjectNumber = document.ProjectInformation.Number ?? string.Empty;
            OrganizationDescription = document.ProjectInformation.OrganizationDescription ?? string.Empty;
            OrganizationName = document.ProjectInformation.OrganizationName ?? string.Empty;
            Status = document.ProjectInformation.Status ?? string.Empty;

            // Place
            PlaceName = document.SiteLocation.PlaceName ?? string.Empty;
            Elevation = document.SiteLocation.Elevation;
            Latitude = document.SiteLocation.Latitude;
            Longitude = document.SiteLocation.Longitude;
            TimeZone = document.SiteLocation.TimeZone;
            GeoCoordinateSystemId = document.SiteLocation.GeoCoordinateSystemId ?? string.Empty;
            GeoCoordinateSystemDefinition = document.SiteLocation.GeoCoordinateSystemDefinition ?? string.Empty;

            // Units
            LengthUnits = document.GetUnits().GetFormatOptions(SpecTypeId.Length).GetUnitTypeId().TypeId;
            AreaUnits = document.GetUnits().GetFormatOptions(SpecTypeId.Area).GetUnitTypeId().TypeId;
            Angle = document.GetUnits().GetFormatOptions(SpecTypeId.Angle).GetUnitTypeId().TypeId;
            Currency = document.GetUnits().GetFormatOptions(SpecTypeId.Currency).GetUnitTypeId().TypeId;
            Number = document.GetUnits().GetFormatOptions(SpecTypeId.Number).GetUnitTypeId().TypeId;
            RotationAngle = document.GetUnits().GetFormatOptions(SpecTypeId.RotationAngle).GetUnitTypeId().TypeId;
            SheetLength = document.GetUnits().GetFormatOptions(SpecTypeId.SheetLength).GetUnitTypeId().TypeId;
            SiteAngle = document.GetUnits().GetFormatOptions(SpecTypeId.SiteAngle).GetUnitTypeId().TypeId;
            Slope = document.GetUnits().GetFormatOptions(SpecTypeId.Slope).GetUnitTypeId().TypeId;
            Speed = document.GetUnits().GetFormatOptions(SpecTypeId.Speed).GetUnitTypeId().TypeId;
            Time = document.GetUnits().GetFormatOptions(SpecTypeId.Time).GetUnitTypeId().TypeId;
            Volume = document.GetUnits().GetFormatOptions(SpecTypeId.Volume).GetUnitTypeId().TypeId;
        }
    }
}
