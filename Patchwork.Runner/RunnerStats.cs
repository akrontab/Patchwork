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

        public TimeSpan RunTime { get; private set; }
        public int TasksRunningCount { get; private set; }
        public int TasksCompletedCount { get; private set; }

    }
}
