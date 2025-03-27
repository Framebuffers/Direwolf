namespace Direwolf.Revit.UI.Commands
{
    public abstract partial class DirewolfRevitCommand
    {
        public readonly record struct LocationInformation
        {
            public string ButtonName { get; init; }
            public string Descriptor { get; init; }
            public string AssemblyLocation { get; init; }
            public string ClassName { get; init; }

            public static LocationInformation Generate(DirewolfRevitCommand dw)
            {
                return new LocationInformation
                {
                    ButtonName = dw.GetType().Name,
                    Descriptor = dw.Descriptor ?? string.Empty,
                    AssemblyLocation = typeof(DirewolfRevitCommand).Assembly.Location,
                    ClassName = dw.GetType().FullName ?? string.Empty
                };
            }

        }
    }
}
