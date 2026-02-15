namespace WroclawTheatreTickets.Application.UseCases.Shows.Commands;

using MediatR;
using WroclawTheatreTickets.Application.Contracts.Dtos;
using WroclawTheatreTickets.Application.Contracts.Repositories;
using WroclawTheatreTickets.Domain.Entities;
using AutoMapper;

public record FilterShowsCommand(ShowFilterCriteria Criteria) : IRequest<IEnumerable<ShowDto>>;

public class FilterShowsCommandHandler : IRequestHandler<FilterShowsCommand, IEnumerable<ShowDto>>
{
    private readonly IShowRepository _showRepository;
    private readonly IMapper _mapper;

    public FilterShowsCommandHandler(IShowRepository showRepository, IMapper mapper)
    {
        _showRepository = showRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ShowDto>> Handle(FilterShowsCommand request, CancellationToken cancellationToken)
    {
        var shows = await _showRepository.FilterAsync(request.Criteria);
        return _mapper.Map<IEnumerable<ShowDto>>(shows);
    }
}
