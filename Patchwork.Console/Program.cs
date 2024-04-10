using Patchwork.Tasks;

namespace Patchwork
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();

            var runner = new Runner.Runner();

            Task.Factory.StartNew(() => runner.Run(cts));

            while (true) 
            {
                var inputChar = Console.Read();

                if (inputChar.Equals('a'))
                {
                    runner.AddTask(new DemoTask());
                }
                if (inputChar.Equals('c'))
                {
                    cts.Cancel();
                    cts = new CancellationTokenSource();
                    runner.RefreshCancellationTokenSource(cts);
                }
            }
        }
    }
}