using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Direwolf.Revit.UI.Commands;

/// <summary>
///     Abstracts some of the boilerplate needed to create a valid <see cref="IExternalCommand" /> to execute inside Revit.
/// </summary>
public abstract class DirewolfRevitCommand : IExternalCommand
{
    /// <summary>
    ///     <inheritdoc />
    /// </summary>
    /// <param name="commandData">
    ///     <inheritdoc />
    /// </param>
    /// <param name="message">
    ///     <inheritdoc />
    /// </param>
    /// <param name="elements">
    ///     <inheritdoc />
    /// </param>
    /// <returns>
    ///     <inheritdoc />
    /// </returns>
    public abstract Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements);
}