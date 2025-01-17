using EDAS.Sorting;
using EDAS.Worker.Services.Factory.SortingAlgo;

namespace EDAS.Worker.Handlers.Commands.Sorting;

public record SortingInputCommand(List<int> Numbers) : IRequest<SortingOutputResult>;

public record SortingOutputResult(List<int> OutputSortedNumber);

public class SolveSortingAlgorithmHandler(ISortingAlgorithmFactory factory, IMapper mapper)
    : IRequestHandler<SortingInputCommand, SortingOutputResult>
{
    public async Task<SortingOutputResult> Handle(SortingInputCommand request, CancellationToken cancellationToken)
    {
        if(request.Numbers == null || request.Numbers.Count() == 0)
        {
            throw new ArgumentException("Bad arguments");
        }

        var input = mapper.Map<SortingAlgoInput>(request);

        var algorithmService = factory.Create(input);

        var output = algorithmService.Run();

        var result = mapper.Map<SortingOutputResult>(output);

        return result;
    }
}
