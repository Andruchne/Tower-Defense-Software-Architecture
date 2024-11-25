using System;

public abstract class Event { }

// To subscribe: EventBus<EventClass>.OnEvent += Function;
// To invoke event: EventBus<EventClass>.Publish(new EventClass())
public class EventBus<T> where T : Event
{
    public static event Action<T> OnEvent;

    public static void Publish(T pEvent)
    {
        OnEvent?.Invoke(pEvent);
    }
}

#region Level Management
public class OnLevelFinishedEvent : Event
{
    // Invoke to indicate that level is complete
    public OnLevelFinishedEvent(bool isGameWon) 
    {
        this.isGameWon = isGameWon;
    }
    bool isGameWon;
}
public class OnLevelLoadedEvent : Event
{
    // Invoke to indicate that level is loaded
    // Necessary to enable cursor, after scene load
    public OnLevelLoadedEvent() { }
}

public class OnLevelUnloadedEvent : Event
{
    // Invoke to indicate that level is unloaded
    public OnLevelUnloadedEvent() { }
}
#endregion

#region Game
public class OnGetGoldEvent : Event
{
    // Invoke to give player gold
    public OnGetGoldEvent(int goldAmount) 
    {
        this.goldAmount = goldAmount;
    }
    public int goldAmount;
}
public class OnDamagePlayerEvent : Event
{
    // Invoke to damage player
    public OnDamagePlayerEvent(float damage) 
    {
        this.damage = damage;
    }
    public float damage;
}
#endregion