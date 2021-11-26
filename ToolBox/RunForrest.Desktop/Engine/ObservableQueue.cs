using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace RunForrest.Desktop
{
  // https://stackoverflow.com/a/4266692/8878639
  public class ObservableQueue<T>: INotifyCollectionChanged, IEnumerable<T>
  {
    private readonly Queue<T> queue;

    public int Count
    {
      get { return this.queue.Count; }
    }

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    public void Enqueue(T item)
    {
      this.queue.Enqueue(item);
      this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, queue.Count));
    }

    public T Dequeue()
    {
      var item = this.queue.Dequeue();
      this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, 0));
      return item;
    }

    public void Clear()
    {
      this.queue.Clear();
      this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public IEnumerator<T> GetEnumerator()
    {
      return queue.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public ObservableQueue()
    {
      this.queue = new Queue<T>();
    }
  }
}
