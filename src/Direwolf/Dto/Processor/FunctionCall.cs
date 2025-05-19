namespace Direwolf.Dto.Processor;

public record FunctionCall<TInput, TOutput>(Func<TInput, WolfDto> Function, TInput Input)
{
    public WolfDto Result {get;} = Function(Input);
}