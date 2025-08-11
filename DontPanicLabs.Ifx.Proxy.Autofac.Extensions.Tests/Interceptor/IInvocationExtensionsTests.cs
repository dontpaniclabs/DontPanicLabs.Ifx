using DontPanicLabs.Ifx.Proxy.Autofac.Extensions.Interceptor;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;
using Moq;
using Shouldly;
using IInvocation = Castle.DynamicProxy.IInvocation;

namespace DontPanicLabs.Ifx.Proxy.Autofac.Extensions.Tests.Interceptor;

[TestClass]
[TestCategoryCI]
public class IInvocationExtensionsTests
{
    [TestMethod]
    public void GetFirstArgument_InvocationWithSingleArgument_ShouldReturnArgument()
    {
        var invocation = new Mock<IInvocation>();
        const string testArg = "test argument";
        _ = invocation.SetupGet(i => i.Arguments).Returns([testArg]);

        var result = invocation.Object.GetFirstArgument<string>();

        result.ShouldBe(testArg);
    }

    [TestMethod]
    public void GetFirstArgument_InvocationWithSingleWrongTypeArgument_ShouldThrow()
    {
        var invocation = new Mock<IInvocation>();
        _ = invocation.SetupGet(i => i.Arguments).Returns([42]);

        var exception = Should.Throw<ArgumentException>(() => invocation.Object.GetFirstArgument<string>());

        const string expectedMessage = "The first argument must be of type 'String'.";

        exception.Message.ShouldBe(expectedMessage);
    }

    [TestMethod]
    public void GetFirstArgument_InvocationWithNoArguments_ShouldThrow()
    {
        var invocation = new Mock<IInvocation>();
        _ = invocation.SetupGet(i => i.Arguments).Returns([]);

        var exception = Should.Throw<ArgumentException>(() => invocation.Object.GetFirstArgument<string>());

        const string expectedMessage = "Invocation must have at least one argument.";

        exception.Message.ShouldBe(expectedMessage);
    }

    [TestMethod]
    public void GetTaskResultType_InvocationWithTaskReturnType_ShouldReturnResultType()
    {
        var invocation = new Mock<IInvocation>();

        _ = invocation
            .SetupGet(i => i.Method)
            .Returns(GetType().GetMethod(nameof(StringTaskMethod))!);

        var resultType = invocation.Object.GetTaskResultType();

        resultType.ShouldBe(typeof(string));
    }

    [DataTestMethod]
    [DataRow(nameof(IntMethod))]
    [DataRow(nameof(TaskMethod))]
    public void GetTaskResultType_InvocationWithoutGenericTask_ShouldThrow(string methodName)
    {
        var invocation = new Mock<IInvocation>();

        _ = invocation
            .SetupGet(i => i.Method)
            .Returns(GetType().GetMethod(methodName)!);

        var exception = Should.Throw<ArgumentException>(() => invocation.Object.GetTaskResultType());

        var expectedMessage = $"Method '{methodName}' must return a Task<T>.";

        exception.Message.ShouldBe(expectedMessage);
    }

    [TestMethod]
    public void IsTaskReturning_InvocationWithTaskReturnType_ShouldReturnTrue()
    {
        var invocation = new Mock<IInvocation>();

        _ = invocation
            .SetupGet(i => i.Method)
            .Returns(GetType().GetMethod(nameof(StringTaskMethod))!);

        var result = invocation.Object.IsTaskReturning();

        result.ShouldBeTrue();
    }

    [TestMethod]
    public void IsTaskReturning_InvocationWithoutTaskReturnType_ShouldReturnFalse()
    {
        var invocation = new Mock<IInvocation>();

        _ = invocation
            .SetupGet(i => i.Method)
            .Returns(GetType().GetMethod(nameof(IntMethod))!);

        var result = invocation.Object.IsTaskReturning();

        result.ShouldBeFalse();
    }

    public Task TaskMethod() => Task.CompletedTask;
    public int IntMethod() => 42;
    public Task<string> StringTaskMethod() => Task.FromResult("test");
}