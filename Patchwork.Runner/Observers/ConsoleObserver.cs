using Patchwork.Tasks;

namespace Patchwork.Runner.Observers
{
    public class ConsoleObserver : IObserver<TaskStats> 
    {
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(TaskStats value)
        {
            foreach (var message in value.Messages)
            {
                Console.WriteLine(message);
            }
        }
    }
}
