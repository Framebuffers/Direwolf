using Direwolf.Contracts;

namespace Direwolf.Python
{
    public class PythonInterop
    {

    }

    public interface IPythonWolf : IWolf
    {

    }

    public interface IPythonHowl : IHowl
    {
        
    }

    public readonly record struct PythonHowl(string Query)
    {

    }
}
