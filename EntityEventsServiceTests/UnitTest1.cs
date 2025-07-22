using EntityEvents;

public class EntityEventsServiceTests
{
    [Fact]
    public void EntityEventArgs_Constructor_ThrowsArgumentNullException_WhenEntityIdsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new EntityEventArgs(null, EntityActionType.Add));
    }

    [Fact]
    public void EntityEventArgs_Constructor_InitializesEntityIdsAndActionType()
    {
        var entityIds = new List<int> { 1, 2, 3 };
        var actionType = EntityActionType.Update;
        var args = new EntityEventArgs(entityIds, actionType);

        Assert.Equal(entityIds, args.EntityIds);
        Assert.Equal(actionType, args.EntityActionType);
    }

    [Fact]
    public void EntityEventArgs_EntityIds_IsReadOnly()
    {
        var entityIds = new List<int> { 1, 2, 3 };
        var args = new EntityEventArgs(entityIds, EntityActionType.Add);

        Assert.IsAssignableFrom<IReadOnlyList<int>>(args.EntityIds);
        Assert.Equal(entityIds.Count, args.EntityIds.Count);
        Assert.Equal(entityIds[0], args.EntityIds[0]);
        Assert.Equal(entityIds[1], args.EntityIds[1]);
        Assert.Equal(entityIds[2], args.EntityIds[2]);
    }


    [Fact]
    public async Task Subscribe_PublishAsync_CallsCallback()
    {
        var service = new EntityEvents.EntityEventsService();
        var subscriber = new object();
        bool callbackCalled = false;
        EntityEventArgs receivedArgs = default;
        object receivedSender = null;

        Func<EntityEventArgs, object, Task> callback = (args, sender) =>
        {
            callbackCalled = true;
            receivedArgs = args;
            receivedSender = sender;
            return Task.CompletedTask;
        };

        service.Subscribe<int>(subscriber, callback);

        var eventArgs = new EntityEventArgs(new List<int> { 1 }, EntityActionType.Add);
        var sender = new object();
        await service.PublishAsync<int>(eventArgs, sender);

        Assert.True(callbackCalled);
        Assert.Equal(eventArgs, receivedArgs);
        Assert.Equal(sender, receivedSender);
    }

    [Fact]
    public async Task Subscribe_PublishAsync_DoesNotCallCallback_WhenSenderIsSubscriber()
    {
        var service = new EntityEvents.EntityEventsService();
        var subscriber = new object();
        bool callbackCalled = false;

        Func<EntityEventArgs, object, Task> callback = (args, sender) =>
        {
            callbackCalled = true;
            return Task.CompletedTask;
        };

        service.Subscribe<int>(subscriber, callback);

        var eventArgs = new EntityEventArgs(new List<int> { 1 }, EntityActionType.Add);
        await service.PublishAsync<int>(eventArgs, subscriber); // Sender is the subscriber

        Assert.False(callbackCalled);
    }

    [Fact]
    public async Task Unsubscribe_RemovesSubscription()
    {
        var service = new EntityEvents.EntityEventsService();
        var subscriber = new object();
        bool callbackCalled = false;

        Func<EntityEventArgs, object, Task> callback = (args, sender) =>
        {
            callbackCalled = true;
            return Task.CompletedTask;
        };

        service.Subscribe<int>(subscriber, callback);
        service.Unsubscribe<int>(subscriber);

        var eventArgs = new EntityEventArgs(new List<int> { 1 }, EntityActionType.Add);
        await service.PublishAsync<int>(eventArgs);

        Assert.False(callbackCalled);
    }

    [Fact]
    public async Task PublishAsync_ThrowsException_WhenEventArgsAreDefault()
    {
        var service = new EntityEvents.EntityEventsService();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await service.PublishAsync<int>(default(EntityEventArgs));
        });
    }

    [Fact]
    public async Task PublishAsync_AggregatesExceptionsFromSubscribers()
    {
        var service = new EntityEvents.EntityEventsService();
        var subscriber1 = new object();
        var subscriber2 = new object();

        Func<EntityEventArgs, object, Task> callback1 = (args, sender) => throw new Exception("Callback 1 failed");
        Func<EntityEventArgs, object, Task> callback2 = (args, sender) => throw new Exception("Callback 2 failed");

        service.Subscribe<int>(subscriber1, callback1);
        service.Subscribe<int>(subscriber2, callback2);

        var eventArgs = new EntityEventArgs(new List<int> { 1 }, EntityActionType.Add);

        await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await service.PublishAsync<int>(eventArgs);
        });
    }

    [Fact]
    public async Task Subscribe_MultipleSubscribers_AllCallbacksAreCalled()
    {
        var service = new EntityEvents.EntityEventsService();
        var subscriber1 = new object();
        var subscriber2 = new object();

        bool callback1Called = false;
        bool callback2Called = false;

        Func<EntityEventArgs, object, Task> callback1 = (args, sender) =>
        {
            callback1Called = true;
            return Task.CompletedTask;
        };

        Func<EntityEventArgs, object, Task> callback2 = (args, sender) =>
        {
            callback2Called = true;
            return Task.CompletedTask;
        };

        service.Subscribe<int>(subscriber1, callback1);
        service.Subscribe<int>(subscriber2, callback2);

        var eventArgs = new EntityEventArgs(new List<int> { 1 }, EntityActionType.Add);
        await service.PublishAsync<int>(eventArgs);

        Assert.True(callback1Called);
        Assert.True(callback2Called);
    }

    [Fact]
    public void Subscribe_ThrowsArgumentNullException_WhenSubscriberIsNull()
    {
        var service = new EntityEvents.EntityEventsService();
        Func<EntityEventArgs, object, Task> callback = (args, sender) => Task.CompletedTask;

        Assert.Throws<ArgumentNullException>(() => service.Subscribe<int>(null, callback));
    }

    [Fact]
    public void Subscribe_ThrowsArgumentNullException_WhenCallbackIsNull()
    {
        var service = new EntityEvents.EntityEventsService();
        var subscriber = new object();

        Assert.Throws<ArgumentNullException>(() => service.Subscribe<int>(subscriber, null));
    }

    [Fact]
    public void Unsubscribe_DoesNotThrow_WhenSubscriberIsNull()
    {
        var service = new EntityEvents.EntityEventsService();
        service.Unsubscribe<int>(null); // Should not throw an exception
    }

    [Fact]
    public async Task PublishAsync_NoSubscribers_DoesNotThrow()
    {
        var service = new EntityEvents.EntityEventsService();
        var eventArgs = new EntityEventArgs(new List<int> { 1 }, EntityActionType.Add);

        // Should not throw an exception
        await service.PublishAsync<int>(eventArgs);
    }
}