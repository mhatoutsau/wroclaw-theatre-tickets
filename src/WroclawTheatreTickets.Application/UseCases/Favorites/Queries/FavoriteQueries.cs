namespace WroclawTheatreTickets.Application.UseCases.Favorites.Queries;

using MediatR;
using WroclawTheatreTickets.Application.Contracts.Dtos;
using WroclawTheatreTickets.Application.Contracts.Repositories;
using AutoMapper;

public record GetUserFavoritesQuery(Guid UserId) : IRequest<IEnumerable<ShowDto>>;

public class GetUserFavoritesQueryHandler : IRequestHandler<GetUserFavoritesQuery, IEnumerable<ShowDto>>
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IMapper _mapper;

    public GetUserFavoritesQueryHandler(IFavoriteRepository favoriteRepository, IMapper mapper)
    {
        _favoriteRepository = favoriteRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ShowDto>> Handle(GetUserFavoritesQuery request, CancellationToken cancellationToken)
    {
        var shows = await _favoriteRepository.GetUserFavoritesAsync(request.UserId);
        return _mapper.Map<IEnumerable<ShowDto>>(shows);
    }
}
