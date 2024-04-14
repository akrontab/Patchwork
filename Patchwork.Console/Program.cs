using Patchwork.Console;
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

			Task.Factory.StartNew(() => runner.Run(cts));

			while (true)
			{
				var keyPress = System.Console.ReadKey();

				if (keyPress.KeyChar.Equals('a'))
				{
					runner.AddTask(new DemoTask());
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