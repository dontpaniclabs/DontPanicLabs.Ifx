using DontPanicLabs.Ifx.Tests.Shared.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DontPanicLabs.Ifx.Tests.Shared.Tests;

[TestClass]
[TestCategoryCI]
public class TestCategoryAttributeTests
{
    [TestMethod]
    public void TestCategoryAttributeBase_CategoryIsUndefined_ShouldThrowException()
    {
        var result = Assert.ThrowsException<ArgumentException>(() => new UndefinedTestCategoryAttribute(42));

        Assert.AreEqual("The test category 42 is not defined.", result.Message);
    }

    [TestMethod]
    public void TestCategoryCI_ShouldAddOnlyTheCICategory()
    {
        var testCategoryCI = new TestCategoryCI();

        Assert.IsTrue(testCategoryCI.TestCategories.Count == 1);
        Assert.AreEqual("CI", testCategoryCI.TestCategories.First());
    }

    [TestMethod]
    public void TestCategoryLocal_ShouldAddOnlyTheLocalCategory()
    {
        var testCategoryLocal = new TestCategoryLocal();

        Assert.IsTrue(testCategoryLocal.TestCategories.Count == 1);
        Assert.AreEqual("Local", testCategoryLocal.TestCategories.First());
    }

    private class UndefinedTestCategoryAttribute(int i) : TestCategoryAttributeBase((TestCategoryType)i);
}