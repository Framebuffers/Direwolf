using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Microsoft.XmlSerializer.Generator;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Text.Json.Serialization;

namespace Direwolf.ManifestGenerator
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ManifestAttributes : System.Attribute
    {
        private Guid ExtensionGuid => new();

        public ManifestAttributes(string revitAddinInstallationPath,
                                  string vendorName = "unknown",
                                  string vendorDescription = "unknown",
                                  string addinName = "unknown",
                                  string addinDescription = "unknown",
                                  string revitDiscipline = "unknown",
                                  string locale = "unknown",
                                  bool alwaysVisible = true)
        {
        }
    }

    public enum VisibilityModeMembers
    {
        AlwaysVisible,
        NotVisibleInProject,
        NotVisibleInFamily,
        NotVisibleWhenNoActiveDocument
    }

    public enum DisciplineMembers
    {
        Any,
        Architecture,
        Structure,
        StructuralAnalysis,
        MassingAndSite,
        EnergyAnalysis,
        Mechanical,
        Electrical,
        Piping,
        MechanicalAnalysis,
        PipingAnalysis,
        ElectricalAnalysis
    }

    public enum RevitAddinType
    {
        Command,
        Application,
        DBApplication
    }

    [Serializable] 
    public record struct RevitAddinManifest : IXmlSerializable
    {
        public RevitAddinType AddinType { get; set; }
        public string Assembly { get; set; }
        public string FullClassName { get; set; }
        public Guid AddInId { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string VendorId { get; set; }
        public string VendorDescription { get; set; }
        public string Description { get; set; }
        public IEnumerable<VisibilityModeMembers> VisibilityMode { get; set; }
        public IEnumerable<DisciplineMembers> Discipline { get; set; }
        public string AvailabilityClassName { get; set; }
        public string LargeImage { get; set; }
        public string SmallImage { get; set; }
        public string LongDescription { get; set; }
        public string TooltipImage { get; set; }
        public string LanguageType { get; set; }
        public string AllowLoadIntoExistingSession { get; set; }

        public XmlSchema? GetSchema() => null!;

        public void ReadXml(XmlReader reader)
        {
            // parse enums

            // some fields may be empty, and they must not be serialized.
            // this makes the process less annoying.
            //
            // however, some input sanitization must be performed first:
            // AddinId:
            //      must be able to parse as Guid,
            // Name:
            //      required; for ExternalApplications only,
            // Text:
            //      ExternalCommands only,
            //      default = "External Tool",
            // VendorId:
            //      should be a valid ASCII string,
            // VendorDescription:
            //      can be null,
            //      optional,
            // Description:
            //      ExternalApplications only,
            //      default = Text's content
            // VisibilityMode:
            //      multiple allowed,
            //      ExternalCommand only,
            //      default = all modes,
            //      should parse to the VisibilityModeMembers enum,
            //      whitelists possible modes,
            // Discipline(s):
            //      should parse to the DisciplineMembers enum,
            //      should only be used in ExternalCommands,
            //      whitelists possible disciplines,
            //      multiple allowed
            // AvailabilityClassName:
            //      ExternalCommand only,
            //      should inherit from IExternalCommandAvailability,
            // LargeImage, SmallImage, TooltipImage:
            //      ExternalCommand only,
            //      should *always* be a valid path,
            //      check for file extensions (usually JPG or PNG),
            //      can be null,
            // LongDescription:
            //      ExternalCommand only.
            // LanguageType:
            //      loaded from the DLL resources by default,
            // AllowLoadIntoExistingSession:
            //      optional,
            //      false = new addin manifests can only be loaded after Revit restarts.
            //      true by default

            void skipIfNull(string result)
            {
                if (String.IsNullOrEmpty(result)) reader.Skip();
            }
            
            string revitAddInType = reader.MoveToAttribute("AddinType") ? reader.Value : null!;
            string visibilityMode = reader.MoveToAttribute("VisibilityMode") ? reader.Value : null!;
            string disciplineMembers = reader.MoveToAttribute("DisciplineMembers") ? reader.Value : null!;
            
            
            
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}