using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Forest
{
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

    public class ExternalCommandsOnlyAttribute : Attribute { }
    public class DBApplicationsOnlyAttribute : Attribute { }
    public class ApplicationsOnlyAttribute : Attribute { }
    public class PathOnlyAttribute : Attribute { }
    public class OptionalPropertyAttribute : Attribute { }
    public class RequiredPropertyAttribute : Attribute { }
    public class VisibilityModeTypeAttribute : Attribute { }
    public class DisciplineTypeAttribute : Attribute { }

    public record struct RevitAddinManifest 
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

    }

    public class CreateManifest : Attribute
    {
        public CreateManifest(string asm)
        {
            RevitAddinManifest m = new();
            m.Assembly = asm;
            Console.WriteLine(asm.ToString());
        }
    }
}
