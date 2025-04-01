namespace Direwolf.Definitions
{
    /// <summary>
    /// Data wrapper for information obtained from a query. It is an immutable record of any kind of object. Bare in mind that this object will be serialized as JSON, therefore, the type being input has to be serializable.
    /// </summary>
    /// <param name="Result">Serializable data</param>
    public readonly record struct Prey(object Result) { }
  }
