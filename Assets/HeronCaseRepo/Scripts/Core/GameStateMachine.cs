public enum GameState { Idle, Playing, LevelComplete }

public class GameStateMachine
{
    public GameState Current { get; private set; } = GameState.Idle;

    public void Enter(GameState newState)
    {
        if (Current == newState)
        {
            return;
        }
        
        Current = newState;
        EventBus<GameStateChangedEvent>.Publish(new GameStateChangedEvent { State = newState });
    }
}
