#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Tests.Serialization
{
    [TestFixture]
    public sealed class AotUtilsTests : TestFixtureBase
    {
        /// <summary>
        ///     This test executes on JIT, so property should be <see langword="false" />.
        ///     The test ensures that no AOT-specified logic will be used on JIT.
        /// </summary>
        [Test]
        public void IsNotJitShouldBeFalse()
        {
            Assert.IsFalse(AotUtils.IsNoJit);
        }
    }
}