using Autodesk.Revit.UI;

namespace Direwolf.Revit.UI
{
    public static class Buttons
    {
        public static PushButton? AddGetModelHealthButton(RibbonPanel? rp)
        {
            PushButtonData b = new("modelHealth", "Get Model Health", References.DirewolfRevitLocation, "Direwolf.Revit.UI.Commands.GetModelHealth");
            return rp?.AddItem(b) as PushButton;
        }

        //public static PushButton? AddPushToDBButton(RibbonPanel? rp)
        //{
        //    PushButtonData b = new("pushToDB", "Push Wolfpack to Database", References.DirewolfRevitLocation, "Direwolf.Revit.UI.Commands.PushToDB");
        //    return rp?.AddItem(b) as PushButton;
        //}


        //public static PushButton? AddGetAnnotativeElementsButton(RibbonPanel? rp)
        //{
        // PushButtonData b = new("modelHealth", "Get Annotative Elements", References.DirewolfRevitLocation, "Direwolf.Revit.Benchmarking.CheckHelpers");
    
        //}


    }
}
