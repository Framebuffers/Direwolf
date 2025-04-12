using Direwolf.Contracts;
using Direwolf.Revit.Definitions.Primitives;

namespace Direwolf.Revit.Contracts;

public interface IRevitWolf : IWolf
{
    public RevitWolfpack Result { get; set; }
    public void Hunt();
}