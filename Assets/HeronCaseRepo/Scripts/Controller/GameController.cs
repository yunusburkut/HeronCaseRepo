using UnityEngine;

public class GameController : MonoBehaviour
{
    private TubeView _selectedTube;

    public void OnTubeClicked(TubeView tube)
    {
        if (_selectedTube == null)
        {
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

        TubeView from = _selectedTube;
        _selectedTube = null;
        from.SetSelected(false);
        from.PourInto(tube, null);
    }
}
