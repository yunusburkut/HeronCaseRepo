using System;

public enum GameState { Idle, Playing, LevelComplete }

public class GameStateMachine
{
    public GameState Current { get; private set; } = GameState.Idle;
    public event Action<GameState> OnStateChanged;

    public void Enter(GameState newState)
    {
        if (Current == newState)
        {
            return;
        } 
        Current = newState;
        OnStateChanged?.Invoke(newState);
    }
}
