using System.Collections.Generic;

public interface ILevelController
{
    void Initialize(List<TubeView> tubes);
    void OnTubeClicked(TubeView tube);
}
