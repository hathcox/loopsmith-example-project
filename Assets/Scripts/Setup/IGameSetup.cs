public interface IGameSetup
{
    int ExecutionOrder { get; }
    void Execute();
}
