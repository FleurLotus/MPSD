namespace MagicPictureSetDownloader.Db
{
    using System;
    using System.Data;
    
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

                if (!_bacthMode)
                    _cnx.Close();
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
                    CheckDisposed();
                    _cnx.ConnectionString = value;
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
                CheckDisposed();
                _cnx.Open();
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
                    return;


                if (!_bacthMode)
                    _cnx.Dispose();

                _disposed = true;
            }

            private void CheckDisposed()
            {
                if (_disposed)
                    throw new ObjectDisposedException(typeof(ConnectionWrapper).Name);
            }

        }

    }
}
