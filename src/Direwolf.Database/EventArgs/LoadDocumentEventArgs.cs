using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;

namespace Direwolf.Database.EventArgs;

public class LoadDocumentEventArgs : System.EventArgs
{
    public ControlledApplication Application { get; set; }
}