using Autodesk.Revit.DB;
using Direwolf.Contracts.Dynamics;

namespace Direwolf.Revit.Contracts.Dynamics
{
    public interface IDynamicRevitHowl : IDynamicHowl
    {
        public Document GetRevitDocument();
        public void SetRevitDocument(Document value);
    }
}
