namespace CubeCollector.Setup
{
    /// <summary>
    /// Interface for setup classes that participate in the F5 rebuild sequence.
    /// Implement this interface in any class that should generate assets or configure the scene.
    /// </summary>
    public interface IGameSetup
    {
        /// <summary>
        /// Determines the order in which setup classes execute. Lower values run first.
        /// </summary>
        int ExecutionOrder { get; }

        /// <summary>
        /// Executes the setup logic (e.g., creating prefabs, materials, scene objects).
        /// </summary>
        void Execute();
    }
}
