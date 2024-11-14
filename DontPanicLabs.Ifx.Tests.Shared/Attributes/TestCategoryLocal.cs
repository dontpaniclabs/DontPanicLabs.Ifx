namespace DontPanicLabs.Ifx.Tests.Shared.Attributes;

/// <summary>
/// This attribute marks tests that should only be run in a local environment.
/// </summary>
public sealed class TestCategoryLocal() : TestCategoryAttributeBase(TestCategoryType.Local);