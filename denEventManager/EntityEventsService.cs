using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace denEventManager
{

    //add, delete, update
    public enum EntityActionType
    {
        Add,
        Delete,
        Update
    }

    public class EventArguments(List<int> entityIds)
    {
        public List<int> EntityIds { get; set; } = entityIds;
        EntityActionType EntityActionType { get; set; }
    }


    /// <summary>
    /// A concrete implementation of <see cref="IEntityEventsService"/> that provides
    /// a simple publish-subscribe mechanism (Event Aggregator / Event Bus) with asynchronous callbacks.
    /// </summary>
    /// <remarks>
    /// Use this class via dependency injection. For example:
    /// <code>
    /// services.AddSingleton&lt;INotificationCenter, NotificationCenter&gt;();
    /// </code>
    /// Then, in your classes, you can receive an <see cref="IEntityEventsService"/> in the constructor,
    /// call <see cref="Subscribe{T}(object, Func{EventArguments, object, Task})"/>, 
    /// <see cref="Unsubscribe{T}(object)"/>, and <see cref="PublishAsync{T}(T, object)"/>.
    /// </remarks>
    public class EntityEventsService : IEntityEventsService
    {
        /// <summary>
        /// Internal subscription record. Holds the subscriber object and its callback delegate.
        /// </summary>
        private class Subscription
        {
            public object Subscriber { get; init; }
            public Delegate Callback { get; init; }
        }

        // We store subscriptions in a dictionary where:
        //  Key:   The event type (e.g. typeof(MyEvent), typeof(string), etc.).
        //  Value: A list of 'Subscription' objects, each representing a subscriber for that event type.
        private readonly Dictionary<Type, List<Subscription>> _subscriptions = new();

        // We use a lock to ensure thread-safety (atomic access)
        // when modifying or enumerating subscriptions.
        private readonly object _lock = new();

        /// <inheritdoc/>
        public void Subscribe<T>(object subscriber, Func<EventArguments, object, Task> callback)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException(nameof(subscriber), "Subscriber cannot be null.");
            }
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback), "Callback cannot be null.");
            }

            lock (_lock)
            {
                var eventType = typeof(T);
                if (!_subscriptions.ContainsKey(eventType))
                {
                    _subscriptions[eventType] = [];
                }

                // Add a new subscription for this eventType.
                _subscriptions[eventType].Add(new Subscription
                {
                    Subscriber = subscriber,
                    Callback = callback
                });
            }
        }

        /// <inheritdoc/>
        public void Unsubscribe<T>(object subscriber)
        {
            if (subscriber is null)
            {
                // If subscriber is null, there's nothing to do.
                return;
            }

            lock (_lock)
            {
                var eventType = typeof(T);
                if (_subscriptions.TryGetValue(eventType, out List<Subscription> value))
                {
                    value.RemoveAll(s => s.Subscriber == subscriber);
                }
            }
        }

        /// <inheritdoc/>
        public async Task PublishAsync<T>(EventArguments eventData, object sender = null)
        {
            List<Subscription> subsCopy;

            lock (_lock)
            {
                var eventType = typeof(T);
                if (!_subscriptions.ContainsKey(eventType))
                {
                    // No subscribers for this event type, so we can return immediately.
                    return;
                }

                // Make a copy of the subscription list to avoid issues if 
                // subscribers modify the collection (subscribe/unsubscribe) during notification.
                subsCopy = _subscriptions[eventType].ToList();
            }

            // We'll hold the tasks here to await them all.
            var tasks = new List<Task>();

            foreach (var sub in subsCopy)
            {
                // Optional: if you want to avoid notifying the same object that published the event
                // (to prevent infinite loops), you could skip if (sub.Subscriber == sender).
                // For example:
                // if (sub.Subscriber == sender) 
                // {
                //     continue;
                // }

                // We expect Callback to be of type Func<T, object, Task>.
                if (sub.Callback is Func<EventArguments, object, Task> func)
                {
                    // Launch the subscriber callback (async).
                    tasks.Add(func(eventData, sender));
                }
            }

            // Wait for all subscriber tasks to complete (concurrently).
            // If any subscriber throws an exception, PublishAsync will propagate that exception
            // (or an AggregateException if multiple fail).
            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks);
            }
        }
    }
}
