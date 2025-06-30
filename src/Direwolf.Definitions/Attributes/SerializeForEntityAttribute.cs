namespace Direwolf.Definitions.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class SerializeForEntityAttribute : Attribute
{
    public string[] Contexts { get; }
    public SerializeForEntityAttribute(params string[]? contexts) => Contexts = contexts ??= [];
}