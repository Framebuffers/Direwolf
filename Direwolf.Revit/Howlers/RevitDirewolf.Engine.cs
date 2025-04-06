using Autodesk.Revit.DB;
using Direwolf.Revit.Definitions;
namespace Direwolf.Revit.Howlers;


public partial class RevitDirewolf
{
    /// <summary>
    /// This is partially based on <see href="https://github.com/lookup-foundation/LookupEngine"> "lookup-foundation/LookupEngine </see>
    /// way of disassembling .NET elements via Reflection. However, this is a more specialised way of doing so:
    ///
    /// <list type="number">
    ///     <item>
    ///         <term>
    ///             Identification:
    ///         </term>
    ///         <description>
    ///             The item is checked against the Document for validity.
    ///             Direwolf works directly with the <see cref="Autodesk.Revit.DB.Element.UniqueId"/>
    ///             of each Element: it skips the <see cref="Autodesk.Revit.DB.ElementId"/> entirely, as it can be
    ///             obtained through the Document using <see cref="Autodesk.Revit.DB.Document.GetElement(String)"/>.
    ///
    ///             Also, the <see cref="Autodesk.Revit.DB.ElementType"/> is retrieved, and creates a unique
    ///             <see cref="ElementEntity"/>. This is a very fast identification tag with all the common parameters
    ///             and data Direwolf queries from each Element.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>
    ///             Inspection:
    ///         </term>
    ///         <description>
    ///             The <see cref="ElementEntity"/> is composed with a 
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>
    ///             Extraction:
    ///         </term>
    ///         <description>
    ///             The item is checked against the Document for validity.
    ///         </description>
    ///     </item>   /// 
    /// </list>
    /// </summary>
    /// <param name="element"></param>
    /// <param name="???"></param>
    /// <returns></returns>
    public static object ReapElement(object? element)
    {   
    }
}