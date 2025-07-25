using System.Net;
using System.Text;

namespace Direwolf;

/// <summary>
/// Direwolf's Server. Handles any external communications between Direwolf and any third-party program.
/// <list type="bullet">
///     <item>
///         <term>Connection with an external PostgreSQL database.</term>
///     </item>
/// </list>
/// </summary>
public class Hunter
{
    private static readonly object Lock = new();
    private static Direwolf _direwolf;
    private static Hunter? _instance;
    private static 

    private Hunter(Direwolf dw) { _direwolf = dw; }

    public static Hunter GetInstance(Direwolf dw)
    {
        if (_instance is not null) return _instance;
        lock(Lock)
        {
            if (_instance is not null) return _instance;
            _instance = new Hunter(dw);
            return _instance;
        }
    }

    /// <summary>
    /// HTTP server to communicate with Direwolf from the outside.
    /// 
    /// <remarks>Direwolf uses port 6621.</remarks>
    /// </summary>
    public void StartListener()
    {
        using var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:6621/");
        listener.Start();

        Console.WriteLine("Listening on port 8001...");

        while (true)
        {
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest req = context.Request;

            Console.WriteLine($"Received request for {req.Url}");

            using HttpListenerResponse resp = context.Response;
            resp.Headers.Set("Content-Type", "text/plain");

            string data = "Hello there!";
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            resp.ContentLength64 = buffer.Length;

            using Stream ros = resp.OutputStream;
            ros.Write(buffer, 0, buffer.Length);
        }
    }

    private void NotFound()
    {
        using HttpListenerResponse resp = ctx.Response;
        resp.Headers.Set("Content-Type", "text/plain");

        using Stream ros = resp.OutputStream;

        ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
        string err = "404 - not found";

        byte[] ebuf = Encoding.UTF8.GetBytes(err);
        resp.ContentLength64 = ebuf.Length;

        ros.Write(ebuf, 0, ebuf.Length); 
    }
    
    
}