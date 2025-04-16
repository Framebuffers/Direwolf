using Direwolf.Contracts;
using Direwolf.Revit.Definitions.Primitives;

namespace Direwolf.Revit.Contracts;

public interface IRevitWolf : IWolf
{
    public new RevitWolfpack Result { get; set; }
    public new void Hunt();
}