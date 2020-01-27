using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ejdb2.Native;

namespace Ejdb2
{
    public sealed class JQL : IDisposable
    {
        private readonly Lazy<EJDB2Handle> _handle;

        private bool _disposed;
        private long _limit;
        private long _skip;
        private bool _explain;
        private StringWriter _explainLog;
        private string _collection;

        private JQL(EJDB2 db, string query, string collection)
        {
            Db = db ?? throw new ArgumentNullException(nameof(db));
            Query = query ?? throw new ArgumentNullException(nameof(query));
            _collection = collection;
            _handle = new Lazy<EJDB2Handle>(() => JQLFacade.Instance.Init(Db.Handle, Query, ref _collection));
        }

        internal EJDB2Handle Handle => _handle.Value;

        public EJDB2 Db { get; }
        public string Query { get; }

        public string Collection
        {
            get => _collection;
            set => _collection = value;
        }

        public JQL WithExplain()
        {
            _explain = true;
            return this;
        }

        public JQL WithNoExplain()
        {
            _explain = false;
            return this;
        }

        public string GetExplainLog() => _explainLog?.ToString();

        public JQL SetSkip(long skip)
        {
            Skip = skip;
            return this;
        }

        public long Skip
        {
            get
            {
                EnsureNotDisposed();
                return _skip > 0 ? _skip : JQLFacade.Instance.GetSkip(_handle.Value);
            }

            set => _skip = value;
        }

        public JQL SetLimit(long limit)
        {
            Limit = limit;
            return this;
        }

        public long Limit
        {
            get
            {
                EnsureNotDisposed();
                return _limit > 0 ? _limit : JQLFacade.Instance.GetLimit(_handle.Value);
            }

            set => _limit = value;
        }

        public JQL SetString(int pos, string val)
        {
            EnsureNotDisposed();
            JQLFacade.Instance.SetString(_handle.Value, pos, null,
                val, JQLFacade.SetStringType.Other);
            return this;
        }

        public JQL SetString(string placeholder, string val)
        {
            EnsureNotDisposed();
            JQLFacade.Instance.SetString(_handle.Value, 0, placeholder,
                val, JQLFacade.SetStringType.Other);
            return this;
        }

        public JQL SetLong(int pos, long val)
        {
            EnsureNotDisposed();
            JQLFacade.Instance.SetLong(_handle.Value, pos, null, val);
            return this;
        }

        public JQL SetLong(string placeholder, long val)
        {
            EnsureNotDisposed();
            JQLFacade.Instance.SetLong(_handle.Value, 0, placeholder, val);
            return this;
        }

        public JQL SetJson(int pos, string json)
        {
            EnsureNotDisposed();
            JQLFacade.Instance.SetString(_handle.Value, pos, null,
                json, JQLFacade.SetStringType.Json);
            return this;
        }

        public JQL SetJson(string placeholder, string json)
        {
            EnsureNotDisposed();
            JQLFacade.Instance.SetString(_handle.Value, 0, placeholder,
                json, JQLFacade.SetStringType.Json);
            return this;
        }

        public JQL SetRegexp(int pos, string regexp)
        {
            EnsureNotDisposed();
            JQLFacade.Instance.SetString(_handle.Value, pos, null,
                regexp, JQLFacade.SetStringType.Regexp);
            return this;
        }

        public JQL SetRegexp(string placeholder, string regexp)
        {
            EnsureNotDisposed();
            JQLFacade.Instance.SetString(_handle.Value, 0, placeholder,
                regexp, JQLFacade.SetStringType.Regexp);
            return this;
        }

        public JQL SetDouble(int pos, double val)
        {
            EnsureNotDisposed();
            JQLFacade.Instance.SetDouble(_handle.Value, pos, null, val);
            return this;
        }

        public JQL SetDouble(string placeholder, double val)
        {
            EnsureNotDisposed();
            JQLFacade.Instance.SetDouble(_handle.Value, 0, placeholder, val);
            return this;
        }

        public JQL SetBoolean(int pos, bool val)
        {
            EnsureNotDisposed();
            JQLFacade.Instance.SetBoolean(_handle.Value, pos, null, val);
            return this;
        }

        public JQL SetBoolean(string placeholder, bool val)
        {
            EnsureNotDisposed();
            JQLFacade.Instance.SetBoolean(_handle.Value, 0, placeholder, val);
            return this;
        }

        public JQL SetNull(int pos)
        {
            EnsureNotDisposed();
            JQLFacade.Instance.SetNull(_handle.Value, pos, null);
            return this;
        }

        public JQL SetNull(string placeholder)
        {
            EnsureNotDisposed();
            JQLFacade.Instance.SetNull(_handle.Value, 0, placeholder);
            return this;
        }

        public void Execute()
        {
            EnsureNotDisposed();
            _explainLog = _explain ? new StringWriter() : null;
            JQLFacade.Instance.Execute(Db.Handle, _handle.Value, _skip, _limit, null, _explainLog);
        }

        public void Execute(JQLCallback cb)
        {
            EnsureNotDisposed();
            _explainLog = _explain ? new StringWriter() : null;
            JQLFacade.Instance.Execute(Db.Handle, _handle.Value, _skip, _limit, cb, _explainLog);
        }

        public KeyValuePair<long, string> First()
        {
            EnsureNotDisposed();
            _explainLog = _explain ? new StringWriter() : null;
            var cb = new JQLCallbackWrapper();
            JQLFacade.Instance.Execute(Db.Handle, _handle.Value, _skip, _limit, cb.Callback, _explainLog);
            return new KeyValuePair<long, string>(cb.Id, cb.Json);
        }

        public string FirstJson() => First().Value;

        public long FirstId() => First().Key;

        public long ExecuteScalarInt64()
        {
            EnsureNotDisposed();
            _explainLog = _explain ? new StringWriter() : null;
            return JQLFacade.Instance.ExecuteScalarInt64(Db.Handle, _handle.Value, _skip, _limit, _explainLog);
        }

        public void Reset()
        {
            EnsureNotDisposed();
            _explainLog = _explain ? new StringWriter() : null;
            JQLFacade.Instance.Reset(_handle.Value);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_handle.IsValueCreated)
                    _handle.Value.Dispose();

                _disposed = true;
            }
        }

        public override string ToString() =>
            new StringBuilder(GetType().Name)
                .Append('[')
                .Append("query=").Append(Query).Append(", ")
                .Append("collection=").Append(Collection)
                .Append(']')
                .ToString();

        public static JQL Create(EJDB2 db, string query, string collection)
        {
            var result = new JQL(db, query, collection);
            result.Initialize();
            return result;
        }

        private void Initialize()
        {
            EJDB2Handle handle = _handle.Value;
            GC.KeepAlive(handle);
        }

        private void EnsureNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        private sealed class JQLCallbackWrapper
        {
            public long Id;
            public string Json;

            public uint Callback(long id, string json)
            {
                Id = id;
                Json = json;
                return 0;
            }
        }
    }
}
