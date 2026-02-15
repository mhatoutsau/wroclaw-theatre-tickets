namespace WroclawTheatreTickets.Application.UseCases.Shows.Queries;

using MediatR;
using WroclawTheatreTickets.Application.Contracts.Dtos;
using WroclawTheatreTickets.Application.Contracts.Repositories;
using WroclawTheatreTickets.Application.Contracts.Services;
using WroclawTheatreTickets.Application.Contracts.Cache;
using AutoMapper;
using Microsoft.Extensions.Options;

public record GetAllShowsQuery : IRequest<IEnumerable<ShowDto>>;

public class GetAllShowsQueryHandler : IRequestHandler<GetAllShowsQuery, IEnumerable<ShowDto>>
{
    private readonly IShowRepository _showRepository;
    private readonly ICacheService _cacheService;
    private readonly CacheOptions _cacheOptions;
    private readonly IMapper _mapper;

    public GetAllShowsQueryHandler(IShowRepository showRepository, ICacheService cacheService, IOptions<CacheOptions> cacheOptions, IMapper mapper)
    {
        _showRepository = showRepository;
        _cacheService = cacheService;
        _cacheOptions = cacheOptions.Value;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ShowDto>> Handle(GetAllShowsQuery request, CancellationToken cancellationToken)
    {
        if (_cacheOptions.Enabled)
        {
            var cached = await _cacheService.GetAsync<IEnumerable<ShowDto>>(CacheKeys.ShowsActive);
            if (cached != null)
            {
                return cached;
            }
        }

        var shows = await _showRepository.GetActiveAsync();
        var result = _mapper.Map<IEnumerable<ShowDto>>(shows);

        if (_cacheOptions.Enabled)
        {
            var ttl = CacheOptions.ToTimeSpan(_cacheOptions.AllShowsTtlMinutes);
            await _cacheService.SetAsync(CacheKeys.ShowsActive, result, ttl);
        }

        return result;
    }
}

public record GetShowByIdQuery(Guid ShowId) : IRequest<ShowDetailDto>;

public class GetShowByIdQueryHandler : IRequestHandler<GetShowByIdQuery, ShowDetailDto>
{
    private readonly IShowRepository _showRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly ICacheService _cacheService;
    private readonly CacheOptions _cacheOptions;
    private readonly IMapper _mapper;

    public GetShowByIdQueryHandler(IShowRepository showRepository, IReviewRepository reviewRepository, ICacheService cacheService, IOptions<CacheOptions> cacheOptions, IMapper mapper)
    {
        _showRepository = showRepository;
        _reviewRepository = reviewRepository;
        _cacheService = cacheService;
        _cacheOptions = cacheOptions.Value;
        _mapper = mapper;
    }

    public async Task<ShowDetailDto> Handle(GetShowByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = string.Format(CacheKeys.ShowDetail, request.ShowId);

        if (_cacheOptions.Enabled)
        {
            var cached = await _cacheService.GetAsync<ShowDetailDto>(cacheKey);
            if (cached != null)
            {
                return cached;
            }
        }

        var show = await _showRepository.GetByIdWithDetailsAsync(request.ShowId);
        if (show == null)
            throw new Exception($"Show with id {request.ShowId} not found");

        show.IncrementViewCount();
        await _showRepository.UpdateAsync(show);

        var dto = _mapper.Map<ShowDetailDto>(show);
        var reviews = await _reviewRepository.GetApprovedByShowIdAsync(request.ShowId);
        dto.Reviews = _mapper.Map<ICollection<ReviewDto>>(reviews);

        if (_cacheOptions.Enabled)
        {
            var ttl = CacheOptions.ToTimeSpan(_cacheOptions.ShowDetailTtlMinutes);
            await _cacheService.SetAsync(cacheKey, dto, ttl);
        }

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
    private readonly ICacheService _cacheService;
    private readonly CacheOptions _cacheOptions;
    private readonly IMapper _mapper;

    public GetUpcomingShowsQueryHandler(IShowRepository showRepository, ICacheService cacheService, IOptions<CacheOptions> cacheOptions, IMapper mapper)
    {
        _showRepository = showRepository;
        _cacheService = cacheService;
        _cacheOptions = cacheOptions.Value;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ShowDto>> Handle(GetUpcomingShowsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = string.Format(CacheKeys.ShowsUpcoming, request.Days);

        if (_cacheOptions.Enabled)
        {
            var cached = await _cacheService.GetAsync<IEnumerable<ShowDto>>(cacheKey);
            if (cached != null)
            {
                return cached;
            }
        }

        var shows = await _showRepository.GetUpcomingAsync(request.Days);
        var result = _mapper.Map<IEnumerable<ShowDto>>(shows);

        if (_cacheOptions.Enabled)
        {
            var ttl = CacheOptions.ToTimeSpan(_cacheOptions.UpcomingShowsTtlMinutes);
            await _cacheService.SetAsync(cacheKey, result, ttl);
        }

        return result;
    }
}

public record GetMostViewedShowsQuery(int Count = 10) : IRequest<IEnumerable<ShowDto>>;

public class GetMostViewedShowsQueryHandler : IRequestHandler<GetMostViewedShowsQuery, IEnumerable<ShowDto>>
{
    private readonly IShowRepository _showRepository;
    private readonly ICacheService _cacheService;
    private readonly CacheOptions _cacheOptions;
    private readonly IMapper _mapper;

    public GetMostViewedShowsQueryHandler(IShowRepository showRepository, ICacheService cacheService, IOptions<CacheOptions> cacheOptions, IMapper mapper)
    {
        _showRepository = showRepository;
        _cacheService = cacheService;
        _cacheOptions = cacheOptions.Value;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ShowDto>> Handle(GetMostViewedShowsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = string.Format(CacheKeys.ShowsTrending, request.Count);

        if (_cacheOptions.Enabled)
        {
            var cached = await _cacheService.GetAsync<IEnumerable<ShowDto>>(cacheKey);
            if (cached != null)
            {
                return cached;
            }
        }

        var shows = await _showRepository.GetMostViewedAsync(request.Count);
        var result = _mapper.Map<IEnumerable<ShowDto>>(shows);

        if (_cacheOptions.Enabled)
        {
            var ttl = CacheOptions.ToTimeSpan(_cacheOptions.TrendingShowsTtlMinutes);
            await _cacheService.SetAsync(cacheKey, result, ttl);
        }

        return result;
    }
}
