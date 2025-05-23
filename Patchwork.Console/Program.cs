﻿using Patchwork.Console;
using Patchwork.Tasks;

namespace Patchwork
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var cts = new CancellationTokenSource();

			var runner = new Runner.Runner();

			runner.SubscribeToStats(new RunnerObserver());

			Task.Factory.StartNew(() => runner.RunAsync(cts));

			while (true)
			{
				var keyPress = System.Console.ReadKey(true);

				if (keyPress.KeyChar.Equals('a'))
				{
					runner.AddTask(new Dictionary<string, string>(), new DemoTask());
				}
				if (keyPress.KeyChar.Equals('c'))
				{
					cts.Cancel();
					cts = new CancellationTokenSource();
					runner.RefreshCancellationTokenSource(cts);
				}
			}
		}
	}
}