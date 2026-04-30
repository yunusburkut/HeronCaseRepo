using System;

public static class EventBus<T>
{
    private static Action<T> _handlers;

    public static void Subscribe(Action<T> handler)
    {
        _handlers += handler;
    }

    public static void Unsubscribe(Action<T> handler)
    {
        _handlers -= handler;
    }

    public static void Publish(T eventData)
    {
        _handlers?.Invoke(eventData);
    }
}
