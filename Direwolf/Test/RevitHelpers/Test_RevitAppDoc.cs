using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using static Direwolf.Helpers;

namespace Direwolf.Test.RevitHelpers;

[Transaction(TransactionMode.Manual)]
public class Test_RevitAppDoc : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        static string test(Document doc)
        {
            using StringWriter s = new();
            //read
            if (doc.IsValidObject) s.WriteLine("Document read: OK");
            else s.WriteLine("Document read: FAIL");

            // write
            try
            {
                Transaction t = new(doc, "Set new units");
                t.Start();
                try
                {
                    doc.SetUnits(new Units(UnitSystem.Imperial));
                    t.Commit();
                    t.Dispose();
                }
                catch
                {
                    t.RollBack();
                }
                s.WriteLine("Document write: PASS");
            }
            catch (Exception e)
            {
                s.WriteLine("Document write: FAIL: " + e.Message);
            }
            return s.ToString();
        }

        try
        {
            Helpers.GenerateNewWindow("Command succeded!", test(RevitAppDoc.GetDocument(commandData)));
            return Result.Succeeded;
        }
        catch
        {
            Helpers.GenerateNewWindow("Command failed!", "Cannot access the Document using this struct.");
            return Result.Failed;
        }
    }
}
