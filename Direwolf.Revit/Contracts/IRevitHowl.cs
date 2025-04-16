using Autodesk.Revit.DB;
using Direwolf.Contracts;
using Direwolf.Revit.Definitions.Primitives;

namespace Direwolf.Revit.Contracts;

public interface IRevitHowl : IHowl
{
    public void SetRevitDocument(Document doc);
    public Document? GetRevitDocument();
    public new RevitWolfpack? ExecuteHunt();
}