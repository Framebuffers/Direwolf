using System.Reflection;
using Direwolf.Revit.Howlers;

namespace Direwolf.Revit.UI.Libraries;

public static class References
{
    public static readonly string AssemblyLocation = Assembly.GetExecutingAssembly().Location;
    public static readonly string DirewolfRevitLocation = typeof(RevitLonewolf).Assembly.Location;
}