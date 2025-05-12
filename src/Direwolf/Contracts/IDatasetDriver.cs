namespace Direwolf.Contracts;

/// <summary>
/// Implements any operations needed to manipulate a Dataset, regardless of its type: a file, a database, etc.
/// </summary>
public interface IDatasetDriver
{
    //TODO: add Dataset type
    public void Initialize();
    public void Create();
    public void Read();
    public void Update();
    public void Delete();
}