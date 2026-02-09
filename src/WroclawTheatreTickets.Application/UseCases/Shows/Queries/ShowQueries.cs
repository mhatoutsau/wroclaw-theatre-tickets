namespace WroclawTheatreTickets.Application.UseCases.Shows.Queries;

using MediatR;
using WroclawTheatreTickets.Application.Contracts.Dtos;
using WroclawTheatreTickets.Application.Contracts.Repositories;
using AutoMapper;

public record GetAllShowsQuery : IRequest<IEnumerable<ShowDto>>;

public class GetAllShowsQueryHandler : IRequestHandler<GetAllShowsQuery, IEnumerable<ShowDto>>
{
    private readonly IShowRepository _showRepository;
    private readonly IMapper _mapper;

    public GetAllShowsQueryHandler(IShowRepository showRepository, IMapper mapper)
    {
        _showRepository = showRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ShowDto>> Handle(GetAllShowsQuery request, CancellationToken cancellationToken)
    {
        var shows = await _showRepository.GetActiveAsync();
        return _mapper.Map<IEnumerable<ShowDto>>(shows);
    }
}

public record GetShowByIdQuery(Guid ShowId) : IRequest<ShowDetailDto>;

public class GetShowByIdQueryHandler : IRequestHandler<GetShowByIdQuery, ShowDetailDto>
{
    private readonly IShowRepository _showRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public GetShowByIdQueryHandler(IShowRepository showRepository, IReviewRepository reviewRepository, IMapper mapper)
    {
        _showRepository = showRepository;
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<ShowDetailDto> Handle(GetShowByIdQuery request, CancellationToken cancellationToken)
    {
        var show = await _showRepository.GetByIdWithDetailsAsync(request.ShowId);
        if (show == null)
            throw new Exception($"Show with id {request.ShowId} not found");

        show.IncrementViewCount();
        await _showRepository.UpdateAsync(show);

        var dto = _mapper.Map<ShowDetailDto>(show);
        var reviews = await _reviewRepository.GetApprovedByShowIdAsync(request.ShowId);
        dto.Reviews = _mapper.Map<ICollection<ReviewDto>>(reviews);

        return dto;
    }
}

public record SearchShowsQuery(string Keyword) : IRequest<IEnumerable<ShowDto>>;

public class SearchShowsQueryHandler : IRequestHandler<SearchShowsQuery, IEnumerable<ShowDto>>
{
    private readonly IShowRepository _showRepository;
    private readonly IMapper _mapper;

    public SearchShowsQueryHandler(IShowRepository showRepository, IMapper mapper)
    {
        _showRepository = showRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ShowDto>> Handle(SearchShowsQuery request, CancellationToken cancellationToken)
    {
        var shows = await _showRepository.SearchAsync(request.Keyword);
        return _mapper.Map<IEnumerable<ShowDto>>(shows);
    }
}

public record GetUpcomingShowsQuery(int Days = 30) : IRequest<IEnumerable<ShowDto>>;

public class GetUpcomingShowsQueryHandler : IRequestHandler<GetUpcomingShowsQuery, IEnumerable<ShowDto>>
{
    private readonly IShowRepository _showRepository;
    private readonly IMapper _mapper;

    public GetUpcomingShowsQueryHandler(IShowRepository showRepository, IMapper mapper)
    {
        _showRepository = showRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ShowDto>> Handle(GetUpcomingShowsQuery request, CancellationToken cancellationToken)
    {
        var shows = await _showRepository.GetUpcomingAsync(request.Days);
        return _mapper.Map<IEnumerable<ShowDto>>(shows);
    }
}

public record GetMostViewedShowsQuery(int Count = 10) : IRequest<IEnumerable<ShowDto>>;

public class GetMostViewedShowsQueryHandler : IRequestHandler<GetMostViewedShowsQuery, IEnumerable<ShowDto>>
{
    private readonly IShowRepository _showRepository;
    private readonly IMapper _mapper;

    public GetMostViewedShowsQueryHandler(IShowRepository showRepository, IMapper mapper)
    {
        _showRepository = showRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ShowDto>> Handle(GetMostViewedShowsQuery request, CancellationToken cancellationToken)
    {
        var shows = await _showRepository.GetMostViewedAsync(request.Count);
        return _mapper.Map<IEnumerable<ShowDto>>(shows);
    }
}
