using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Direwolf.ManifestGenerator;
using static Autodesk.Internal.Windows.SwfMediaPlayer;

namespace Lab1PlaceGroup
{
    [Transaction(TransactionMode.Manual)]
    [ManifestAttributes(
        """C:\ProgramData\Autodesk\Revit\Addins\2025""",
        "Framebuffer",
        "github.com\\/Framebuffers",
        "Direwolf",
        "Data Reaper for Revit",
        "Architecture",
        "unknown")]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get application and document objects  
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;

            //Define a reference Object to accept the pick result  
            Reference pickedref;

            //Pick a group  
            Selection sel = uiapp.ActiveUIDocument.Selection;
            pickedref = sel.PickObject(ObjectType.Element, "Please select a group");

            Element elem = doc.GetElement(pickedref);
            Group? group = elem as Group;

            //Pick point  
            XYZ point = sel.PickPoint("Please pick a point to place group");

            //Place the group  
            Transaction trans = new Transaction(doc);
            trans.Start("Lab");
            doc.Create.PlaceGroup(point, group?.GroupType);
            trans.Commit();

            return Result.Succeeded;
        }
    }
}