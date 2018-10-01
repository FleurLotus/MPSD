namespace MagicPictureSetDownloader.Db
{
    using System;
    using System.Data;

    using Common.Database;

    internal partial class DatabaseConnection
    {
        private sealed class ConnectionWrapper : IDbConnection
        {
            private bool _disposed;
            private readonly IDbConnection _cnx;
            private readonly bool _bacthMode;

            public ConnectionWrapper(IDbConnection cnx, bool bacthMode)
            {
                _cnx = cnx;
                _bacthMode = bacthMode;
            }

            public IDbTransaction BeginTransaction(IsolationLevel il)
            {
                CheckDisposed();
                return _cnx.BeginTransaction(il);
            }

            public IDbTransaction BeginTransaction()
            {
                CheckDisposed();
                return _cnx.BeginTransaction();
            }

            public void ChangeDatabase(string databaseName)
            {
                CheckDisposed();
                _cnx.ChangeDatabase(databaseName);
            }

            public void Close()
            {
                CheckDisposed();

                CloseInner();
            }

            public string ConnectionString
            {
                get
                {
                    CheckDisposed();
                    return _cnx.ConnectionString;
                }
                set
                {
                    throw new ApplicationDbException("Change of ConnectionString is not allowed");
                }
            }

            public int ConnectionTimeout
            {
                get
                {
                    CheckDisposed();
                    return _cnx.ConnectionTimeout;
                }
            }

            public IDbCommand CreateCommand()
            {
                CheckDisposed();
                return _cnx.CreateCommand();
            }

            public string Database
            {
                get
                {
                    CheckDisposed();
                    return _cnx.Database;
                }
            }

            public void Open()
            {
                throw new ApplicationDbException("Open is not allowed");
            }

            public ConnectionState State
            {
                get
                {
                    CheckDisposed();
                    return _cnx.State;
                }
            }

            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                CloseInner();

                _disposed = true;
            }

            private void CheckDisposed()
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(typeof(ConnectionWrapper).Name);
                }
            }

            private void CloseInner()
            {
                if (!_bacthMode)
                {
                    _cnx.Close();
                }
            }
        }
    }
}
