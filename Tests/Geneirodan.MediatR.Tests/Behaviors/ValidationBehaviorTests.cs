using Ardalis.Result;
using Geneirodan.MediatR.Behaviors;
using Geneirodan.SampleApi.Application.Commands;
using JetBrains.Annotations;
using Shouldly;

namespace Geneirodan.MediatR.Tests.Behaviors;

[TestSubject(typeof(ValidationBehavior<,>))]
public class ValidationBehaviorTests(ApiFactory factory) : PipelineTest(factory)
{
    [Theory]
    [InlineData("")]
    [InlineData("InvalidEmail")]
    public async Task ValidationBehavior_ShouldReturnInvalid_WhenValidationFails(string email)
    {
        var command = new ValidatedCommand(email);
        var result = await Sender.Send(command, TestContext.Current.CancellationToken);
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldNotBeEmpty();
        result.ValidationErrors.ShouldContain(x => x.Identifier == nameof(command.Email));
    }

    [Theory]
    [InlineData("Valid@email.com")]
    public async Task ValidationBehavior_ShouldReturnSuccess_WhenRequestIsValid(string email)
    {
        var command = new ValidatedCommand(email);
        var result = await Sender.Send(command, TestContext.Current.CancellationToken);
        result.Status.ShouldBe(ResultStatus.Ok);
        result.ValidationErrors.ShouldBeEmpty();
    }
}