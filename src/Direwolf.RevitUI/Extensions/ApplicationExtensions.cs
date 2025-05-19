using System.Reflection;

using Autodesk.Revit.UI;

namespace Direwolf.RevitUI.Extensions;

public static class ApplicationExtensions
{
    /// <summary>
    /// Get <see cref="Autodesk.Revit.UI.UIApplication"/> using the <paramref name="application"/>
    /// Taken from https://dev.to/chuongmep/use-await-async-revit-api-3nk4
    /// </summary>
    /// <param name="application">Revit UIApplication</param>
    public static UIApplication? GetUIApplication(this UIControlledApplication application)
    {
        var type = typeof(UIControlledApplication);

        var property = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                           .FirstOrDefault(e => e.FieldType == typeof(UIApplication));

        return property?.GetValue(application) as UIApplication;
    }
}