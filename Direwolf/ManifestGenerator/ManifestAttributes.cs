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
using System.Xml.Linq;

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
    [XmlRoot("RevitAddIn")]
    public record struct RevitAddinManifest : IXmlSerializable
    {
        // some input sanitization must be performed first:
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

        [XmlElement]
        public RevitAddinType AddinType { get; set; }

        [XmlElement]
        public string Assembly { get; set; }

        [XmlElement]
        public string FullClassName { get; set; }

        [XmlElement]
        public Guid AddInId { get; set; }

        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string Text { get; set; }

        [XmlElement]
        public string VendorId { get; set; }

        [XmlElement]
        public string VendorDescription { get; set; }

        [XmlElement]
        public string Description { get; set; }

        [XmlArray("VisibilityMode")]
        public IEnumerable<VisibilityModeMembers> VisibilityMode { get; set; }

        [XmlArray("Discipline")]
        public IEnumerable<DisciplineMembers> Discipline { get; set; }

        [XmlElement]
        public string AvailabilityClassName { get; set; }

        [XmlElement]
        public string LargeImage { get; set; }

        [XmlElement]
        public string SmallImage { get; set; }

        [XmlElement]
        public string LongDescription { get; set; }

        [XmlElement]
        public string TooltipImage { get; set; }

        [XmlElement]
        public string LanguageType { get; set; }

        [XmlElement]
        public string AllowLoadIntoExistingSession { get; set; }

        public XmlSchema? GetSchema() => null!;

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();

            //var newManifest = new RevitAddinManifest();
            //// parse enums

            //// some fields may be empty, and they must not be serialized.
            //// this makes the process less annoying.

            ////string revitAddInType = reader.MoveToAttribute("AddinType") ? reader.Value : null!;
            ////string visibilityMode = reader.MoveToAttribute("VisibilityMode") ? reader.Value : null!;
            ////string disciplineMembers = reader.MoveToAttribute("DisciplineMembers") ? reader.Value : null!;

            //string revitAddInType = reader.MoveToAttribute("AddinType") ? reader.Value : null!;
            //string assembly = reader.MoveToAttribute("Assembly") ? reader.Value : null!;
            //string fullClassName = reader.MoveToAttribute("FullClassName") ? reader.Value : null!;
            //string addInId = reader.MoveToAttribute("AddInId") ? reader.Value : null!;
            //string name = reader.MoveToAttribute("Name") ? reader.Value : null!;
            //string text = reader.MoveToAttribute("Text") ? reader.Value : null!;
            //string vendorId = reader.MoveToAttribute("VendorId") ? reader.Value : null!;
            //string vendorDescription = reader.MoveToAttribute("VendorDescription") ? reader.Value : null!;
            //string description = reader.MoveToAttribute("Description") ? reader.Value : null!;
            //string visibilityMode = reader.MoveToAttribute("VisibilityMode") ? reader.Value : null!;
            //string discipline = reader.MoveToAttribute("Discipline") ? reader.Value : null!;
            //string availabilityClassName = reader.MoveToAttribute("AvailabilityClassName") ? reader.Value : null!;
            //string largeImage = reader.MoveToAttribute("LargeImage") ? reader.Value : null!;
            //string smallImage = reader.MoveToAttribute("SmallImage") ? reader.Value : null!;
            //string longDescription = reader.MoveToAttribute("LongDescription") ? reader.Value : null!;
            //string tooltipImage = reader.MoveToAttribute("TooltipImage") ? reader.Value : null!;
            //string languageType = reader.MoveToAttribute("LanguageType") ? reader.Value : null!;
            //string allowLoadIntoExistingSession = reader.MoveToAttribute("AllowLoadIntoExistingSession") ? reader.Value : null!;
            
            

        }

        public void WriteXml(XmlWriter writer)
        {
            XDocument manifest = new(new XDeclaration("1.0", "utf-8", "no"));
            using (var w = manifest.CreateWriter())
            {
                XElement revitAddInsNode = new("RevitAddIns");

                //switch (AddinType)
                //{
                //    case RevitAddinType.Command:

                //        break;
                //    case RevitAddinType.Application:
                //        break;
                //    case RevitAddinType.DBApplication:
                //        break;
                //}
            }
        }
    }
}