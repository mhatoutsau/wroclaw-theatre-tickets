namespace WroclawTheatreTickets.Application.UseCases.Favorites.Commands;

using MediatR;
using WroclawTheatreTickets.Application.Contracts.Repositories;
using WroclawTheatreTickets.Domain.Entities;

public record AddFavoriteCommand(Guid UserId, Guid ShowId) : IRequest<Unit>;

public class AddFavoriteCommandHandler : IRequestHandler<AddFavoriteCommand, Unit>
{
    private readonly IFavoriteRepository _favoriteRepository;

    public AddFavoriteCommandHandler(IFavoriteRepository favoriteRepository)
    {
        _favoriteRepository = favoriteRepository;
    }

    public async Task<Unit> Handle(AddFavoriteCommand request, CancellationToken cancellationToken)
    {
        var existing = await _favoriteRepository.GetAsync(request.UserId, request.ShowId);
        if (existing != null)
            return Unit.Value;

        var favorite = UserFavorite.Create(request.UserId, request.ShowId);
        await _favoriteRepository.AddAsync(favorite);

        return Unit.Value;
    }
}

public record RemoveFavoriteCommand(Guid UserId, Guid ShowId) : IRequest<Unit>;

public class RemoveFavoriteCommandHandler : IRequestHandler<RemoveFavoriteCommand, Unit>
{
    private readonly IFavoriteRepository _favoriteRepository;

    public RemoveFavoriteCommandHandler(IFavoriteRepository favoriteRepository)
    {
        _favoriteRepository = favoriteRepository;
    }

    public async Task<Unit> Handle(RemoveFavoriteCommand request, CancellationToken cancellationToken)
    {
        await _favoriteRepository.RemoveAsync(request.UserId, request.ShowId);
        return Unit.Value;
    }
}
