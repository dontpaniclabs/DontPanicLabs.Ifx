namespace DontPanicLabs.Ifx.Tests.Shared.Attributes;

/// <summary>
/// This attribute marks tests that can be run in any environment.
/// </summary>
public sealed class TestCategoryCI() : TestCategoryAttributeBase(TestCategoryType.CI);