using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Ejdb2.Tests
{
    public class EJDB2Tests
    {
        [Fact]
        public void Ctor_NullArgument_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new EJDB2(null));
        }
    }
}
