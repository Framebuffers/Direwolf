using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.EventHandlers;
using Direwolf.Revit.Definitions;
using Direwolf.Revit.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Howls
{
    public record class GetIntrospection : RevitHowl
    {
        public GetIntrospection(Document doc)
        {
            ArgumentNullException.ThrowIfNull(doc);
            SetRevitDocument(doc);
        }
        public override bool Execute()
        {
            try
            {



                return true;
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Exception thrown: {e.Message}"));
                return false;
            }
        }

        private Prey Hunt()
        {
            var elements = new FilteredElementCollector(GetRevitDocument()).WhereElementIsNotElementType();

            foreach (Element e in elements)
            {
                    
            }

        }

        private (DocumentIntrospection, ProjectInformationIntrospection, ProjectSiteIntrospection, UnitIntrospection) HuntDocument()
        {
            return (new DocumentIntrospection(GetRevitDocument()),
                new ProjectInformationIntrospection(GetRevitDocument()),
                new ProjectSiteIntrospection(GetRevitDocument()),
                new UnitIntrospection(GetRevitDocument()));
        }

        private KeyValuePair<ElementIntrospection, List<ParameterIntrospection>> HuntElement(Element e)
        {
            try
            {
                List<ParameterIntrospection> parameters = [];
                foreach (Parameter p in e.GetOrderedParameters())
                {
                    parameters.Add(new ParameterIntrospection(p));
                }
                return new KeyValuePair<ElementIntrospection, List<ParameterIntrospection>>(new ElementIntrospection(e), parameters);
            }
            catch
            {
                return new KeyValuePair<ElementIntrospection, List<ParameterIntrospection>>(new ElementIntrospection(), []);
            }
        }

    }
}
