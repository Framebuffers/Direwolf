using Direwolf.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
