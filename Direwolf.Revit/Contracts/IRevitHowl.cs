using Autodesk.Revit.DB;
using Direwolf.Contracts;
using Direwolf.Revit.Definitions.Primitives;

namespace Direwolf.Revit.Contracts;

public interface IRevitHowl : IHowl
{
    public Document? Document { get; set; }
    public new RevitWolfpack? ExecuteHunt();
}