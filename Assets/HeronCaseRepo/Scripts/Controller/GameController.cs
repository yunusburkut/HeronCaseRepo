using System;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    
    public GameObject OnLevelCompletedTest;
    public event Action OnLevelCompleted;

    private TubeView _selectedTube;
    private List<TubeView> _allTubes;
    private bool _isAnimating;

    private TubeView _shakeTarget;
    private TubeView _pourFrom;
    private TubeView _pourTo;
    private Action _onShakeComplete;
    private Action _onPourComplete;

    private void Awake()
    {
        _onShakeComplete = OnShakeComplete;
        _onPourComplete = OnPourComplete;
    }

    public void Initialize(List<TubeView> tubes)
    {
        _allTubes = new List<TubeView>(tubes);
        OnLevelCompleted += OnLevelComplete;
    }

    public void OnTubeClicked(TubeView tube)
    {
        if (_isAnimating)
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

        if (!MoveValidator.CanPour(_selectedTube, tube))
        {
            _shakeTarget = _selectedTube;
            _selectedTube = null;
            _isAnimating = true;
            _shakeTarget.Shake(_onShakeComplete);
            return;
        }

        _pourFrom = _selectedTube;
        _pourTo = tube;
        _selectedTube = null;
        _isAnimating = true;
        _pourFrom.SetSelected(false);
        _pourFrom.PourInto(_pourTo, _onPourComplete);
    }

    private void OnShakeComplete()
    {
        _isAnimating = false;
        _shakeTarget.SetSelected(false);
    }

    private void OnPourComplete()
    {
        _isAnimating = false;
        CheckAfterPour(_pourFrom, _pourTo);
    }

    private void OnLevelComplete()
    {
        OnLevelCompletedTest.SetActive(true);
    }

    private void CheckAfterPour(TubeView from, TubeView to)
    {
        from.TrySetSolved();
        to.TrySetSolved();

        if (MoveValidator.IsLevelComplete(_allTubes))
        {
            OnLevelCompleted?.Invoke();
        }
    }
}
