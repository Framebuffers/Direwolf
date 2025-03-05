using Autodesk.Revit.DB;

namespace Direwolf.Contracts
{
    public interface IRevitHowl : IHowl
    {
        public Document GetRevitDocument();
        public void SetRevitDocument(Document value);
    }
}
