using Geneirodan.MediatR.Behaviors;
using Geneirodan.SampleApi.Application.Commands;
using JetBrains.Annotations;
using Serilog.Events;
using Serilog.Sinks.TestCorrelator;
using Shouldly;

namespace Geneirodan.MediatR.Tests.Behaviors;

[TestSubject(typeof(UnhandledExceptionBehavior<,>))]
public sealed class UnhandledExceptionBehaviorTests(ApiFactory factory) : PipelineTest(factory)
{
    [Fact]
    public async Task UnhandledExceptionBehavior_ShouldLogException()
    {
        using (TestCorrelator.CreateContext())
        {
            var command = new Command(ShouldFail: true);
            await Should.ThrowAsync<Exception>(async () =>
                await Sender.Send(command, TestContext.Current.CancellationToken));
            var events = TestCorrelator.GetLogEventsFromCurrentContext();
            var entry = events.FirstOrDefault(x => string.Equals(
                    a: x.MessageTemplate.Text,
                    b: "Request: Unhandled Exception for Request {RequestName}",
                    comparisonType: StringComparison.Ordinal
                )
            );
            entry.ShouldNotBeNull();
            entry.Properties.ShouldContainKeyAndValue("RequestName", new ScalarValue(nameof(Command)));
            entry.Exception.ShouldNotBeNull();
            entry.Exception.Message.ShouldBeEquivalentTo("SomeSortOfError");
        }
    }
}