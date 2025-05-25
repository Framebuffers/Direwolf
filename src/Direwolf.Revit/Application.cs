using Nice3point.Revit.Toolkit.External;

using Direwolf.Revit.Commands;

namespace Direwolf.Revit;

/// <summary>
///     Application entry point
/// </summary>
[UsedImplicitly]
public class Application : ExternalApplication
{
    public override void OnStartup()
    {
        CreateRibbon();
    }

    private void CreateRibbon()
    {
        var panel = Application.CreatePanel("Commands",
                                            "Direwolf.Revit");

        panel.AddPushButton<StartupCommand>("Execute")
             .SetImage(
                 "/Direwolf.Revit;component/Resources/Icons/RibbonIcon16.png")
             .SetLargeImage(
                 "/Direwolf.Revit;component/Resources/Icons/RibbonIcon32.png");
    }
}