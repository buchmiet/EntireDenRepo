namespace EntityEvents;

/// <summary>
/// An immutable, lightweight data structure containing information about a particular entity-related event,
/// including the affected entity IDs and the type of action performed.
/// </summary>
/// <remarks>
/// <para>
/// As a <c>readonly struct</c>, <see cref="EntityEventArgs"/> is immutable, so once you create an instance,
/// you cannot modify it. This can help avoid unintended side effects.
/// </para>
/// <para>
/// Example usage:
/// <code>
/// var args = new EntityEventArgs(
///     new List&lt;int&gt; { 42, 99 },
///     EntityActionType.Update
/// );
/// await _entityEvents.PublishAsync&lt;Order&gt;(args, this);
/// </code>
/// </para>
/// </remarks>
public readonly struct EntityEventArgs
{
    /// <summary>
    /// Creates a new instance of the <see cref="EntityEventArgs"/> struct
    /// with a collection of affected entity IDs and the type of action that occurred.
    /// </summary>
    /// <param name="entityIds">
    /// The IDs of the entities that were affected by this event. Cannot be null.
    /// </param>
    /// <param name="entityActionType">
    /// The type of operation performed, such as <see cref="EntityActionType.Add"/> or <see cref="EntityActionType.Update"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="entityIds"/> is null.
    /// </exception>
    public EntityEventArgs(IEnumerable<int> entityIds, EntityActionType entityActionType)
    {
        if (entityIds == null)
            throw new ArgumentNullException(nameof(entityIds));

        // We copy to avoid potential modifications from the outside
        // once this struct is constructed.
        EntityIds = entityIds.ToList();
        EntityActionType = entityActionType;
    }

    /// <summary>
    /// Gets the IDs of the entities that were affected by this event.
    /// </summary>
    /// <remarks>
    /// This is an <see cref="IReadOnlyList{Int32}"/> to ensure that no external modifications can be made
    /// after the struct is constructed.
    /// </remarks>
    public IReadOnlyList<int> EntityIds { get; }

    /// <summary>
    /// Gets the type of operation that occurred,
    /// such as <see cref="EntityActionType.Add"/> or <see cref="EntityActionType.Update"/>.
    /// </summary>
    public EntityActionType EntityActionType { get; }
}

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