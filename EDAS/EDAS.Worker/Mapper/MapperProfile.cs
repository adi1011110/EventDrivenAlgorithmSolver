using AutoMapper;
using EDAS.BacktrackingCombinatronics;
using EDAS.Common.Models;
using EDAS.Sorting;
using EDAS.Worker.Handlers.Commands.Combinations;
using EDAS.Worker.Handlers.Commands.Sorting;
namespace EDAS.Worker.Mapper;

public class MapperProfile : Profile
{
	public MapperProfile()
	{
        CreateMap<CombinationsInputModel, CombinationsInputCommand>()
             .ForMember(dest => dest.Elements, 
             opt => opt.MapFrom<CommaSeparatedStringToIntCombinatronicsResolver>());

        CreateMap<CombinationsInputCommand, CombinationAlgoInput>();

        CreateMap<CombinationAlgoOutput, CombinationsOutput>()
            .ForMember(dest => dest.Elements,
                opt => opt.MapFrom(src => src.Solution));

        CreateMap<SortingInputCommand, SortingAlgoInput>();

        CreateMap<SortingAlgoOutput, SortingOutputResult>();

        CreateMap<SortingInputModel, SortingInputCommand>()
            .ForMember(dest => dest.Numbers,
            opt => opt.MapFrom<CommaSeparatedStringToIntSortingResolver>());
    }

}
