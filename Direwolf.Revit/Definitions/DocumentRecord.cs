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
        public Guid RecordUniqueId { get; init; }
        public string? DocumentName { get; init; }
        public string? DocumentPath { get; init; }
        public required string DocumentUniqueId { get; init; }
        public string? DocumentVersionId { get; init; }
        public int DocumentSaveCount { get; init; }
        public string? ProjectName { get; init; }
        public string? ProjectClient { get; init; }
        public string? ProjectAddress { get; init; }
        public string? ProjectAuthor { get; init; }
        public string? ProjectBuildingName { get; init; }
        public string? ProjectIssueDate { get; init; }
        public string? ProjectLocation { get; init; }
        public string? ProjectNumber { get; init; }
        public string? ProjectOrganizationDescription { get; init; }
        public string? ProjectOrganizationName { get; init; }
        public string? ProjectStatus { get; init; }
        public string? ProjectPlaceName { get; init; }
        public double? ProjectElevation { get; init; }
        public double? ProjectLatitude { get; init; }
        public double? ProjectLongitude { get; init; }
        public double? ProjectTimeZone { get; init; }
        public string? ProjectGeoCoordinateSystemId { get; init; }
        public string? ProjectGeoCoordinateSystemDefinition { get; init; }
        public string? ProjectSpecLength { get; init; }
        public string? ProjectSpecArea { get; init; }
        public string? ProjectSpecAngle { get; init; }
        public string? ProjectSpecCurrency { get; init; }
        public string? ProjectSpecNumber { get; init; }
        public string? ProjectSpecRotationAngle { get; init; }
        public string? ProjectSpecSheetLength { get; init; }
        public string? ProjectSpecSiteAngle { get; init; }
        public string? ProjectSpecSlope { get; init; }
        public string? ProjectSpecSpeed { get; init; }
        public string? ProjectSpecTime { get; init; }
        public string? ProjectSpecVolume { get; init; }
        public Guid ProjectWarningsFK { get; init; }
        public DocumentRecord(Document document)
        {
            // Document
            DocumentName = document.Title;
            DocumentPath = document.PathName;
            DocumentUniqueId = document.CreationGUID.ToString();
            DocumentVersionId = Document.GetDocumentVersion(document).VersionGUID.ToString();
            DocumentSaveCount = Document.GetDocumentVersion(document).NumberOfSaves;

            // ProjectWarnings
            //ProjectWarnings = new(document);

            // Project
            ProjectName = document.ProjectInformation.Name ?? string.Empty;
            ProjectClient = document.ProjectInformation.ClientName ?? string.Empty;
            ProjectAddress = document.ProjectInformation.Address ?? string.Empty;
            ProjectAuthor = document.ProjectInformation.Author ?? string.Empty;
            ProjectBuildingName = document.ProjectInformation.BuildingName ?? string.Empty;
            ProjectIssueDate = document.ProjectInformation.IssueDate ?? string.Empty;
            ProjectLocation = document.ProjectInformation.Location?.ToString() ?? string.Empty;
            ProjectNumber = document.ProjectInformation.Number ?? string.Empty;
            ProjectOrganizationDescription = document.ProjectInformation.OrganizationDescription ?? string.Empty;
            ProjectOrganizationName = document.ProjectInformation.OrganizationName ?? string.Empty;
            ProjectStatus = document.ProjectInformation.Status ?? string.Empty;

            // Place
            ProjectPlaceName = document.SiteLocation.PlaceName ?? string.Empty;
            ProjectElevation = document.SiteLocation.Elevation;
            ProjectLatitude = document.SiteLocation.Latitude;
            ProjectLongitude = document.SiteLocation.Longitude;
            ProjectTimeZone = document.SiteLocation.TimeZone;
            ProjectGeoCoordinateSystemId = document.SiteLocation.GeoCoordinateSystemId ?? string.Empty;
            ProjectGeoCoordinateSystemDefinition = document.SiteLocation.GeoCoordinateSystemDefinition ?? string.Empty;

            // Units
            ProjectSpecLength = document.GetUnits().GetFormatOptions(SpecTypeId.Length).GetUnitTypeId().TypeId;
            ProjectSpecArea = document.GetUnits().GetFormatOptions(SpecTypeId.Area).GetUnitTypeId().TypeId;
            ProjectSpecAngle = document.GetUnits().GetFormatOptions(SpecTypeId.Angle).GetUnitTypeId().TypeId;
            ProjectSpecCurrency = document.GetUnits().GetFormatOptions(SpecTypeId.Currency).GetUnitTypeId().TypeId;
            ProjectSpecNumber = document.GetUnits().GetFormatOptions(SpecTypeId.Number).GetUnitTypeId().TypeId;
            ProjectSpecRotationAngle = document.GetUnits().GetFormatOptions(SpecTypeId.RotationAngle).GetUnitTypeId().TypeId;
            ProjectSpecSheetLength = document.GetUnits().GetFormatOptions(SpecTypeId.SheetLength).GetUnitTypeId().TypeId;
            ProjectSpecSiteAngle = document.GetUnits().GetFormatOptions(SpecTypeId.SiteAngle).GetUnitTypeId().TypeId;
            ProjectSpecSlope = document.GetUnits().GetFormatOptions(SpecTypeId.Slope).GetUnitTypeId().TypeId;
            ProjectSpecSpeed = document.GetUnits().GetFormatOptions(SpecTypeId.Speed).GetUnitTypeId().TypeId;
            ProjectSpecTime = document.GetUnits().GetFormatOptions(SpecTypeId.Time).GetUnitTypeId().TypeId;
            ProjectSpecVolume = document.GetUnits().GetFormatOptions(SpecTypeId.Volume).GetUnitTypeId().TypeId;
        }
    }
}
