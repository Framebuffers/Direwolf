using Direwolf.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf
{
    public abstract record class Wolf
    {
        public List<Howl> Howls { get; set; } = [];
        public List<Catch> Catches { get; set; } = [];
        public abstract void Execute();

    }
}
