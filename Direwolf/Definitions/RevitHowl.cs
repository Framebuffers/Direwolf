using Autodesk.Revit.DB;
using Direwolf.Contracts;
using System.Text.Json.Serialization;

namespace Direwolf.Definitions
{
    public record class RevitHowl : IRevitHowl
    {
        private Guid RequestIdentification { get; init; } = Guid.NewGuid();
        [JsonIgnore] public IWolf? Callback { get; set; }

        public void SendCatchToCallback(Catch c)
        {
            var d = new Dictionary<string, object>()
            {
                ["Timestamp"] = DateTime.Now,
                ["GUID"] = RequestIdentification.ToString(),
                [GetType().Name] = c
            };
            Callback?.Catches.Push(new Catch(d));
        }

        public virtual bool Execute()
        {
            try
            {
                // A catch won't handle data retrieval on it's own, as it is just meant to be a dumb container.
                // Any data retrieval operation should be done here.
                // If, for example, the result returns a void or a bool itself (without having to get data itself)
                // Just return true. No need to forge a blank Catch. The Wolf *should* expect this result.
                if (Callback is not null)
                {
                    return true; // hunt successful!
                }
                else
                {
                    return false; // howled into the air, but no wolf to hear back...
                }
            }
            catch
            {
                return false; // failed to hunt.
            }
        }

        public override string ToString() // the default implementation will recursively serialize everything up the tree. that is: not good.
        {
            var d = new Dictionary<string, object>()
            {
                { "Callback", Callback?.GetType().Name ?? "unknown" },
                { "Timestamp", DateTime.Now.ToString() }
            };
            return new Catch(d).ToString();
        }

        private Document? _rvtDoc; // it should never be null though, unless *directly* done so.
        public Document GetRevitDocument()
        {
            ArgumentNullException.ThrowIfNull(_rvtDoc);
            return _rvtDoc;
        }
        public void SetRevitDocument(Document value)
        {
            ArgumentNullException.ThrowIfNull(value);
            _rvtDoc = value;
        }
    }
}
