using Direwolf.Definitions.Internal.Enums;
using Direwolf.Definitions.Telemetry;

namespace Direwolf.Definitions.Parser;

// Unimplemented feature as of 2025-05-29
public record WolfpackParameters(DocumentType Type, bool Async, int RevitVersion, ExecutionTrigger Trigger);