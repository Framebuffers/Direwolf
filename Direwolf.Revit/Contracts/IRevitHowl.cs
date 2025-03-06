using Autodesk.Revit.DB;
using Direwolf.Contracts;

namespace Direwolf.Revit.Contracts
{
    public interface IRevitHowl : IHowl
    {
        public Document GetRevitDocument();
        public void SetRevitDocument(Document value);
    }
}
