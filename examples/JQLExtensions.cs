using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Ejdb2.Examples
{
    public static class JQLExtensions
    {
        public static IAsyncEnumerable<(long Id, string Json)> ToAsyncEnumerable(this JQL query, CancellationToken token = default)
            => new JQLAsync(query).Execute(token);
    }
}
