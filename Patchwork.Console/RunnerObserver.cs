using Patchwork.Runner;

namespace Patchwork.Console
{
    internal class RunnerObserver : IObserver<RunnerStats>
    {
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(RunnerStats value)
        {
            System.Console.WriteLine(value);
        }
    }
}
