using Direwolf.Contracts;
using Direwolf.Revit.Definitions.Primitives;

namespace Direwolf.Revit.Contracts;

public interface IRevitWolfpack : IWolfpack
{
    public RevitDocumentId RevitDocument { get; init; }
}