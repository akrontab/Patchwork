namespace Patchwork.Tasks
{
    public class DemoTask : BaseTask<TaskStats>
    {
        public override Task RunAsync(Dictionary<string,string> parameters, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                var threadID = Environment.CurrentManagedThreadId;

                var stats = new TaskStats();
                stats.Messages.Add($"{nameof(DemoTask)} {threadID} started...");
                NotifyObservers(stats);

                while (true && !cancellationToken.IsCancellationRequested)
                {
                    stats = new TaskStats();
                    stats.Messages.Add($"{nameof(DemoTask)} {threadID} running...");
                    NotifyObservers(stats);

                    Thread.Sleep(1500);
                }
                
                stats = new TaskStats();
                stats.Messages.Add($"{nameof(DemoTask)} {threadID} stopping...");
                NotifyObservers(stats);
            });
        }
    }
}