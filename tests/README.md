# WroclawTheatreTickets - Backend Unit Tests

This directory contains comprehensive unit tests for the WroclawTheatreTickets backend, covering Domain, Application, and Infrastructure layers.

## Test Projects

### WroclawTheatreTickets.Domain.Tests
Tests for domain entities and business logic without external dependencies.

**Coverage:**
- User entity (Create, CreateOAuth, validation)
- Show entity (IncrementViewCount, UpdateRating)
- Theatre entity (Create factory method)
- UserInteraction entities (Favorite, Review, ViewHistory, Notification)

### WroclawTheatreTickets.Application.Tests
Tests for CQRS command/query handlers using mocked dependencies.

**Coverage:**
- FilterShowsCommandHandler (filtering logic)
- RegisterUserCommandHandler (user registration)
- LoginUserCommandHandler (authentication)

### WroclawTheatreTickets.Infrastructure.Tests
Tests for services and repositories with InMemory database for data access tests.

**Coverage:**
- AuthenticationService (JWT, password hashing)
- TheatreRepository (CRUD operations)
- ShowRepository (CRUD, filtering)

## Technologies Used

- **xUnit**: Test framework
- **FakeItEasy**: Mocking library for isolating dependencies
- **AutoFixture**: Automatic test data generation
- **EF Core InMemory**: In-memory database for repository tests

## Running Tests

```bash
# Run all tests
dotnet test ../WroclawTheatreTickets.slnx

# Run specific test project
dotnet test WroclawTheatreTickets.Domain.Tests/

# Run with detailed output
dotnet test ../WroclawTheatreTickets.slnx --verbosity normal

# Run and generate coverage report (if configured)
dotnet test --collect:"XPlat Code Coverage"
```

## Test Statistics

- **Total Tests**: 66
- **Domain**: 29 tests
- **Application**: 12 tests
- **Infrastructure**: 25 tests

All tests follow the AAA (Arrange-Act-Assert) pattern and test both success and failure scenarios.

## Adding New Tests

When adding new tests, follow these guidelines:

1. **Naming Convention**: `{ClassUnderTest}Tests.cs`
2. **Test Method Naming**: `MethodName_Scenario_ExpectedBehavior`
3. **Use AAA Pattern**: Arrange, Act, Assert sections
4. **Mock Dependencies**: Use FakeItEasy for external dependencies
5. **Test Data**: Use AutoFixture for generating test data
6. **Isolation**: Each test should be independent and not rely on other tests

### Example Test Structure

```csharp
[Fact]
public async Task MethodName_WithValidInput_ShouldReturnExpectedResult()
{
    // Arrange
    var dependency = A.Fake<IDependency>();
    var sut = new SystemUnderTest(dependency);
    var input = new Input();
    
    A.CallTo(() => dependency.Method()).Returns(expectedValue);

    // Act
    var result = await sut.MethodName(input);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(expectedValue, result.Property);
    A.CallTo(() => dependency.Method()).MustHaveHappenedOnceExactly();
}
```

## Continuous Integration

These tests are designed to be run in CI/CD pipelines. Ensure all tests pass before merging to main branch.

## Coverage Goals

Current coverage focuses on:
- ✅ Critical business logic
- ✅ Data access layer
- ✅ Authentication and authorization
- ✅ Entity factory methods
- ✅ Command handlers

Future improvements could include:
- Additional command/query handlers
- Edge cases and error scenarios
- Integration tests
- Performance tests
