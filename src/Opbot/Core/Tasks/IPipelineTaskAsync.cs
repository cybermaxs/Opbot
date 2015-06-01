
namespace Opbot.Core.Tasks
{
    interface IPipelineTask<TIn, TResult>
    {
        TResult Execute(TIn input);
    }
}
