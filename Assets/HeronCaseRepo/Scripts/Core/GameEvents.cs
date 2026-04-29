public struct GameStateChangedEvent
{
    public GameState State;
}

public struct LevelCompletedEvent { }

public struct PourAnimationCompletedEvent
{
    public TubeView From;
    public TubeView To;
}

public struct PourCompletedEvent
{
    public TubeView From;
    public TubeView To;
}

public struct TubeClickedEvent
{
    public TubeView Tube;
}

public struct ShakeCompletedEvent
{
    public TubeView Tube;
}
