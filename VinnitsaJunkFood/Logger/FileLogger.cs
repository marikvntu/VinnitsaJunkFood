using System;
using System.Diagnostics;

namespace VinnitsaJunkFood.Logger
{
    public class FileLogger{        
        private static object _lockObject = new object();        

        public FileLogger(){            
            TextWriterTraceListener logListener = new TextWriterTraceListener();
            Trace.Listeners.Add(logListener);                 
        }        

        /// <summary>
        /// Pushes message to the log file
        /// </summary>
        /// <param name="message"></param>
        public void WriteLog(string message) {
            lock (_lockObject){
                string timeStamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff");
                Trace.WriteLine(timeStamp + " - "  + message);                                                
            }
        }       
    }
}