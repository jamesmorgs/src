using System;
using System.IO;

namespace AddressProcessing.CSV
{
    /*
        2) Refactor this class into clean, elegant, rock-solid & well performing code, without over-engineering.
           Assume this code is in production and backwards compatibility must be maintained.
    */

    public class CSVReaderWriter : IDisposable
    {
        private StreamReader _readerStream = null;
        private StreamWriter _writerStream = null;

        [Flags]
        public enum Mode { Read = 1, Write = 2 };

        public void Open(string fileName, Mode mode)
        {
            try
            {
                switch (mode)
                {
                    case Mode.Read:
                        _readerStream = File.OpenText(fileName);
                        break;
                    case Mode.Write:
                        FileInfo fileInfo = new FileInfo(fileName);
                        _writerStream = fileInfo.CreateText();
                        break;
                    default:
                        throw new Exception("Unknown file mode for " + fileName);                        
                }
            }
            catch (FileNotFoundException fnf)
            {
                // Log Error and throw if necessary
                //throw;
            }
            catch (IOException io)
            {
                // Log Error and throw if necessary
                //throw;
            }
            catch (Exception ex)
            {
                // Log Error and throw if necessary
                //throw;
            }
        }       

        // Write columns straight to writerstream. No need to append to a string object and then write.
        public void Write(params string[] columns)
        {
            if (_writerStream != null)
            {
                for (int i = 0; i < columns.Length; i++)
                {
                    _writerStream.Write(columns[i] + (((columns.Length - 1) != i) ? "\t" : null));
                }

                _writerStream.WriteLine();
            }
        }

        // Left this method in for backwards compatibility. It just acts as a wrapper for the Read method below. Although this method is probabley never called.
        public bool Read(string column1, string column2)
        {            
            return Read(out column1, out column2);
        }

        // Refactored this method to produce cleaner and reduced lines of code 
        public bool Read(out string column1, out string column2)
        {
            if (_readerStream != null)
            {
                string line = _readerStream.ReadLine();

                if (line != null)
                {
                    string[] columns = line.Split('\t');

                    if (columns.Length != 0)
                    {
                        const int FIRST_COLUMN = 0;
                        const int SECOND_COLUMN = 1;

                        return SetColumns(out column1, out column2, columns[FIRST_COLUMN], columns[SECOND_COLUMN], true);
                    }
                }
            }            

            return SetColumns(out column1, out column2);
        }

        // Extracted this method for code re-use
        private static bool SetColumns(out string column1, out string column2, string colvalue1 = null, string colValue2 = null, bool returnValue = false)
        {
            column1 = colvalue1;
            column2 = colValue2;

            return returnValue;
        }

        public void Close()
        {
            Dispose();
        }

        #region IDisposable Support

        /*
            Adding this Dispose pattern allows the AddressFileProcessor class to instantiate from CSVReaderWriter with the using(var reader = new CSVReaderWriter())
            construct. So that the Dispose method is automatically called.     
        */

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // Free unmanaged resources (unmanaged objects) and override a finalizer below.                
                if (_writerStream != null)
                {
                    _writerStream.Close();
                }

                if (_readerStream != null)
                {
                    _readerStream.Close();
                }

                disposedValue = true;
            }
        }
       
        ~CSVReaderWriter()
        {            
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {            
            Dispose(true);           
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
