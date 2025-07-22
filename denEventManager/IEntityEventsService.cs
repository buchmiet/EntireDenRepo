using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace denEventManager
{
    /// <summary>
    /// Defines a contract for a simple publish-subscribe notification center
    /// that supports asynchronous subscribers (returning <see cref="Task"/>).
    /// </summary>
    /// <remarks>
    /// This interface allows clients to:
    /// <list type="bullet">
    ///   <item>Subscribe to receive certain types of events.</item>
    ///   <item>Unsubscribe to stop receiving those events.</item>
    ///   <item>Publish new events asynchronously, which will be delivered to subscribers.</item>
    /// </list>
    /// By using an interface, we can inject a concrete implementation in any part of the app
    /// via dependency injection, promoting loose coupling and testability.
    /// </remarks>
    public interface IEntityEventsService
    {
        /// <summary>
        /// Subscribes to events of type <typeparamref name="T"/> with an asynchronous callback.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the event data the subscriber wants to receive.
        /// </typeparam>
        /// <param name="subscriber">
        /// The object subscribing to the event. This allows us to identify the subscriber
        /// (for example, to avoid notifying the same object if it itself is the publisher).
        /// </param>
        /// <param name="callback">
        /// The asynchronous function to be executed when an event of type <typeparamref name="T"/> is published.
        /// The callback receives two parameters:
        /// <list type="number">
        ///   <item>The event data of type <typeparamref name="T"/>.</item>
        ///   <item>An <c>object</c> reference to the sender (the publisher).</item>
        /// </list>
        /// It should return a <see cref="Task"/>, allowing asynchronous operations inside the subscriber.
        /// </param>
        void Subscribe<T>(object subscriber, Func<EventArguments, object, Task> callback);

        /// <summary>
        /// Unsubscribes the specified subscriber from events of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the event data the subscriber was receiving.
        /// </typeparam>
        /// <param name="subscriber">
        /// The object to be unsubscribed.
        /// </param>
        void Unsubscribe<T>(object subscriber);

        /// <summary>
        /// Publishes (fires) an event of type <typeparamref name="T"/> asynchronously, optionally specifying the sender.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the event data to be published.
        /// </typeparam>
        /// <param name="eventData">
        /// The event data to pass to all subscribers of type <typeparamref name="T"/>.
        /// </param>
        /// <param name="sender">
        /// The object that is publishing the event (can be <c>this</c>). 
        /// If <c>null</c>, it indicates there's no specific sender.
        /// Subscribers may use this parameter to check if the event originated from themselves
        /// and potentially ignore it to prevent loops.
        /// </param>
        /// <returns>
        /// A task that completes when all subscribers have been notified (or if there are no subscribers).
        /// If any subscriber throws an exception, the <see cref="Task"/> may complete in a faulted state,
        /// depending on error handling logic.
        /// </returns>
        Task PublishAsync<T>(EventArguments eventData, object sender = null);
    }
}
