using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Channels;

namespace Ejdb2
{
    public class JQLAsync
    {
        private readonly JQL _query;
        private readonly Channel<(long Id, string Json)> _channel =
            Channel.CreateUnbounded<(long Id, string Json)>();

        public JQLAsync(JQL query)
        {
            _query = query;
        }

        public IAsyncEnumerable<(long Id, string Json)> Execute(CancellationToken token = default)
        {
            _query.OnNextRecord += OnNextRecord;
            _query.OnCompleted += OnComplete;

            _query.Execute();

            _query.OnCompleted -= OnComplete;
            _query.OnNextRecord -= OnNextRecord;

            return _channel.Reader.ReadAllAsync(token);
        }

        private void OnNextRecord(object sender, NextRecordEventArgs e)
            => _channel.Writer.TryWrite((e.Id, e.Json));

        private void OnComplete(object sender, EventArgs e)
            => _channel.Writer.Complete();
    }
}
