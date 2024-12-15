using EDAS.BacktrackingCombinatronics;

namespace EDAS.Worker.Services.Factory;

public interface ICombinationsAlgorithmFactory
{
    ICombinationAlgo Create(CombinationAlgoInput input);
}
