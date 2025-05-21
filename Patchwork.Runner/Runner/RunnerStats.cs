using Patchwork.Tasks;

namespace Patchwork.Runner
{
    public class RunnerStats
    {
        public RunnerStats(TimeSpan runTime, int taskRunningCount, int taskCompleteCount)
        {
            RunTime = runTime;
            TasksRunningCount = taskRunningCount;
            TasksCompletedCount = taskCompleteCount;
        }

        public TimeSpan RunTime { get; }
        public int TasksRunningCount { get; }
        public int TasksCompletedCount { get; }

        public override string ToString() => $"RunTime: {RunTime.TotalSeconds} seconds; TasksRunning: {TasksRunningCount}; TasksCompleted: {TasksCompletedCount}";
    }
}
