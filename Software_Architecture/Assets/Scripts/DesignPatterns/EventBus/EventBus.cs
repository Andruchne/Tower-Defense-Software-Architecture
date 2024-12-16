using System;

/// <summary>
/// Key part of the project script architecture
/// It enables decoupled invoking and listening, for whichever event currently needed
/// </summary>

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
public class OnLevelWon : Event
{
    public OnLevelWon() { }
}
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
public class OnQueueUpBreakTime : Event
{
    // Invoke to start a break
    public OnQueueUpBreakTime() { }
}
public class OnStartedBreakTime : Event
{
    // Notifies, that the break timer has started
    public OnStartedBreakTime() { }
}
public class OnStopBreakTimeEarly : Event
{
    // Notifies, that the break timer has ended early
    public OnStopBreakTimeEarly() { }
}
public class OnStopBreakTime : Event
{
    // Notifies, that the break time has ended
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
public class OnWithdrawGoldEvent : Event
{
    // Withdraws gold from the player
    public OnWithdrawGoldEvent(int amount) 
    {
        this.goldAmount = amount;
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
public class OnPlayerHUDLoaded : Event
{
    // Used to signal, that the player HUD has finished loading
    public OnPlayerHUDLoaded() { }
}
public class OnUpdateCurrentGold : Event
{
    // Updates the gold UI for the player
    public OnUpdateCurrentGold(int currentAmount)
    {
        this.currentAmount = currentAmount;
    }
    public int currentAmount;
}
public class OnUpdateCurrentHealth : Event
{
    // Updates health UI for the player
    public OnUpdateCurrentHealth(float currentPercent)
    {
        this.currentPercent = currentPercent;
    }
    public float currentPercent;
}
public class OnUpdateCurrentTime : Event
{
    // Updates the current break time UI for the player
    public OnUpdateCurrentTime(int currentAmount)
    {
        this.currentAmount = currentAmount;
    }
    public int currentAmount;
}
public class OnUpdateCurrentWave : Event
{
    public OnUpdateCurrentWave(int currentWave, int maxWave) 
    {
        this.currentWave = currentWave;
        this.maxWave = maxWave;
    }
    public int currentWave;
    public int maxWave;
}

// Camera Movement
public class OnCameraMoved : Event
{
    // Invoked when camera was moved
    public OnCameraMoved() { }
}

#endregion

#region Debug

public class OnOneHitEnemies : Event
{
    // Invoke to one hit enemies with any attack
    public OnOneHitEnemies(bool state) 
    {
        this.state = state;
    }
    public bool state;
}

public class OnInfiniteRiches : Event
{
    // Invoke to have endless gold
    public OnInfiniteRiches(bool state)
    {
        this.state = state;
    }
    public bool state;
}

public class OnImmediateLooser : Event
{
    // Invoke to be a looser
    public OnImmediateLooser(bool state)
    {
        this.state = state;
    }
    public bool state;
}

public class OnInvincibleBase : Event
{
    // Invoke to be invincible
    public OnInvincibleBase(bool state)
    {
        this.state = state;
    }
    public bool state;
}

#endregion