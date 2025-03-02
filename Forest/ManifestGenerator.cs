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
 //public record struct AddInManifest()
    //{
    //    public string Assembly { get; set; } = string.Empty;
    //    public Guid ClientID { get; set; } = Guid.NewGuid();
    //    public string FullClassName { get; set; } = string.Empty;
    //    public string VendorID { get; set; } = string.Empty;
    //    public string VendorDescription { get; set; } = string.Empty;
    //    public string ProductImage { get; set; } = string.Empty;
    //    public string ProductDescription { get; set; } = string.Empty;
    //    public bool AllowLoadingIntoExistingSession { get; set; } = true;
    //    public string ProductVersion { get; set; } = string.Empty;
    //    public string AddInName { get; set; } = string.Empty;
    //    public string Name { get; set; } = string.Empty;
    //    public bool LoadInRevitWorker { get; set; } = false;
    //    public bool LoadInDedicatedRevitWorkerOnly { get; set; } = false;
    //}



    public record class Manifest(Guid AddInID,
                                 Type FullClassName,
                                 string VendorID,
                                 string AssemblyName)
    {
        //string t => base.GetType().FullName;
    }

   

    public record class CommandManifest(Guid AddInID,
                                        Type FullClassName,
                                        string VendorID,
                                        string AssemblyName) : Manifest(AddInID, FullClassName, VendorID, AssemblyName)
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

    public record class ApplicationManifest(string Name,
                                            Guid AddInID,
                                            Type FullClassName,
                                            string VendorID,
                                            string AssemblyName) : Manifest(AddInID, FullClassName, VendorID, AssemblyName)
    { }
    
    public record class DBApplicationManifest(string Name,
                                              Guid AddInID,
                                              Type FullClassName,
                                              string VendorID,
                                              string AssemblyName) : Manifest(AddInID, FullClassName, VendorID, AssemblyName)
    {
        public bool LoadInRevitWorker { get; set; } = false;
        public bool LoadInDedicatedRevitWorkerOnly { get; set; } = false;
        internal string AddInName
        {
            get
            {
                return FullClassName.FullName ?? string.Empty;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class RevitCommand : Attribute
    {
        public CommandManifest Manifest { get; }
        public RevitCommand(string addInName, string id, Type commandEntryPoint, string vendorId, string installationPath, string text = "", string description = "", string largeImagePath = "", string tooltipImagePath = "", string longDescription = "", string availabilityClassName = "", string langType = "")
        {
            _ = Guid.TryParse(id, out Guid parsedId); //because I can't pass a raw GUID inside an attribute.
            CommandManifest c = new(parsedId, commandEntryPoint, vendorId, installationPath);
            
            if (!String.IsNullOrEmpty(addInName)) c.AddInName = addInName;
            if (!String.IsNullOrEmpty(text)) c.Text = text;
            if (!String.IsNullOrEmpty(description)) c.Description = description;
            if (!String.IsNullOrEmpty(largeImagePath)) c.LargeImage = largeImagePath;
            if (!String.IsNullOrEmpty(tooltipImagePath)) c.TooltipImage = tooltipImagePath;
            if (!String.IsNullOrEmpty(longDescription)) c.LongDescription = longDescription;
            if (!String.IsNullOrEmpty(availabilityClassName)) c.AvailabilityClassName = availabilityClassName;
            if (!String.IsNullOrEmpty(langType)) c.LanguageType = langType;

            Manifest = c;

            Console.WriteLine(this.ToString());
        }
        public override string ToString()
        {
            return Manifest.ToString();
        }
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

}
