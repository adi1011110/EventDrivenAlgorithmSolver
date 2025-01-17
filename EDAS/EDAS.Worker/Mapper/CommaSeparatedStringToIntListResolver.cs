using EDAS.Worker.Handlers.Commands.Combinations;
using EDAS.Worker.Handlers.Commands.Sorting;
using System.Collections;

namespace EDAS.Worker.Mapper;

public class CommaSeparatedStringToIntListResolver<TSource, TDestination> 
    : IValueResolver<TSource, TDestination, List<int>>
    where TSource : BaseCSVElementsInputModel
{
    public List<int> Resolve(TSource source,
        TDestination destination,
        List<int> destMember, 
        ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(source.ElementsCSV))
            return new List<int>();

        List<int> elements = new List<int>();

        try
        {
            elements = source.ElementsCSV
               .Split(',', StringSplitOptions.RemoveEmptyEntries)
               .Select(int.Parse)
               .ToList();
        }
        catch
        {
            return new List<int>();
        }

        return elements;

    }
}

public class CommaSeparatedStringToIntCombinatronicsResolver : 
    CommaSeparatedStringToIntListResolver<
        CombinationsInputModel, 
        CombinationsInputCommand>
{
}

public class CommaSeparatedStringToIntSortingResolver :
    CommaSeparatedStringToIntListResolver<
        SortingInputModel,
        SortingInputCommand>
{ }