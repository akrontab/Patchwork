using Patchwork.Tasks;

namespace Patchwork.Runner.Observers
{
	public class TaskObserver : IObserver<TaskStats>
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
			throw new NotImplementedException();
		}
	}
}
