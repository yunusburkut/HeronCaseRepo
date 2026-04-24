using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private TubeView _selectedTube;
    private List<TubeView> _allTubes;

    public void Initialize(List<TubeView> tubes)
    {
        _allTubes = new List<TubeView>(tubes);
    }

    public void OnTubeClicked(TubeView tube)
    {
        if (tube.IsSolved)
        {
            return;
        }

        if (_selectedTube == null)
        {
            if (tube.IsEmpty)
            {
                return;
            }

            _selectedTube = tube;
            tube.SetSelected(true);
            return;
        }

        if (_selectedTube == tube)
        {
            tube.SetSelected(false);
            _selectedTube = null;
            return;
        }

        if (!CanPour(_selectedTube, tube))
        {
            TubeView toShake = _selectedTube;
            _selectedTube = null;
            toShake.Shake(() => toShake.SetSelected(false));
            return;
        }

        TubeView from = _selectedTube;
        _selectedTube = null;
        from.SetSelected(false);
        from.PourInto(tube, () => CheckAfterPour(from, tube));
    }

    private bool CanPour(TubeView from, TubeView to)
    {
        if (from.IsEmpty || to.IsFull)
        {
            return false;
        }

        return to.IsEmpty || to.TopColor == from.TopColor;
    }

    private void CheckAfterPour(TubeView from, TubeView to)
    {
        from.TrySetSolved();
        to.TrySetSolved();

        if (IsLevelComplete())
        {
            OnLevelComplete();
        }
    }

    private bool IsLevelComplete()
    {
        foreach (var tube in _allTubes)
        {
            if (!tube.IsEmpty && !tube.IsSolved)
            {
                return false;
            }
        }

        return true;
    }

    private void OnLevelComplete()
    {
        Debug.Log("Level Complete!");
    }
}
