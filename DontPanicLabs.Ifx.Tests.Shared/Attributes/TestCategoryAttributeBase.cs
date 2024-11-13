namespace DontPanicLabs.Ifx.Tests.Shared.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly)]
public class TestCategoryAttributeBase : Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryBaseAttribute
{
    protected TestCategoryAttributeBase(TestCategoryType testCategory)
    {
        if (!Enum.IsDefined(testCategory))
        {
            throw new ArgumentException($"The test category {testCategory} is not defined.");
        }

        TestCategories = [testCategory.ToString()];
    }

    public override IList<string> TestCategories { get; }
}