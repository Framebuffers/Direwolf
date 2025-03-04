using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Examples.Howls;

namespace Direwolf.Examples.Howlers
{
    public record class RevitDocumentDispatch : Howler
    {
        public RevitDocumentDispatch(Document revitDoc)
        {
            GetElementIdByFamily h = new(revitDoc);
            CreateWolf(new Wolf(), h);
        }
    }
}
