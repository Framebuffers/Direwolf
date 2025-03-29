using Autodesk.Revit.UI;
using Direwolf.Revit.UI.Commands;
using static Direwolf.Revit.UI.Commands.DirewolfRevitCommand;

namespace Direwolf.Revit.UI
{
    public static class Buttons
    {
        public static PushButton? CreateButton(LocationInformation l, RibbonPanel? rp)
        {
            PushButtonData pb = new(l.ButtonName, l.Descriptor, l.AssemblyLocation, l.ClassName);
            return rp?.AddItem(pb) as PushButton;
        }

        //public static PushButton? AddGetModelHealthButton(RibbonPanel? rp)
        //{
        //    var b = GetModelHealth.GetButtonData();
        //    PushButtonData p = 
        //        new(b.ButtonName, b.Descriptor, b.AssemblyLocation, b.ClassName);
        //    return rp?.AddItem(p) as PushButton;
        //}

        //public static PushButton? AddPushToDBButton(RibbonPanel? rp)
        //{
        //    PushButtonData b = new("pushToDB", "Push Wolfpack to Database", References.DirewolfRevitLocation, "Direwolf.Revit.UI.Commands.PushToDB");
        //    return rp?.AddItem(b) as PushButton;
        //}


        //public static PushButton? AddGetAnnotativeElementsButton(RibbonPanel? rp)
        //{
        // PushButtonData b = new("modelHealth", "Get Annotative Elements", References.DirewolfRevitLocation, "Direwolf.Revit.Benchmarking.DocumentExtensions");
    
        //}

    }
}
