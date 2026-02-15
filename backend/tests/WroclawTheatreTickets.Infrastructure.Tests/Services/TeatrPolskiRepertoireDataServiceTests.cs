namespace WroclawTheatreTickets.Infrastructure.Tests.Services;

using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WroclawTheatreTickets.Application.Contracts.Dtos;
using WroclawTheatreTickets.Infrastructure.Configuration;
using WroclawTheatreTickets.Infrastructure.Services;
using Xunit;

public class TeatrPolskiRepertoireDataServiceTests
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<TheatreApiConfiguration> _config;
    private readonly ILogger<TeatrPolskiRepertoireDataService> _logger;
    private readonly TeatrPolskiRepertoireDataService _service;

    public TeatrPolskiRepertoireDataServiceTests()
    {
        _httpClientFactory = A.Fake<IHttpClientFactory>();
        _config = A.Fake<IOptions<TheatreApiConfiguration>>();
        _logger = A.Fake<ILogger<TeatrPolskiRepertoireDataService>>();

        var configValue = new TheatreApiConfiguration
        {
            Url = "https://www.teatrpolski.wroc.pl/api/repertoire?lang=pl",
            TimeoutSeconds = 30
        };
        A.CallTo(() => _config.Value).Returns(configValue);

        _service = new TeatrPolskiRepertoireDataService(_httpClientFactory, _config, _logger);
    }

    [Fact]
    public async Task FetchAndMapRepertoireAsync_WithValidResponse_ReturnsMappedShowEntities()
    {
        // Arrange
        var theatreId = Guid.NewGuid();
        var mockHttpClient = new HttpClient(new MockHttpMessageHandler(CreateValidApiResponse()));
        A.CallTo(() => _httpClientFactory.CreateClient(A<string>._)).Returns(mockHttpClient);

        // Act
        var shows = await _service.FetchAndMapRepertoireAsync(theatreId, CancellationToken.None);

        // Assert
        Assert.NotEmpty(shows);
        Assert.All(shows, show =>
        {
            Assert.Equal(theatreId, show.TheatreId);
            Assert.NotNull(show.Title);
            Assert.NotEqual(show.Id, Guid.Empty);
        });
    }

    [Fact]
    public async Task FetchAndMapRepertoireAsync_WithHiddenEvents_FiltersThemOut()
    {
        // Arrange
        var theatreId = Guid.NewGuid();
        var response = new RepertoireApiResponse
        {
            Events =
            [
                CreateValidEvent(hidden: false),
                CreateValidEvent(hidden: true),
                CreateValidEvent(hidden: false)
            ]
        };
        var mockHttpClient = new HttpClient(new MockHttpMessageHandler(response));
        A.CallTo(() => _httpClientFactory.CreateClient(A<string>._)).Returns(mockHttpClient);

        // Act
        var shows = await _service.FetchAndMapRepertoireAsync(theatreId, CancellationToken.None);

        // Assert
        // Should have 2 shows (hidden event filtered out)
        Assert.Equal(2, shows.Count);
    }

    [Fact]
    public async Task FetchAndMapRepertoireAsync_WithMissingRequiredFields_SkipsThatEvent()
    {
        // Arrange
        var theatreId = Guid.NewGuid();
        var response = new RepertoireApiResponse
        {
            Events =
            [
                CreateValidEvent(),
                CreateEventWithMissingTitle(),
                CreateValidEvent()
            ]
        };
        var mockHttpClient = new HttpClient(new MockHttpMessageHandler(response));
        A.CallTo(() => _httpClientFactory.CreateClient(A<string>._)).Returns(mockHttpClient);

        // Act
        var shows = await _service.FetchAndMapRepertoireAsync(theatreId, CancellationToken.None);

        // Assert
        // Should have 2 shows (invalid event skipped)
        Assert.Equal(2, shows.Count);
    }

    [Fact]
    public async Task FetchAndMapRepertoireAsync_WithNoEvents_ReturnsEmptyList()
    {
        // Arrange
        var theatreId = Guid.NewGuid();
        var response = new RepertoireApiResponse { Events = [] };
        var mockHttpClient = new HttpClient(new MockHttpMessageHandler(response));
        A.CallTo(() => _httpClientFactory.CreateClient(A<string>._)).Returns(mockHttpClient);

        // Act
        var shows = await _service.FetchAndMapRepertoireAsync(theatreId, CancellationToken.None);

        // Assert
        Assert.Empty(shows);
    }

    [Fact]
    public async Task FetchAndMapRepertoireAsync_WithHttpError_ThrowsHttpRequestException()
    {
        // Arrange
        var theatreId = Guid.NewGuid();
        var mockHandler = new MockHttpMessageHandler(statusCode: System.Net.HttpStatusCode.InternalServerError);
        var mockHttpClient = new HttpClient(mockHandler);
        A.CallTo(() => _httpClientFactory.CreateClient(A<string>._)).Returns(mockHttpClient);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(
            () => _service.FetchAndMapRepertoireAsync(theatreId, CancellationToken.None));
    }

    // Helper methods
    private RepertoireApiResponse CreateValidApiResponse()
    {
        return new RepertoireApiResponse
        {
            Events =
            [
                CreateValidEvent(theatreName: "Opera Test"),
                CreateValidEvent(theatreName: "Ballet Test"),
                CreateValidEvent(theatreName: "Play Test")
            ]
        };
    }

    private RepertoireEventDto CreateValidEvent(
        string theatreName = "Test Show",
        bool hidden = false,
        string categoryTitle = "play",
        string ageTitle = "0+")
    {
        return new RepertoireEventDto
        {
            RepertoireEventId = Guid.NewGuid().ToString(),
            Title = theatreName,
            Date = DateTime.Now.AddDays(7),
            Duration = 120,
            HiddenFromRepertoire = hidden,
            PaymentUrl = "https://example.com/tickets",
            PaymentDisabled = false,
            Stage = new StageDto
            {
                Building = new BuildingDto { Name = "Main Stage" }
            },
            RepertoireCategories =
            [
                new RepertoireCategoryDto { Title = categoryTitle }
            ],
            AgeCategories =
            [
                new AgeCategoryDto { Title = ageTitle }
            ],
            AdditionalProps = null
        };
    }

    private RepertoireEventDto CreateEventWithMissingTitle()
    {
        var evt = CreateValidEvent();
        evt.Title = string.Empty;
        return evt;
    }

    // Mock HTTP message handler for testing
    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly RepertoireApiResponse? _response;
        private readonly System.Net.HttpStatusCode _statusCode;
        private readonly bool _shouldDelay;

        public MockHttpMessageHandler(
            RepertoireApiResponse? response = null,
            System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.OK,
            bool shouldDelay = false)
        {
            _response = response;
            _statusCode = statusCode;
            _shouldDelay = shouldDelay;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (_shouldDelay)
            {
                await Task.Delay(5000, cancellationToken);
            }

            if (_statusCode != System.Net.HttpStatusCode.OK)
            {
                return new HttpResponseMessage(_statusCode);
            }

            var json = System.Text.Json.JsonSerializer.Serialize(_response);
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
        }
    }
}
