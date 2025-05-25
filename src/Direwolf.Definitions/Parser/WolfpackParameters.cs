using Direwolf.Definitions.Internal.Enums;
using Direwolf.Definitions.Telemetry;

namespace Direwolf.Definitions.Parser;

public record WolfpackParameters(
    DocumentType     Type,
    bool             Async,
    int              RevitVersion,
    ExecutionTrigger Trigger);