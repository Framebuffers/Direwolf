using System.Reflection;

namespace Direwolf.Revit.UI.Libraries;

public static class References
{
    public static readonly string AssemblyLocation = Assembly.GetExecutingAssembly().Location;
    public static readonly string DirewolfRevitLocation = typeof(RevitDirewolf).Assembly.Location;
}