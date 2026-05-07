using System.Collections.Generic;

public static class MoveValidator
{
    public static bool CanPour(TubeView from, TubeView to)
    {
        if (from.IsEmpty || to.IsFull)
            return false;

        return to.IsEmpty || to.TopColor == from.TopColor;
    }

    public static bool IsLevelComplete(List<TubeView> tubes)
    {
        for (var i = 0; i < tubes.Count; i++)
        {
            if (!tubes[i].IsEmpty && !tubes[i].IsSolved)
                return false;
        }

        return true;
    }
}
