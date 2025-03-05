using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Examples.Howls;

namespace Direwolf.Examples.Howlers
{
    public record class RevitElementDispatch : Howler
    {
        public RevitElementDispatch(Document revitDoc)
        {
            GetElementIdByFamily h = new(revitDoc);
            CreateWolf(new Wolf(), h);
        }
    }
}
