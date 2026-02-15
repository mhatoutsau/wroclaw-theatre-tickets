# WroclawTheatreTickets - Backend Unit Tests

This directory contains comprehensive unit tests for the WroclawTheatreTickets backend, covering the Clean Architecture layers: Domain, Application, and Infrastructure.

## Test Projects Organization

The test suite follows the same layer structure as the main source projects for clear separation of concerns.

### WroclawTheatreTickets.Domain.Tests
Tests for domain entities and core business logic without external dependencies.

**Test Files (4 test classes, 29 tests):**
- `Entities/UserTests.cs` - User entity creation, OAuth support, validation
- `Entities/ShowTests.cs` - Show entity, view counts, ratings
- `Entities/TheatreTests.cs` - Theatre entity factory methods
- `Entities/UserInteractionTests.cs` - Favorite, Review, ViewHistory, Notification interactions

**Coverage Focus:**
- Entity creation and validation
- Value object behavior
- Business rule enforcement
- Factory methods and aggregates

### WroclawTheatreTickets.Application.Tests
Tests for CQRS command/query handlers using mocked dependencies (no database).

**Test Files (3 test classes, 12 tests):**
- `UseCases/Shows/Commands/FilterShowsCommandHandlerTests.cs` - Show filtering with multiple criteria
- `UseCases/Users/Commands/RegisterUserCommandHandlerTests.cs` - User registration and validation
- `UseCases/Users/Commands/LoginUserCommandHandlerTests.cs` - Authentication logic

**Coverage Focus:**
- Command/Query handler business logic
- Dependency mocking (repositories, services)
- Validation and error handling
- DTO mapping and transformation

### WroclawTheatreTickets.Infrastructure.Tests
Tests for data access services and repositories using in-memory database.

**Test Files (3 test classes, 25 tests):**
- `Services/AuthenticationServiceTests.cs` - JWT generation, password hashing (BCrypt), token verification
- `Repositories/ShowRepositoryTests.cs` - Show CRUD, filtering, querying
- `Repositories/TheatreRepositoryTests.cs` - Theatre CRUD and retrieval operations

**Coverage Focus:**
- Repository CRUD operations
- Query filtering and projection
- Authentication and security
- Data persistence and EF Core integration

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

**Total: 66 Tests | Pass Rate: 100%**

| Layer | Test Classes | Test Count | Status |
|-------|-------------|-----------|--------|
| Domain | 4 | 29 | ✅ Passing |
| Application | 3 | 12 | ✅ Passing |
| Infrastructure | 3 | 25 | ✅ Passing |
| **TOTAL** | **10** | **66** | **✅ All Passing** |

### Test Execution Times (Average)
- Domain tests: ~69 ms
- Application tests: ~182 ms
- Infrastructure tests: ~1000 ms
- Total runtime: ~1.25 seconds

All tests follow the AAA (Arrange-Act-Assert) pattern and test both success paths and failure scenarios.

## Adding New Tests

When adding new tests, follow these guidelines:

### File Organization
- Create test files in the matching directory structure: `{LayerName}.Tests/{Feature}/{ClassName}Tests.cs`
- For Domain tests: `Entities/`, `ValueObjects/`, etc.
- For Application tests: `UseCases/{Feature}/{CommandOrQuery}/`
- For Infrastructure tests: `Services/`, `Repositories/`

### Naming Conventions
1. **Test Class Name**: `{ClassUnderTest}Tests`
2. **Test Method Name**: `{MethodName}_{Scenario}_{ExpectedBehavior}`
   - Examples: `Handle_WithValidInput_ShouldReturnResult`, `Create_WithNullEmail_ShouldThrowException`
3. **Variable Names**: Use meaningful names (`sut` for System Under Test, `result` for method output)

### Test Structure (AAA Pattern)
```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedBehavior()
{
    // Arrange - Set up test data and mocks
    var dependency = A.Fake<IDependency>();
    var sut = new SystemUnderTest(dependency);
    A.CallTo(() => dependency.Method()).Returns(expectedValue);

    // Act - Execute the method being tested
    var result = await sut.MethodName(input);

    // Assert - Verify the outcome
    Assert.NotNull(result);
    Assert.Equal(expectedValue, result.Property);
    A.CallTo(() => dependency.Method()).MustHaveHappenedOnceExactly();
}
```

### Best Practices
- **Independence**: Each test should be completely independent and not rely on others
- **Isolation**: Domain tests should have no dependencies; Application/Infrastructure tests should mock external services
- **Clarity**: Make test intent obvious; prefer explicit assertions over implicit behaviors
- **Completeness**: Test both happy paths and error scenarios
- **Maintainability**: Keep tests simple and focused on one behavior

### Layer-Specific Testing Guidance

**Domain Tests:**
- No mocks needed - directly instantiate entities
- Focus on business rules and invariants
- Test factory methods and value objects

**Application Tests:**
- Use FakeItEasy to mock repositories and services
- Test CQRS handler logic with various scenarios
- Verify correct delegation to dependencies
- Example imports: `using FakeItEasy; using AutoMapper;`

**Infrastructure Tests:**
- Use EF Core InMemory database for repository tests
- Test actual database operations and queries
- Test service implementations with real logic
- Example: `Microsoft.EntityFrameworkCore.InMemory`

### Example: Complete Test Class

```csharp
namespace WroclawTheatreTickets.Domain.Tests.Entities;

using WroclawTheatreTickets.Domain.Entities;
using Xunit;

public class UserTests
{
    [Fact]
    public void Create_WithValidEmail_ShouldCreateUser()
    {
        // Arrange
        var email = "test@example.com";
        var firstName = "John";
        var lastName = "Doe";

        // Act
        var user = User.Create(email, firstName, lastName);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
        Assert.Equal(firstName, user.FirstName);
    }

    [Fact]
    public void Create_WithoutNames_ShouldCreateUserWithNullNames()
    {
        // Arrange & Act
        var user = User.Create("test@example.com");

        // Assert
        Assert.Null(user.FirstName);
        Assert.Null(user.LastName);
    }
}
```

## Continuous Integration

These tests are designed to run in CI/CD pipelines:

```bash
# Build and test before committing
dotnet build WroclawTheatreTickets.slnx
dotnet test
```

Ensure all tests pass before merging to the main branch.

## Current Test Coverage

**What's Tested:**
- ✅ Critical business logic (entities, domain rules)
- ✅ Data access layer (repositories, queries, filters)
- ✅ Authentication and security (JWT, password hashing)
- ✅ CQRS handlers (commands, proper delegation)
- ✅ Factory methods and entity creation
- ✅ Error scenarios and validation

**Future Test Additions:**
- Favorites and Reviews use cases
- Additional Show queries (GetById, GetByTheatre, etc.)
- User profile queries
- Notification services
- Job processing (background jobs)
- Edge cases and performance tests
- API endpoint integration tests
