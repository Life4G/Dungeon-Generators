using System;
using System.Collections.Generic;
using System.Linq;

public class PriorityQueue<TElement, TPriority>
{
    private readonly SortedDictionary<TPriority, Queue<TElement>> dictionary = new SortedDictionary<TPriority, Queue<TElement>>();

    public void Enqueue(TElement element, TPriority priority)
    {
        if (!dictionary.TryGetValue(priority, out var queue))
        {
            queue = new Queue<TElement>();
            dictionary[priority] = queue;
        }
        queue.Enqueue(element);
    }

    public TElement Dequeue()
    {
        if (dictionary.Count == 0)
        {
            throw new InvalidOperationException("The queue is empty.");
        }
        var pair = dictionary.First();
        var element = pair.Value.Dequeue();
        if (pair.Value.Count == 0)
        {
            dictionary.Remove(pair.Key);
        }
        return element;
    }

    public int Count => dictionary.Sum(pair => pair.Value.Count);

    public IEnumerable<(TElement Element, TPriority Priority)> UnorderedItems =>
        dictionary.SelectMany(pair => pair.Value.Select(element => (element, pair.Key)));
}

