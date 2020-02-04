using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ejdb2.Native;

namespace Ejdb2
{
    /// <summary>
    /// EJDB2 Query specification.
    /// </summary>
    /// <remarks>
    /// <para>Query can be reused multiple times with various placeholder parameters.See
    /// JQL specification: https://github.com/Softmotions/ejdb/blob/master/README.md#jql</para>
    /// <para>Memory resources used by JQL instance must be released explicitly by
    /// <see cref="Ejdb2.JQL.Dispose"/>.</para>
    /// <para>Note: If user did not close instance explicitly it will be
    /// freed anyway once jql object will be garbage collected.</para>
    /// <para>Typical usage:</para>
    /// <code>
    /// using (JQL q = db.CreateQuery("/[foo=:val]", "mycoll").SetString("val", "bar")) {
    ///     q.Execute((docId, doc) => {
    ///         Console.WriteLine("Found {0} {1}", docId, doc);
    ///         return 1;
    ///     });
    /// }
    /// </code>
    /// </remarks>
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

        /// <summary>
        /// Owner database instance
        /// </summary>
        public EJDB2 Db { get; }

        /// <summary>
        /// Query specification used to construct this query object
        /// </summary>
        public string Query { get; }

        /// <summary>
        /// Collection name used for this query
        /// </summary>
        public string Collection
        {
            get => _collection;
            set => _collection = value;
        }

        /// <summary>
        /// Turn on collecting of query execution log. See also <seealso cref="GetExplainLog"/>
        /// </summary>
        /// <returns>This object</returns>
        public JQL WithExplain()
        {
            _explain = true;
            return this;
        }

        /// <summary>
        /// Turn off collecting of query execution log . See also <seealso cref="GetExplainLog"/>
        /// </summary>
        /// <returns>This object</returns>
        public JQL WithNoExplain()
        {
            _explain = false;
            return this;
        }

        public string GetExplainLog() => _explainLog?.ToString();

        /// <summary>
        /// Number of records to skip. This parameter takes precedence over <c>skip</c> encoded in query spec.
        /// </summary>
        /// <param name="skip">Number of records to skip</param>
        /// <returns>This object</returns>
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

        /// <summary>
        /// Maximum number of records to retrive. This parameter takes precedence
        /// over <c>limit</c> encoded in query spec.
        /// </summary>
        /// <param name="limit">Maximum number of records to retrive</param>
        /// <returns>This object</returns>
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

        /// <summary>
        /// Set positional string parameter starting for <c>0</c> index.
        /// </summary>
        /// <remarks>
        /// <para>Example:</para>
        /// <para><c>
        /// db.CreateQuery("/[foo=:?]", "mycoll").SetString(0, "zaz")
        /// </c></para>
        /// </remarks>
        /// <param name="pos">Zero based positional index</param>
        /// <param name="val">Value to set</param>
        /// <returns>This object</returns>
        /// <exception cref="EJDB2Exception"></exception>
        public JQL SetString(int pos, string val)
        {
            EnsureNotDisposed();
            JQLFacade.Instance.SetString(_handle.Value, pos, null,
                val, JQLFacade.SetStringType.Other);
            return this;
        }

        /// <summary>
        /// Set string parameter placeholder in query spec.
        /// </summary>
        /// <remarks>
        /// <para>Example:</para>
        /// <para><c>
        /// db.CreateQuery("/[foo=:val]", "mycoll").SetString("val", "zaz");
        /// </c></para>
        /// </remarks>
        /// <param name="placeholder">Placeholder name</param>
        /// <param name="val">Value to set</param>
        /// <returns>This object</returns>
        /// <exception cref="EJDB2Exception"></exception>
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

        /// <summary>
        /// Execute query without result set callback
        /// </summary>
        /// <exception cref="EJDB2Exception"></exception>
        public void Execute()
        {
            EnsureNotDisposed();
            _explainLog = _explain ? new StringWriter() : null;
            JQLFacade.Instance.Execute(Db.Handle, _handle.Value, _skip, _limit, null, _explainLog);
        }

        /// <summary>
        /// Execute query and handle records by provided <c>cb</c>
        /// </summary>
        /// <param name="cb">Optional callback SAM</param>
        /// <exception cref="EJDB2Exception"></exception>
        public void Execute(JQLCallback cb)
        {
            EnsureNotDisposed();
            _explainLog = _explain ? new StringWriter() : null;
            JQLFacade.Instance.Execute(Db.Handle, _handle.Value, _skip, _limit, cb, _explainLog);
        }

        /// <summary>
        /// Get first record entry <c>{documentId, json}</c> in results set. Entry will
        /// contain nulls if no records found.
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<long, string> First()
        {
            EnsureNotDisposed();
            _explainLog = _explain ? new StringWriter() : null;
            var cb = new JQLCallbackWrapper();
            JQLFacade.Instance.Execute(Db.Handle, _handle.Value, _skip, _limit, cb.Callback, _explainLog);
            return new KeyValuePair<long, string>(cb.Id, cb.Json);
        }

        /// <summary>
        /// Get first document body as JSON string or null
        /// </summary>
        /// <returns>Document body</returns>
        public string FirstJson() => First().Value;

        /// <summary>
        /// Get first document id ot null
        /// </summary>
        /// <returns>Document id</returns>
        public long FirstId() => First().Key;

        /// <summary>
        /// Execute scalar query
        /// </summary>
        /// <remarks>
        /// <para>Example:</para>
        /// <para><c>long count = db.CreateQuery("@mycoll/* | count").ExecuteScalarInt64(); </c></para>
        /// </remarks>
        /// <returns>Scalar result</returns>
        public long ExecuteScalarInt64()
        {
            EnsureNotDisposed();
            _explainLog = _explain ? new StringWriter() : null;
            return JQLFacade.Instance.ExecuteScalarInt64(Db.Handle, _handle.Value, _skip, _limit, _explainLog);
        }

        /// <summary>
        /// Reset data stored in positional placeholders
        /// </summary>
        public void Reset()
        {
            EnsureNotDisposed();
            _explainLog = _explain ? new StringWriter() : null;
            JQLFacade.Instance.Reset(_handle.Value);
        }

        /// <summary>
        /// Close query instance releasing memory resources
        /// </summary>
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
