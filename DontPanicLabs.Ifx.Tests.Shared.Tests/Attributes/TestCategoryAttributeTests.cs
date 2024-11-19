using System.Reflection;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;

namespace DontPanicLabs.Ifx.Tests.Shared.Tests.Attributes;

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

    [TestMethod]
    public void TestCategories_ShouldBeRequiredForEveryTestInEveryTestAssembly()
    {
        var testAssemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => a.GetName().Name!.Contains(".Tests"));

        // Grab all the test classes that don't have a [TestCategory] at the class level.
        var uncategorizedTestClasses = testAssemblies
            .SelectMany(assembly => assembly
                .GetTypes()
                .Where(type => type.HasOrInheritsAttribute<TestClassAttribute>()))
            .Where(testClass => !testClass.HasTestCategory())
            .ToArray();

        // From those classes, determine all the test methods that don't have a [TestCategory] at the method level.
        var uncategorizedTestMethodStrings = uncategorizedTestClasses
            .SelectMany(testClass => testClass
                .GetMethods()
                .Where(m => m.HasOrInheritsAttribute<TestMethodAttribute>() && !m.HasTestCategory()))
            .Select(methodInfo => $"{methodInfo.DeclaringType!.FullName}.{methodInfo.Name}")
            .ToList();

        // We should have one guaranteed failure due to test cases created in this file
        var indexOfExpectedFail = uncategorizedTestMethodStrings.IndexOf(
            "DontPanicLabs.Ifx.Tests.Shared.Tests.Attributes.TestCategoryAttributeTests+TestClassWithoutCategory.MethodWithoutCategory"
        );

        Assert.IsTrue(indexOfExpectedFail >= 0, "Expected test method was not found.");

        uncategorizedTestMethodStrings.RemoveAt(indexOfExpectedFail);

        // If there are any remaining test methods without a category, fail the test and list all the failures.
        Assert.IsFalse(uncategorizedTestMethodStrings.Any(),
            $"The following methods need a test category at the method or class level:{Environment.NewLine}" +
            string.Join(Environment.NewLine, uncategorizedTestMethodStrings));
    }

    [TestMethod]
    [DataRow(typeof(TestClassWithCategory), true, DisplayName = "Should be true if class has a category")]
    [DataRow(typeof(DerivedClassWithoutCategory), true, DisplayName = "Should be true if base class has a category")]
    [DataRow(typeof(TestClassWithoutCategory), false, DisplayName = "Should be false if class is missing a category")]
    public void HasTestCategory_Classes(Type type, bool expected)
    {
        Assert.AreEqual(expected, type.HasTestCategory());
    }

    [DataTestMethod]
    [DataRow(typeof(TestClassWithCategory), nameof(TestClassWithCategory.MethodWithoutCategory), true,
        DisplayName = "Should be true if only class has a category")]
    [DataRow(typeof(DerivedClassWithoutCategory), nameof(DerivedClassWithoutCategory.MethodWithoutCategory), true,
        DisplayName = "Should be true if only the base class has a category")]
    [DataRow(typeof(TestClassWithoutCategory), nameof(TestClassWithoutCategory.MethodWithCategory), true,
        DisplayName = "Should be true if only the method has a category")]
    [DataRow(typeof(TestClassWithoutCategory), nameof(TestClassWithoutCategory.MethodWithoutCategory), false,
        DisplayName = "Should be false if the method and class have no category")]
    public void HasTestCategory_Methods(Type type, string methodName, bool expected)
    {
        var methodInfo = type.GetMethod(methodName);

        Assert.AreEqual(expected, methodInfo!.HasTestCategory());
    }

    [TestCategoryLocal]
    private abstract class BaseClassWithCategory;

    [TestClass]
    private class DerivedClassWithoutCategory : BaseClassWithCategory
    {
        [TestMethod]
        public void MethodWithoutCategory()
        {
            Assert.IsTrue(true);
        }
    }

    [TestClass]
    [TestCategoryLocal]
    private class TestClassWithCategory
    {
        [TestMethod]
        public void MethodWithoutCategory()
        {
            Assert.IsTrue(true);
        }
    }

    [TestClass]
    private class TestClassWithoutCategory
    {
        [TestMethod]
        [TestCategoryLocal]
        public void MethodWithCategory()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void MethodWithoutCategory()
        {
            Assert.IsTrue(true);
        }
    }
}

file static class MemberInfoExtensions
{
    public static bool HasTestCategory(this MemberInfo memberInfo)
    {
        var memberInfoHasTestCategory = memberInfo.HasOrInheritsAttribute<TestCategoryAttributeBase>();

        if (memberInfo is Type { IsClass: true })
        {
            return memberInfoHasTestCategory;
        }

        return memberInfoHasTestCategory || memberInfo.DeclaringType!.HasTestCategory();
    }

    public static bool HasOrInheritsAttribute<T>(this MemberInfo memberInfo) where T : Attribute
    {
        return memberInfo.GetCustomAttributes<T>(true).Any();
    }
}

file class UndefinedTestCategoryAttribute(int i) : TestCategoryAttributeBase((TestCategoryType)i);