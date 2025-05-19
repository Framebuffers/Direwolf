using Direwolf.Dto.InternalDb.Enums;
using Direwolf.Dto.Mapper;

namespace Direwolf.Dto.Parser;

public record WolfpackParameters(DocumentType Type, bool Async, int RevitVersion, ExecutionTrigger Trigger);