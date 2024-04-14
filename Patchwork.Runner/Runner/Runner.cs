using System.Diagnostics;
using Microsoft.Extensions.Options;
using Patchwork.Common;
using Patchwork.Runner.Observers;
using Patchwork.Tasks;

namespace Patchwork.Runner
{
	public class Runner
	{
		private List<IObserver<RunnerStats>> _statsObservers = new List<IObserver<RunnerStats>>();
		private List<Task> _tasks = new List<Task>();
		private CancellationTokenSource _cts = new CancellationTokenSource();
		private int _tasksCompleted = 0;
		private Stopwatch _runTimer = new Stopwatch();
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
				TaskCleanUpInterval	= 2000
			};
		}

		public void Run(CancellationTokenSource cts)
		{
			// Time used to report run statics
			_runTimer.Start();

			_cts = cts;

			// TODO - Add Options Class to control task cleanup time and Runner Stats publishing interval
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

			while (true && !_cts.IsCancellationRequested) { }
		}
		public void AddTask(BaseTask<TaskStats> taskToAdd)
		{
			taskToAdd.Subscribe(new ConsoleObserver());

			_tasks.Add(taskToAdd.Run(_cts.Token));
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
			_statsObservers.ForEach(o => o.OnNext(stats));
		}

		/// <summary>
		/// HACK - Needed a way to reset the cancelation token source at run time. Calling this before cancelling all tasks
		/// that used the original cts.Token will leave those tasks without a way to cancel them.
		/// </summary>
		public void RefreshCancellationTokenSource(CancellationTokenSource cts)
		{
			_cts = cts;
		}
	}
}