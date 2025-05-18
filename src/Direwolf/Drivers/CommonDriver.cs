using Autodesk.Revit.DB;
using Direwolf.Contracts;
using Direwolf.Dto;
using Transaction = Direwolf.Dto.InternalDb.Transaction;
using Direwolf.Dto.InternalDb.Enums;
using Direwolf.Dto.Parser;

namespace Direwolf.Drivers;

/// <summary>
/// This contract enforces four operations for each driver:
///     - Create a new instance of the driver, returning a connection using the Create() method.
///         - 
///     - Read from the database to get a result, using the Read() method.
///     - Update the database, passing an instruction through the Update() method.
///     - Delete a record from the remote database detailed upon its instantiation, through the Delete() method.
///     - Destroy the remote data warehouse through the Destroy() method.
///     - Dispose the instance using the Dispose() method implemented through IDisposable.
///         - This is meant to be used for connections that use persistent connections, and connecting for one
///           query is not a good idea. It's designed to be used, for example, with Npgsql 
/// </summary>
public interface IDriver : IDisposable
{
    public abstract IDriver Create(string[] args);
    public abstract WolfDto Read(Wolfpack instructions, out bool result);
    public abstract void Update(Query instruction, out bool result);
    public abstract void Delete(Query instruction, out bool result);
    public abstract void Destroy(out bool result);
}

/// <summary>
/// A Driver is in charge of managing queries in, and getting results out of Direwolf.
/// It must implement:
///         - A way to 
/// </summary>
public abstract class CommonDriver : IDriver
{

    public abstract IDriver Create(string[] args);
    public abstract WolfDto Read(Wolfpack instructions, out bool result);
    public abstract void Update(Query instruction, out bool result);
    public abstract void Delete(Query instruction, out bool result);
    public abstract void Destroy(out bool result);
    public abstract void Dispose();
}