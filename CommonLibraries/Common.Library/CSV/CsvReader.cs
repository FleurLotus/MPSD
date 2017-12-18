namespace Common.Library.CSV
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Common.Library.Exception;

    public class CsvReader : ICsvReader
    {
        public const char EscapingChar = '"';

        private const char DefaultSeparator = ',';
        private const int UninitializedColumnsCount = -1;

        private string[] _currentData;
        private string[] _headers;
        private bool _withHeader;
        private int _columnsCount;
        private char _separator;
        private StreamReader _streamReader;
        private bool _disposed;
        private bool _errorState;
        private int _currentLine;

        public CsvReader(Stream stream, bool withHeader)
            : this(stream, withHeader, DefaultSeparator)
        {
        }
        public CsvReader(Stream stream, bool withHeader, char separator)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            Init(stream, withHeader, separator);
        }

        public CsvReader(string path, bool withHeader)
            : this(path, withHeader, DefaultSeparator)
        {
        }
        public CsvReader(string path, bool withHeader, char separator)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            Init(new FileStream(path, FileMode.Open, FileAccess.Read), withHeader, separator);
        }

        public bool WithHeader
        {
            get
            {
                CheckDisposed();
                return _withHeader;
            }
        }
        public char Separator
        {
            get
            {
                CheckDisposed();
                return _separator;
            }
        }
        public bool EndOfStream
        {
            get
            {
                CheckDisposed();
                return _streamReader.EndOfStream;
            }
        }

        private void Init(Stream stream, bool withHeader, char separator)
        {
            if (separator == EscapingChar)
            {
                throw new ArgumentException("Could not be " + EscapingChar, nameof(separator));
            }

            _withHeader = withHeader;
            _separator = separator;

            _streamReader = new StreamReader(stream);

            if (_withHeader)
            {
                _currentLine = -2;
                _headers = GetLine();
                _columnsCount = _headers.Length;
            }
            else
            {
                _currentLine = -1;
                _columnsCount = UninitializedColumnsCount;
                _headers = null;
            }
        }
        private void CheckErrorState()
        {
            if (_errorState)
            {
                throw new CsvReaderErrorStateException();
            }
        }
        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("CsvReader");
            }
        }
        private string[] GetLine()
        {
            _currentLine++;
            
            List<string> ret = new List<string>();
            bool inEscape = false;

            StringBuilder sb = new StringBuilder();
            while (true)
            {
                //Read next char
                int readChar = _streamReader.Read();
                if (readChar < 0)
                {
                    //End of stream
                    if (inEscape)
                    {
                        _errorState = true;
                        throw new CsvReaderUnclosedEscapeException();
                    }
                    ret.Add(sb.ToString());
                    break;
                }

                char c = (char)readChar;

                if (inEscape)
                {
                    if (c == EscapingChar)
                    {
                        int next = _streamReader.Peek();
                        if (next < 0)
                        {
                            //End of stream after end of escape so ok
                            inEscape = false;
                        }
                        else
                        {
                            char cNext = (char)next;
                            if (cNext == EscapingChar)
                            {
                                //Double Escape char so add one to field
                                _streamReader.Read();
                                sb.Append(EscapingChar);
                            }
                            else if (cNext == _separator || cNext == '\r' || cNext == '\n')
                            {
                                //End of escape ok
                                inEscape = false;
                            }
                            else
                            {
                                //After end
                                _errorState = true;
                                throw new CsvReaderBadClosingEscapeException();
                            }
                        }
                    }
                    else
                    {
                        //Default
                        sb.Append(c);
                    }
                }
                else
                {
                    if (c == _separator)
                    {
                        //Next field
                        ret.Add(sb.ToString());
                        sb = new StringBuilder();
                    }
                    else if (c == '\n')
                    {
                        ret.Add(sb.ToString());
                        break;
                    }
                    else if (c == '\r')
                    {
                        //Dixit StreamReader.ReadLine()
                        // \n - UNIX   \r\n - DOS   \r - Mac
                        int next = _streamReader.Peek();
                        if (next >= 0 && ((char)next) == '\n')
                        {
                            _streamReader.Read();
                        }

                        ret.Add(sb.ToString());
                        break;
                    }
                    else if (sb.Length == 0 && c == EscapingChar)
                    {
                        //Starting of escape
                        inEscape = true;
                    }
                    else
                    {
                        //Default
                        sb.Append(c);
                    }
                }
            }
            
            return ret.ToArray();
        }

        public int GetCurrentLine()
        {
            CheckDisposed();
            return _currentLine;
        }
        public int GetColumnsCount()
        {
            CheckDisposed();
            if (!_withHeader && _currentData == null)
            {
                throw new CsvReaderNoReadCallException();
            }

            return _columnsCount;
        }
        public string[] GetHeaders()
        {
            CheckDisposed();

            if (WithHeader)
                return (string[])_headers.Clone();
            return new string[0];
        }
        public string GetValue(int index)
        {
            CheckDisposed();
            CheckErrorState();

            if (_currentData == null)
            {
                throw new CsvReaderNoReadCallException();
            }

            if (index < 0 || index >= _currentData.Length)
            {
                throw new IndexOutOfRangeException();
            }

            return _currentData[index];
        }
        public bool Read()
        {
            CheckDisposed();
            CheckErrorState();

            if (EndOfStream)
            {
                _currentData = new string[0];
                return false;
            }

            _currentData = GetLine();
            if (!_withHeader && _columnsCount == UninitializedColumnsCount)
            {
                _columnsCount = _currentData.Length;
            }
            else if (_columnsCount != _currentData.Length)
            {
                _errorState = true;
                throw new CsvReaderWrongNumberOfColumnsException(_currentLine, _currentData.Length, _columnsCount);
            }

            return true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _streamReader.Dispose();
            }
            _disposed = true;
        }
    }
}
