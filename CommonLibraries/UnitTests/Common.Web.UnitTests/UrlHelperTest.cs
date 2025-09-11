namespace Common.Web.UnitTests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    [TestFixture]
    public class UrlHelperTest
    {
        [TestCaseSource(nameof(TestToAbsoluteUrlCases), new object[] { nameof(TestToAbsoluteUrl) })]
        public string TestToAbsoluteUrl(string baseurl, string relativeurl, bool useOnlyDomain)
        {
            return UrlHelper.ToAbsoluteUrl(baseurl, relativeurl, useOnlyDomain);
        }

        public static IEnumerable<TestCaseData> TestToAbsoluteUrlCases(string methodCaller)
        {
            yield return new TestCaseData(null, null, false).Returns(null).SetName($"{methodCaller} (both url null)");
            yield return new TestCaseData(null, null, true).Returns(null).SetName($"{methodCaller} (both url null and useOnlyDomain)");
            yield return new TestCaseData(string.Empty, string.Empty, false).Returns(string.Empty).SetName($"{methodCaller} (both url empty)");
            yield return new TestCaseData(string.Empty, string.Empty, true).Returns(string.Empty).SetName($"{methodCaller} (both url empty and useOnlyDomain)");
            yield return new TestCaseData("aaa", null, false).Returns("aaa").SetName($"{methodCaller} (relative url null)");
            yield return new TestCaseData("aaa", null, true).Returns("aaa").SetName($"{methodCaller} (relative url null and useOnlyDomain)");
            yield return new TestCaseData("aaa", string.Empty, false).Returns("aaa").SetName($"{methodCaller} (relative url empty)");
            yield return new TestCaseData("aaa", string.Empty, true).Returns("aaa").SetName($"{methodCaller} (relative url empty and useOnlyDomain)");
            yield return new TestCaseData(null, "bbb", false).Returns("bbb").SetName($"{methodCaller} (base url null)");
            yield return new TestCaseData(null, "bbb", true).Returns("bbb").SetName($"{methodCaller} (base url null and useOnlyDomain)");
            yield return new TestCaseData(string.Empty, "bbb", false).Returns("bbb").SetName($"{methodCaller} (base url empty)");
            yield return new TestCaseData(string.Empty, "bbb", true).Returns("bbb").SetName($"{methodCaller} (base url empty and useOnlyDomain)");
            yield return new TestCaseData(@"http://test/", "ftp://aaa", false).Returns("ftp://aaa").SetName($"{methodCaller} (relative url is absolute)");
            yield return new TestCaseData(@"http://test/", "ftp://aaa", true).Returns("ftp://aaa").SetName($"{methodCaller} (relative url is absolute and useOnlyDomain)");

            yield return new TestCaseData(@"http://test", "aaa", false).Returns("http://aaa").SetName($"{methodCaller} (no domain)");
            yield return new TestCaseData(@"http://test", "aaa", true).Returns("http://aaa").SetName($"{methodCaller} (no domain and useOnlyDomain)");

            yield return new TestCaseData(@"http://test/aaa/", "abc/123", false).Returns("http://test/aaa/abc/123").SetName($"{methodCaller} (base case)");
            yield return new TestCaseData(@"http://test/aaa", "abc/123", false).Returns("http://test/abc/123").SetName($"{methodCaller} (base case no / ending in base)");
            yield return new TestCaseData(@"http://test/aaa/", "/abc/123", false).Returns("http://test/aaa/abc/123").SetName($"{methodCaller} (base case with / in relative)");
            yield return new TestCaseData(@"http://test/aaa", "/abc/123", false).Returns("http://test/abc/123").SetName($"{methodCaller} (base case no / ending in base but / in relative)");

            yield return new TestCaseData(@"http://test/aaa/bbb/", "abc/123", true).Returns("http://test/abc/123").SetName($"{methodCaller} (base case and useOnlyDomain)");
            yield return new TestCaseData(@"http://test/aaa/bbb", "abc/123", true).Returns("http://test/abc/123").SetName($"{methodCaller} (base case no / ending in base and useOnlyDomain)");
            yield return new TestCaseData(@"http://test/aaa/bbb/", "/abc/123", true).Returns("http://test/abc/123").SetName($"{methodCaller} (base case with / in relative and useOnlyDomain)");
            yield return new TestCaseData(@"http://test/aaa/bbb", "/abc/123", true).Returns("http://test/abc/123").SetName($"{methodCaller} (base case no / ending in base but / in relative and useOnlyDomain)");

            yield return new TestCaseData(@"http://test/aaa/bbb/ccc/ddd/", "../../abc/123", false).Returns("http://test/aaa/bbb/abc/123").SetName($"{methodCaller} (with relative ..)");
            yield return new TestCaseData(@"http://test/aaa/bbb/ccc/ddd/", "../../abc/123", true).Returns("http://abc/123").SetName($"{methodCaller} (with relative .. and useOnlyDomain)");

            yield return new TestCaseData(@"http://test/aaa/", "../../../../../../../abc/123", false).Returns("http://abc/123").SetName($"{methodCaller} (with too many relative ..)");
            yield return new TestCaseData(@"http://test/aaa/", "../../../../../../../abc/123", true).Returns("http://abc/123").SetName($"{methodCaller} (with too many relative .. and useOnlyDomain)");

            yield return new TestCaseData("aaa", "bbb", false).Returns("aaa/bbb").SetName($"{methodCaller} (no Scheme)");
            yield return new TestCaseData("aaa", "bbb", true).Returns("aaa/bbb").SetName($"{methodCaller} (no Scheme and useOnlyDomain)");

            yield return new TestCaseData("aaa/ccc/", "../bbb", false).Returns("aaa/bbb").SetName($"{methodCaller} (no Scheme with relative ..)");
            yield return new TestCaseData("aaa/ccc/", "../bbb", true).Returns("aaa/bbb").SetName($"{methodCaller} (no Scheme with relative .. and useOnlyDomain)");

            yield return new TestCaseData("aaa/ccc/ddd/", "../../../../../../bbb", false).Returns("bbb").SetName($"{methodCaller} (no Scheme with too many relative ..)");
            yield return new TestCaseData("aaa/ccc/ddd/", "../../../../../../bbb", true).Returns("bbb").SetName($"{methodCaller} (no Scheme with too many relative .. and useOnlyDomain)");
        }
    }
}