using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Revit.Extensions;
using Direwolf.Revit.Howls;
using Direwolf.Revit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Introspection
{
    /// <summary>
    /// Given an <see cref="Element"/>, get the family it belongs to, and return all the parameters for each instance.
    /// Check for overridden parameters.
    /// </summary>
    public record class ElementIntrospection : RevitHowl
    {
        public ElementIntrospection(Document doc, ElementSet e)
        {
            SetRevitDocument(doc);
            _e = e;
        }

        private readonly ElementSet? _e;
        public override bool Execute()
        {
            try
            {
                if (_e is not null)
                {
               
                    foreach (Element sibling in from element in _e as IEnumerable<Element>
                                            let fi = element as FamilyInstance
                                            where fi?.Symbol.Family is not null
                                            let f = fi.Symbol.Family
                                            let instances = f.GetFamilySymbolIds()
                                            from sibling in
                                                from instance in instances
                                                let sibling = GetRevitDocument().GetElement(instance)
                                                where sibling is not null
                                                select sibling
                                            select sibling)
                    {
                        try
                        {
                            SendCatchToCallback(new Prey(sibling._GetElementInformation(GetRevitDocument())));
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
