using System.IO;

namespace TestServer
{
    public static class FileIO
    {
        /// <summary>
        /// Reads the specified file's contents
        /// Returns null on errors
        /// </summary>
        /// <param name="path">Path to a file</param>
        /// <returns>File contents string / null</returns>
        public static string ReadFromFile(string path)
        {
            string toReturn = null;

            try
            {
                using (StreamReader sr = new StreamReader(path, true))
                    toReturn = sr.ReadToEnd();
            }
            catch {  }

            return toReturn;
        }
    }
}
