using System.Collections.Generic;

public static class MoveValidator
{
    public static bool CanPour(TubeView from, TubeView to)
    {
        if (from.IsEmpty || to.IsFull)
        {
            return false;
        }

        return to.IsEmpty || to.TopColor == from.TopColor;
    }

    public static bool IsLevelComplete(List<TubeView> tubes)
    {
        foreach (var tube in tubes)
        {
            if (!tube.IsEmpty && !tube.IsSolved)
            {
                return false;
            }
        }

        return true;
    }
}
