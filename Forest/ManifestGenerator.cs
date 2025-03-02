using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Forest
{
    //// Gets the class name as Name: Assembly.Class
    //[RevitCommand("test", "id", "test")]
    //public readonly record struct AddinFullClassName
    //{
    //    public string Name { get; init; } = default!;
    //    public override string ToString()
    //    {
    //        XElement x = new("FullClassName", Name);
    //        return x.ToString();
    //    }
    //    public AddinFullClassName(Type a) => Name = a is not null ? a.FullName ?? string.Empty : string.Empty;
    //}

    //public readonly record struct AssemblyPath(string path) { }
    //public readonly record struct AddinGuid
    //{
    //    public AddinGuid() => Guid = Guid.NewGuid();
    //    public Guid Guid { get; init; } = default!;
    //}

    //[AttributeUsage(AttributeTargets.Struct)]
    //public class RevitApplication : Attribute
    //{
    //    public RevitApplication(string appName, string guid, string vendorId)
    //    {

    //    }
    //}

    //public class RevitCommand : RevitApplication
    //{
    //    public RevitCommand(string appName, string id, string vendorId) : base(appName, id, vendorId)
    //    {
    //        Console.WriteLine($"{appName} :: {id}, {vendorId}");
    //    }
    //}

    public record class Manifest(string AssemblyName, Guid AddInID, string FullClassName, string VendorID)
    {
    }

    public record struct AddInManifest()
    {
        public string Assembly { get; set; } = string.Empty;
        public Guid ClientID { get; set; } = Guid.NewGuid();
        public string FullClassName { get; set; } = string.Empty;
        public string VendorID { get; set; } = string.Empty;
        public string VendorDescription { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public bool AllowLoadingIntoExistingSession { get; set; } = true;
        public string ProductVersion { get; set; } = string.Empty;
        public string AddInName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool LoadInRevitWorker { get; set; } = false;
        public bool LoadInDedicatedRevitWorkerOnly { get; set; } = false;
    }

    public enum VisibilityMode
    {

    }

    public enum Discipline
    {

    }
    public enum LanguageType
    {

    }

    public record class CommandManifest(string AssemblyName, Guid AddInID, string FullClassName, string VendorID) : Manifest(AssemblyName, AddInID, FullClassName, VendorID)
    {
        public string Text { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LargeImage { get; set; } = string.Empty;
        public string LongDescription { get; set; } = string.Empty;
        public string TooltipImage { get; set; } = string.Empty;
        public VisibilityMode VisibilityMode { get; set; }
        public Discipline Discipline { get; set; }
        public string AvailabilityClassName { get; set; } = string.Empty;
        public string LanguageType { get; set; } = string.Empty;
        public string AddInName { get; set; } = string.Empty;
    }

    public record class ApplicationManifest(string Name, string AssemblyName, Guid AddInID, string FullClassName, string VendorID) : Manifest(AssemblyName, AddInID, FullClassName, VendorID) { }
    
    public record class DBApplicationManifest(string Name, string AssemblyName, Guid AddInID, string FullClassName, string VendorID) : Manifest(AssemblyName, AddInID, FullClassName, VendorID)
    {
        public bool LoadInRevitWorker { get; set; } = false;
        public bool LoadInDedicatedRevitWorkerOnly { get; set; } = false;
        internal string AddInName
        {
            get
            {
                return Name ?? base.FullClassName;
            }
        }
    }

    
    public class RevitApplication : Attribute
    {
        
    }



}
