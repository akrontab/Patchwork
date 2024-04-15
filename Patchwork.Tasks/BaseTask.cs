using Patchwork.Common;

namespace Patchwork.Tasks
{
    public abstract class BaseTask<T> : IObservable<T>
    {
        private List<IObserver<T>> _observers = new List<IObserver<T>>();

		public abstract Task RunAsync(Dictionary<string,string> parameters, CancellationToken cancellationToken);

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            return new Unsubscriber<T>(_observers, observer);
        }

        protected void NotifyObservers(T obj)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(obj);
            }
        }
    }
}
