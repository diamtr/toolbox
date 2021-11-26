using NUnit.Framework;
using RunForrest.Desktop;

namespace RunForrestPlugin.Tests
{
  [TestFixture]
  public class ObservableQueueTests
  {
    [Test]
    public void Create()
    {
      var queue = new ObservableQueue<string>();
      Assert.Zero(queue.Count);
    }

    [Test]
    public void EnqueueDequeue()
    {
      var queue = new ObservableQueue<string>();
      // Enqueue
      queue.Enqueue("First");
      Assert.AreEqual(1, queue.Count);
      queue.Enqueue("Second");
      Assert.AreEqual(2, queue.Count);
      queue.Enqueue("Third");
      Assert.AreEqual(3, queue.Count);
      // Dequeue
      Assert.AreEqual("First", queue.Dequeue());
      Assert.AreEqual(2, queue.Count);
      Assert.AreEqual("Second", queue.Dequeue());
      Assert.AreEqual(1, queue.Count);
      Assert.AreEqual("Third", queue.Dequeue());
      Assert.AreEqual(0, queue.Count);
      // Mixed
      queue.Enqueue("First");
      Assert.AreEqual(1, queue.Count);
      queue.Enqueue("Second");
      Assert.AreEqual(2, queue.Count);
      Assert.AreEqual("First", queue.Dequeue());
      Assert.AreEqual(1, queue.Count);
      queue.Enqueue("Third");
      Assert.AreEqual(2, queue.Count);
    }

    [Test]
    public void CollectionChangedHandled()
    {
      var addHandled = false;
      var removeHandled = false;
      var queue = new ObservableQueue<string>();
      queue.CollectionChanged += (sender, e) => {
        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
          addHandled = true;
        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
          removeHandled = true;
      };
      queue.Enqueue("Test item");
      Assert.IsTrue(addHandled);
      Assert.IsFalse(removeHandled);
      addHandled = false;
      removeHandled = false;
      queue.Dequeue();
      Assert.IsFalse(addHandled);
      Assert.IsTrue(removeHandled);
    }
  }
}
