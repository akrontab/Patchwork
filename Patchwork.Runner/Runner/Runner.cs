using System.Diagnostics;
using Microsoft.Extensions.Options;
using Patchwork.Common;
using Patchwork.Runner.Observers;
using Patchwork.Tasks;

namespace Patchwork.Runner
{
    public class Runner : IDisposable
    {
        private readonly List<IObserver<RunnerStats>> _statsObservers = new();
        private readonly List<Task> _tasks = new();
        private CancellationTokenSource _cts = new();
        private int _tasksCompleted;
        private readonly Stopwatch _runTimer = new();
        private readonly RunnerOptions _runnerOptions;

        public Runner(IOptions<RunnerOptions> options)
        {
            _runnerOptions = options.Value;
        }

        public Runner()
        {
            _runnerOptions = new RunnerOptions
            {
                RunnerStatsPublishInterval = 1000,
                TaskCleanUpInterval = 2000
            };
        }

        public Task RunAsync(CancellationTokenSource cts)
        {
            _runTimer.Start();
            _cts = cts;

            void TimerCallback(object? state)
            {
                _tasksCompleted += _tasks.RemoveAll(t => t.IsCompleted);
            }
            var taskCleanupTimer = new Timer(TimerCallback, null, 0, _runnerOptions.TaskCleanUpInterval);

            void StatsPublishCallback(object? state)
            {
                var stats = new RunnerStats(_runTimer.Elapsed, _tasks.Count, _tasksCompleted);
                PublishRunnerStats(stats);
            }
            var statsPublishTimer = new Timer(StatsPublishCallback, null, 0, _runnerOptions.RunnerStatsPublishInterval);

            return Task.Factory.StartNew(() =>
            {
                while (!_cts.IsCancellationRequested) { }
            });
        }

        public void AddTask(Dictionary<string, string> taskParameters, BaseTask<TaskStats> taskToAdd)
        {
            AddTask(taskParameters, taskToAdd, new ConsoleObserver());
        }

        public void AddTask(Dictionary<string, string> taskParameters, BaseTask<TaskStats> taskToAdd, IObserver<TaskStats> observer)
        {
            taskToAdd.Subscribe(observer);
            _tasks.Add(taskToAdd.RunAsync(new Dictionary<string, string>(), _cts.Token));
        }

        public IDisposable SubscribeToStats(IObserver<RunnerStats> observer)
        {
            if (!_statsObservers.Contains(observer))
            {
                _statsObservers.Add(observer);
            }
            return new Unsubscriber<RunnerStats>(_statsObservers, observer);
        }

        private void PublishRunnerStats(RunnerStats stats)
        {
            foreach (var observer in _statsObservers)
            {
                observer.OnNext(stats);
            }
        }

        /// <summary>
        /// HACK - Needed a way to reset the cancelation token source at run time. Calling this before cancelling all tasks
        /// that used the original cts.Token will leave those tasks without a way to cancel them.
        /// </summary>
        public void RefreshCancellationTokenSource(CancellationTokenSource cts)
        {
            _cts = cts;
        }

        public void Dispose()
        {
            _runTimer.Stop();
            _tasks.Clear();
            _statsObservers.Clear();
        }
    }
}