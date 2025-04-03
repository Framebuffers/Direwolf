using Autodesk.Revit.DB;
using Direwolf.Contracts;

namespace Direwolf.Revit.Contracts;

/// <summary>
///     Adds the requirement of a valid Revit document to a regular <see cref="IHowl" />
/// </summary>
public interface IRevitHowl : IHowl
{
    public Document GetRevitDocument();
    public void SetRevitDocument(Document value);
}