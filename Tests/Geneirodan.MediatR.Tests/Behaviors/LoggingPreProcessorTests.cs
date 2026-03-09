using Geneirodan.Abstractions.Domain;
using Geneirodan.MediatR.Behaviors;
using Geneirodan.SampleApi.Application.Commands;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Serilog.Events;
using Serilog.Sinks.TestCorrelator;
using Shouldly;

namespace Geneirodan.MediatR.Tests.Behaviors;

[TestSubject(typeof(LoggingPreProcessor<>))]
public sealed class LoggingPreProcessorTests(ApiFactory factory) : PipelineTest(factory)
{
    [Fact]
    public async Task Send_ShouldLogRequest()
    {
        using (TestCorrelator.CreateContext())
        {
            var command = new Command(ShouldFail: false);
            await Sender.Send(command, TestContext.Current.CancellationToken);
            var events = TestCorrelator.GetLogEventsFromCurrentContext();
            var entry = events.FirstOrDefault(x => string.Equals(
                a: x.MessageTemplate.Text,
                b: "Processing {RequestName}",
                comparisonType: StringComparison.Ordinal
            ));
            entry.ShouldNotBeNull();
            entry.Properties.ShouldContainKeyAndValue("RequestName", new ScalarValue(nameof(Command)));
            entry.Properties.ShouldContainKey("Request");
            entry.Properties["Request"].ToString().ShouldBeEquivalentTo("Command { ShouldFail: False }");
        }
    }

    [Fact]
    public async Task Send_ShouldLogRequestWithUserId()
    {
        using (TestCorrelator.CreateContext())
        {
            var userId = Guid.NewGuid();
            Scope.ServiceProvider.GetRequiredService<Mock<IUser>>().Setup(x => x.Id).Returns(userId);
            var command = new Command(ShouldFail: false);
            await Sender.Send(command, TestContext.Current.CancellationToken);
            var events = TestCorrelator.GetLogEventsFromCurrentContext();
            var entry = events.FirstOrDefault(x => string.Equals(
                a: x.MessageTemplate.Text,
                b: "Processing {RequestName} with user {UserId}",
                comparisonType: StringComparison.Ordinal
            ));
            entry.ShouldNotBeNull();
            entry.Properties.ShouldContainKeyAndValue("RequestName", new ScalarValue(nameof(Command)));
            entry.Properties.ShouldContainKeyAndValue("UserId", new ScalarValue(userId));
            entry.Properties.ShouldContainKey("Request");
            entry.Properties["Request"].ToString().ShouldBeEquivalentTo("Command { ShouldFail: False }");
        }
    }
}