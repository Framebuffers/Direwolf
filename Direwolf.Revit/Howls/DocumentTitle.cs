using System.Diagnostics;
using Direwolf.Revit.Definitions.Primitives;

namespace Direwolf.Revit.Howls;

public record DocumentTitle : RevitHowl
{
    
    public override RevitWolfpack? ExecuteHunt()
    {
        // test time check
        Stopwatch s = new();
        
        s.Start();
        var title = Document?.Title;
        s.Stop();

        // do a null check before returning
        return Document is not null
            ? RevitWolfpack.New("docTitle",
                Document,
                title,
                s.ElapsedMilliseconds,
                true)
            : null;
    }
}