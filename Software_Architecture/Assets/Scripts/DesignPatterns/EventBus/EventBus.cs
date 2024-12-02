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

// Finishing level
public class OnLevelFinishedEvent : Event
{
    // Invoke to indicate that level is complete
    public OnLevelFinishedEvent() { }
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

// Level states
public class OnStartBreakTime : Event
{
    public OnStartBreakTime() { }
}
public class OnStopBreakTimeEarly : Event
{
    public OnStopBreakTimeEarly() { }
}
public class OnStopBreakTime : Event
{
    public OnStopBreakTime() { }
}

#endregion

#region Player

// General Player
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
public class OnPlayerDefeatedEvent : Event
{
    // Invoke to indicate player's defeat
    public OnPlayerDefeatedEvent() { }
}

// Player UI
public class OnUpdateCurrentGold : Event
{
    public OnUpdateCurrentGold(int currentAmount)
    {
        this.currentAmount = currentAmount;
    }
    public int currentAmount;
}
public class OnUpdateCurrentHealth : Event
{
    public OnUpdateCurrentHealth(float currentPercent)
    {
        this.currentPercent = currentPercent;
    }
    public float currentPercent;
}
public class OnUpdateCurrentTime : Event
{
    public OnUpdateCurrentTime(int currentAmount)
    {
        this.currentAmount = currentAmount;
    }
    public int currentAmount;
}

#endregion