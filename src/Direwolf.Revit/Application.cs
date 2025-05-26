using Nice3point.Revit.Toolkit.External;

using Direwolf.Revit.Commands;

namespace Direwolf.Revit;

/// <summary>
///     Application entry point
/// </summary>
[UsedImplicitly]
public class Application : ExternalApplication
{
    public static Direwolf? Direwolf;
    public override void OnStartup()
    {
        CreateRibbon();
        Application.ControlledApplication.DocumentOpened += (sender, args) =>
        {
            Direwolf = new Direwolf(args.Document);
        };
        
    }

    private void CreateRibbon()
    {
        var panel = Application.CreatePanel("Commands",
                                            "Direwolf");

        panel.AddPushButton<PopulateDatabase>("Execute")
             .SetImage(
                 "/Direwolf.Revit;component/Resources/Icons/RibbonIcon16.png")
             .SetLargeImage(
                 "/Direwolf.Revit;component/Resources/Icons/RibbonIcon32.png");
    }
}