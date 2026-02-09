namespace WroclawTheatreTickets.Web;

using MediatR;
using WroclawTheatreTickets.Application.UseCases.Shows.Queries;
using WroclawTheatreTickets.Application.UseCases.Shows.Commands;
using WroclawTheatreTickets.Application.UseCases.Users.Commands;
using WroclawTheatreTickets.Application.UseCases.Favorites.Queries;
using WroclawTheatreTickets.Application.UseCases.Favorites.Commands;
using WroclawTheatreTickets.Application.UseCases.Reviews.Commands;
using WroclawTheatreTickets.Application.Contracts.Dtos;
using WroclawTheatreTickets.Application.Contracts.Repositories;
using System.Security.Claims;

public static class Endpoints
{
    public static void RegisterEndpoints(WebApplication app)
    {
        // Show endpoints
        var showGroup = app.MapGroup("/api/shows").WithName("Shows");

        showGroup.MapGet("/", GetAllShows)
            .WithName("GetAllShows")
            .Produces<IEnumerable<ShowDto>>();

        showGroup.MapGet("/{id}", GetShowById)
            .WithName("GetShowById")
            .Produces<ShowDetailDto>();

        showGroup.MapGet("/upcoming", GetUpcomingShows)
            .WithName("GetUpcomingShows")
            .Produces<IEnumerable<ShowDto>>();

        showGroup.MapGet("/search", SearchShows)
            .WithName("SearchShows")
            .Produces<IEnumerable<ShowDto>>();

        showGroup.MapPost("/filter", FilterShows)
            .WithName("FilterShows")
            .Produces<IEnumerable<ShowDto>>();

        showGroup.MapGet("/trending/viewed", GetMostViewed)
            .WithName("GetMostViewed")
            .Produces<IEnumerable<ShowDto>>();

        // Auth endpoints
        var authGroup = app.MapGroup("/api/auth").WithName("Auth");

        authGroup.MapPost("/register", Register)
            .WithName("Register")
            .AllowAnonymous()
            .Produces<AuthenticationResponse>();

        authGroup.MapPost("/login", Login)
            .WithName("Login")
            .AllowAnonymous()
            .Produces<AuthenticationResponse>();

        authGroup.MapPost("/oauth", OAuthLogin)
            .WithName("OAuthLogin")
            .AllowAnonymous()
            .Produces<AuthenticationResponse>();

        // Favorites endpoints
        var favGroup = app.MapGroup("/api/favorites").WithName("Favorites").RequireAuthorization();

        favGroup.MapGet("/", GetUserFavorites)
            .WithName("GetUserFavorites")
            .Produces<IEnumerable<ShowDto>>();

        favGroup.MapPost("/{showId}", AddFavorite)
            .WithName("AddFavorite");

        favGroup.MapDelete("/{showId}", RemoveFavorite)
            .WithName("RemoveFavorite");

        // Review endpoints
        var reviewGroup = app.MapGroup("/api/reviews").WithName("Reviews");

        reviewGroup.MapPost("/", CreateReview)
            .WithName("CreateReview")
            .RequireAuthorization()
            .Produces<ReviewDto>();

        // Admin endpoints
        var adminGroup = app.MapGroup("/api/admin").WithName("Admin").RequireAuthorization("AdminOnly");

        adminGroup.MapPost("/reviews/{reviewId}/approve", ApproveReview)
            .WithName("ApproveReview");
    }

    // Show endpoints
    private static async Task<IResult> GetAllShows(IMediator mediator)
    {
        try
        {
            var result = await mediator.Send(new GetAllShowsQuery());
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    private static async Task<IResult> GetShowById(Guid id, IMediator mediator)
    {
        try
        {
            var result = await mediator.Send(new GetShowByIdQuery(id));
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    private static async Task<IResult> GetUpcomingShows(int days = 30, IMediator? mediator = null)
    {
        try
        {
            var result = await mediator!.Send(new GetUpcomingShowsQuery(days));
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    private static async Task<IResult> SearchShows(string keyword, IMediator mediator)
    {
        try
        {
            var result = await mediator.Send(new SearchShowsQuery(keyword));
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    private static async Task<IResult> FilterShows(ShowFilterCriteria criteria, IMediator mediator)
    {
        try
        {
            var result = await mediator.Send(new FilterShowsCommand(criteria));
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    private static async Task<IResult> GetMostViewed(IMediator mediator)
    {
        try
        {
            var result = await mediator.Send(new GetMostViewedShowsQuery());
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    // Auth endpoints
    private static async Task<IResult> Register(UserRegistrationRequest request, IMediator mediator)
    {
        try
        {
            var result = await mediator.Send(new RegisterUserCommand(request));
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    private static async Task<IResult> Login(UserLoginRequest request, IMediator mediator)
    {
        try
        {
            var result = await mediator.Send(new LoginUserCommand(request));
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    private static async Task<IResult> OAuthLogin(OAuthRequest request, IMediator mediator)
    {
        try
        {
            var result = await mediator.Send(new OAuthLoginCommand(request));
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    // Favorites endpoints
    private static async Task<IResult> GetUserFavorites(ClaimsPrincipal user, IMediator mediator)
    {
        try
        {
            var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var result = await mediator.Send(new GetUserFavoritesQuery(userId));
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    private static async Task<IResult> AddFavorite(Guid showId, ClaimsPrincipal user, IMediator mediator)
    {
        try
        {
            var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            await mediator.Send(new AddFavoriteCommand(userId, showId));
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    private static async Task<IResult> RemoveFavorite(Guid showId, ClaimsPrincipal user, IMediator mediator)
    {
        try
        {
            var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            await mediator.Send(new RemoveFavoriteCommand(userId, showId));
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    // Review endpoints
    private static async Task<IResult> CreateReview(CreateReviewRequest request, ClaimsPrincipal user, IMediator mediator)
    {
        try
        {
            var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var result = await mediator.Send(new CreateReviewCommand(userId, request));
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    // Admin endpoints
    private static async Task<IResult> ApproveReview(Guid reviewId, IMediator mediator)
    {
        try
        {
            await mediator.Send(new ApproveReviewCommand(reviewId));
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
