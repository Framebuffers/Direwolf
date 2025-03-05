using Autodesk.Revit.DB;

namespace Direwolf.Contracts.Dynamics
{
    public interface IDynamicRevitHowl : IDynamicHowl
    {
        public Document GetRevitDocument();
        public void SetRevitDocument(Document value);
    }
}
