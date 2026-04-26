using System;
using System.Collections.Generic;

public interface ILevelController
{
    event Action OnLevelCompleted;
    void Initialize(List<TubeView> tubes);
    void OnTubeClicked(TubeView tube);
}
