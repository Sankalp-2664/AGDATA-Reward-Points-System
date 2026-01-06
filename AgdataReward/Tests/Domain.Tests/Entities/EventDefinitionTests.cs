using Domain.Entities.Event;
using FluentAssertions;

namespace Tests.Domain.Tests.Entities;

public class EventDefinitionTests
{
    [Fact]
    public void Constructor_Should_Create_EventDefinition_When_ValidArguments()
    {
        // Arrange
        var id = Guid.NewGuid();
        var code = "HACKATHON2025";
        var title = "Annual Hackathon 2025";

        // Act
        var eventDef = new EventDefinition(id, code, title);

        // Assert
        eventDef.Id.Should().Be(id);
        eventDef.Code.Should().Be(code);
        eventDef.Title.Should().Be(title);
        eventDef.Instances.Should().BeEmpty();
        eventDef.RewardRules.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_Should_Throw_When_IdIsEmpty()
    {
        // Arrange
        var id = Guid.Empty;
        var code = "HACK";
        var title = "Title";

        // Act
        Action act = () => new EventDefinition(id, code, title);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("Id cannot be empty.*")
           .And.ParamName.Should().Be("id");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_Throw_When_CodeIsInvalid(string code)
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = "Title";

        // Act
        Action act = () => new EventDefinition(id, code, title);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("Code is required.*")
           .And.ParamName.Should().Be("code");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_Throw_When_TitleIsInvalid(string title)
    {
        // Arrange
        var id = Guid.NewGuid();
        var code = "CODE123";

        // Act
        Action act = () => new EventDefinition(id, code, title);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("Title is required.*")
           .And.ParamName.Should().Be("title");
    }

    [Fact]
    public void AddInstance_Should_Add_Instance_To_EventDefinition()
    {
        // Arrange
        var eventDef = new EventDefinition(Guid.NewGuid(), "CODE123", "Some Event");
        var instance = new EventInstance(Guid.NewGuid(), eventDef.Id);

        // Act
        eventDef.AddInstance(instance);

        // Assert
        eventDef.Instances.Should().ContainSingle()
            .Which.Should().Be(instance);
    }

    [Fact]
    public void AddInstance_Should_Throw_When_InstanceIsNull()
    {
        // Arrange
        var eventDef = new EventDefinition(Guid.NewGuid(), "CODE123", "Some Event");

        // Act
        Action act = () => eventDef.AddInstance(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("instance");
    }
}
