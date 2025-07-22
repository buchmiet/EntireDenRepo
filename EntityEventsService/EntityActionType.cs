namespace EntityEvents;

/// <summary>
/// Represents the type of operation performed on an entity, such as Add, Delete, or Update.
/// </summary>
public enum EntityActionType
{
    /// <summary>
    /// A new entity has been added to the system.
    /// </summary>
    Add,

    /// <summary>
    /// An existing entity has been removed from the system.
    /// </summary>
    Delete,

    /// <summary>
    /// An existing entity has been updated in the system.
    /// </summary>
    Update
}