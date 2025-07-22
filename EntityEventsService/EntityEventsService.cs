namespace EntityEvents;

/// <summary>
/// A default implementation of <see cref="IEntityEventsService"/> that manages subscriptions using a thread-safe dictionary.
/// </summary>
/// <remarks>
/// <para>
/// This implementation is thread-safe thanks to the use of the private <c>_lock</c> object
/// for all subscription operations. It does not use weak references, so subscribers
/// must remember to call <see cref="Unsubscribe{TEntity}"/> explicitly when they are no longer interested in events.
/// </para>
/// <para>
/// Implementation details:
/// <list type="bullet">
///   <item><description>Subscribes and unsubscribes are O(1) to locate or insert the list, but removing from the list is O(n).</description></item>
///   <item><description>Event publication is O(n) for the number of current subscribers for that entity type.</description></item>
///   <item><description>Order of event delivery is not guaranteed among multiple subscribers.</description></item>
///   <item><description>Exceptions in subscriber callbacks will propagate to the publisher upon calling <see cref="PublishAsync{TEntity}"/>.</description></item>
/// </list>
/// </para>
/// <para>
/// Typical registration in a DI container:
/// <code>
/// services.AddSingleton&lt;IEntityEventsService, EntityEventsService&gt;();
/// </code>
/// </para>
/// </remarks>
public class EntityEventsService : IEntityEventsService
{
    /// <summary>
    /// Inner class that holds the subscriber object and its callback.
    /// </summary>
    private class Subscription
    {
        /// <summary>
        /// The subscriber object (often 'this' from wherever <see cref="Subscribe{TEntity}"/> was called).
        /// </summary>
        public object Subscriber { get; init; }

        /// <summary>
        /// The callback function that should be invoked when an event matching the subscribed entity type is published.
        /// </summary>
        /// <remarks>
        /// We store it as <see cref="Func{EntityEventArgs, object, Task}"/> to avoid using reflection via <c>DynamicInvoke</c>.
        /// This gives us compile-time checking of method signatures and is more performant.
        /// </remarks>
        public Func<EntityEventArgs, object, Task> Callback { get; init; }
    }

    /// <summary>
    /// A thread-safe dictionary that maps each entity type (like typeof(Order)) 
    /// to a list of subscriptions interested in that entity type.
    /// </summary>
    private readonly Dictionary<Type, List<Subscription>> _subscriptions = new();

    /// <summary>
    /// A private lock object to ensure mutual exclusion on subscription-related operations.
    /// </summary>
    private readonly object _lock = new();

    /// <inheritdoc />
    public void Subscribe<TEntity>(object subscriber, Func<EntityEventArgs, object, Task> callback)
    {
        if (subscriber == null)
            throw new ArgumentNullException(nameof(subscriber));

        if (callback == null)
            throw new ArgumentNullException(nameof(callback));

        // Acquire a lock to ensure thread safety when modifying the subscription list.
        lock (_lock)
        {
            var key = typeof(TEntity);

            // Try to get the existing subscription list for this entity type.
            if (!_subscriptions.TryGetValue(key, out var subs))
            {
                subs = new List<Subscription>();
                _subscriptions[key] = subs;
            }

            // Add a new subscription record to that list.
            subs.Add(new Subscription
            {
                Subscriber = subscriber,
                Callback = callback
            });
        }
    }

    /// <inheritdoc />
    public void Unsubscribe<TEntity>(object subscriber)
    {
        // If subscriber is null, there's nothing to remove.
        if (subscriber == null)
            return;

        lock (_lock)
        {
            // Check if we have a list of subscribers for the given TEntity type.
            if (_subscriptions.TryGetValue(typeof(TEntity), out var subs))
            {
                // Remove all subscriptions that match this subscriber object.
                subs.RemoveAll(s => s.Subscriber == subscriber);
            }
        }
    }

    /// <inheritdoc />
    public async Task PublishAsync<TEntity>(EntityEventArgs eventData, object sender = null)
    {
        if (eventData.Equals(default(EntityEventArgs)))
            throw new ArgumentNullException(nameof(eventData),
                "EntityEventArgs cannot be the default struct (did you forget to initialize it?).");

        List<Subscription> subsCopy;

        // Lock to safely read the current subscriptions,
        // then copy them into a separate list for thread-safe iteration.
        lock (_lock)
        {
            if (!_subscriptions.TryGetValue(typeof(TEntity), out var subs))
            {
                // No subscriptions for this entity type, so nothing to do.
                return;
            }

            // Make a shallow copy to avoid issues if the subscriptions collection changes while we publish.
            subsCopy = new List<Subscription>(subs);
        }

        // Prepare all subscriber tasks, excluding the one whose Subscriber == sender
        // if the publisher doesn't want to notify itself.
        var tasks = subsCopy
            .Where(sub => sub.Subscriber != sender)
            .Select(sub => sub.Callback(eventData, sender))
            .ToList();

        // Execute all subscription callbacks in parallel, waiting until they're all done.
        // If any throw exceptions, Task.WhenAll will re-throw the first encountered exception.
        if (tasks.Count > 0)
        {
            await Task.WhenAll(tasks);
        }
    }
}