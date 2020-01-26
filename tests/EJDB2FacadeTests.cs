using System;
using System.Collections.Generic;
using System.Text;
using Ejdb2.Native;
using Xunit;

namespace Ejdb2.Tests
{
    public class EJDB2FacadeTests
    {
        [Fact]
        public void Init_ShouldNotThrowAnException()
        {
            var inst = EJDB2Facade.Instance;
            GC.KeepAlive(inst);
        }

        [Fact]
        public void GetVersion_MustReturnNonEmptyValue()
        {
            var version = EJDB2Facade.Instance.GetVersion();
            Assert.NotEqual(new Version(), version);
        }

        [Fact]
        public void Open_OptionsIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => EJDB2Facade.Instance.Open(null));
        }

        [Fact]
        public void Open_PathIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => EJDB2Facade.Instance.Open(new EJDB2Options(null)));
        }

        [Fact]
        public void Open_PathIsEmpty_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => EJDB2Facade.Instance.Open(new EJDB2Options("")));
        }
    }
}
