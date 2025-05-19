using Autodesk.Revit.DB;

using Direwolf.Dto.InternalDb.Enums;
using Direwolf.Dto.Parser;

using Transaction = Direwolf.Dto.InternalDb.Transaction;

namespace Direwolf.Contracts;

/// <summary>
///     <para>
///         An <see cref="IWolf" /> is the basic transactional object inside Direwolf. Any and all query, Internal or
///         External, is wrapped inside an <see cref="IWolf" />. It's separated into three (3) main parts:
///         Identification, Implementation and Instruction.
///         <list type="bullet">
///             <item>
///                 <description>
///                     A way to uniquely define the object, in a way similar to a Primary Key (PK).
///                 </description>
///             </item>
///             <item>
///                 <description>
///                     The query it will perform and on which <see cref="Realm" /> will it act upon.
///                 </description>
///             </item>
///             <item>
///                 <description>
///                     The operation itself.
///                 </description>
///             </item>
///         </list>
///     </para>
///     <para>
///         Direwolf will only accept and return <see cref="IWolf" /> objects. It uses the quirks of records, like their
///         partial immutability and shallow copies, to leave the implementation of its result at a later time.
///     </para>
///     <remarks>
///         <see cref="IWolf" />-derived objects are meant to be created one, and composed throughout the query process.
///         You shouldn't need to create new <see cref="IWolf" /> objects throughout a query life cycle, unless you need to
///         use two completely different queries for one operation.
///         This should be done outside Direwolf.
///     </remarks>
/// </summary>
public interface IWolf
{
    public Cuid            UniqueId {get; init;}
    public string          Name     {get; init;}
    public Realm           Realm    {get; init;}
    public BuiltInCategory Category {get; init;}
    public Transaction     Data     {get; set;}
}