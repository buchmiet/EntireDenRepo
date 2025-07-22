namespace EntityEvents;

/// <summary>
/// Defines a contract for tracking and notifying about changes in entity states within a system.
/// This is a generic pub-sub (publish-subscribe) mechanism specialized for database entities.
/// </summary>
/// <remarks>
/// <para>
/// Typical usage includes:
/// <list type="bullet">
///   <item>Cache invalidation when certain entities change.</item>
///   <item>Cross-component synchronization (ensuring different parts of the system see the same updates).</item>
///   <item>Audit logging for entity modifications.</item>
///   <item>Triggering real-time UI updates based on server-side data changes.</item>
/// </list>
/// </para>
/// <para>
/// Subscribers register via <see cref="Subscribe{TEntity}"/> and receive notifications via a callback when
/// <see cref="PublishAsync{TEntity}"/> is called by a publisher.
/// </para>
/// </remarks>
public interface IEntityEventsService
{
    /// <summary>
    /// Subscribes to lifecycle events for a specific entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to monitor (e.g., Order, Customer, etc.).</typeparam>
    /// <param name="subscriber">
    /// The subscribing component (usually 'this' when you call it).
    /// This is used to identify the subscriber and to optionally skip self-notifications.
    /// </param>
    /// <param name="callback">
    /// The asynchronous handler that will be called when an event is published for <typeparamref name="TEntity"/>.
    /// It receives:
    /// <list type="number">
    ///   <item>
    ///     <see cref="EntityEventArgs"/>, containing IDs of affected entities and the <see cref="EntityActionType"/>.
    ///   </item>
    ///   <item>
    ///     The <c>sender</c> object that published the event (if any).
    ///   </item>
    /// </list>
    /// </param>
    /// <example>
    /// <code>
    /// _entityEvents.Subscribe&lt;Order&gt;(this, async (args, sender) =>
    /// {
    ///     // Ignore events originating from myself:
    ///     if (sender == this) return;
    ///     
    ///     await RefreshOrderCache(args.EntityIds);
    /// });
    /// </code>
    /// </example>
    void Subscribe<TEntity>(object subscriber, Func<EntityEventArgs, object, Task> callback);

    /// <summary>
    /// Removes all subscriptions for a specific entity type from a given subscriber.
    /// </summary>
    /// <typeparam name="TEntity">The entity type whose subscriptions should be removed.</typeparam>
    /// <param name="subscriber">
    /// The subscriber instance that previously called <see cref="Subscribe{TEntity}"/>.
    /// </param>
    /// <remarks>
    /// This method unsubscribes *all* callbacks that were registered by the given subscriber
    /// for <typeparamref name="TEntity"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Used when disposing or if the subscriber no longer cares about event notifications.
    /// _entityEvents.Unsubscribe&lt;Order&gt;(this);
    /// </code>
    /// </example>
    void Unsubscribe<TEntity>(object subscriber);

    /// <summary>
    /// Notifies all subscribers that some entities of type <typeparamref name="TEntity"/> have changed.
    /// </summary>
    /// <typeparam name="TEntity">The entity type that was changed.</typeparam>
    /// <param name="eventData">
    /// Information about the change, including which IDs are affected and the type of action.
    /// </param>
    /// <param name="sender">
    /// The publisher (optional). You can use this to skip self-notifications if desired.
    /// For example, if the publishing code is in a specific repository or service, it might pass 'this' as the sender.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> that completes once all relevant subscriber callbacks have finished processing.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="eventData"/> is null.
    /// </exception>
    /// <remarks>
    /// <para>
    /// Each registered subscriber (except the one whose object reference matches <c>sender</c>) will
    /// receive the <paramref name="eventData"/> in their callback.
    /// </para>
    /// <para>
    /// Any exceptions thrown in subscriber callbacks will be aggregated
    /// and re-thrown by <see cref="System.Threading.Tasks.Task.WhenAll(System.Collections.Generic.IEnumerable{System.Threading.Tasks.Task})"/>.
    /// This means if one callback fails, the entire <c>PublishAsync</c> will throw an exception once all
    /// callbacks have finished or faulted.
    /// </para>
    /// </remarks>
    Task PublishAsync<TEntity>(EntityEventArgs eventData, object sender = null);
}