using System;
using System.Diagnostics;
using System.Text;

namespace VinnitsaJunkFood.Logger
{
    public class FileLogger{        
        private static object lockObject = new object();        

        public FileLogger(){            
            TextWriterTraceListener logListener = new TextWriterTraceListener();
            Trace.Listeners.Add(logListener);                 
        }        

        /// <summary>
        /// Pushes message to the log file
        /// </summary>
        /// <param name="message"></param>
        public void WriteLog(string message) {
            lock (lockObject){
                string timeStamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff");
                Trace.WriteLine(timeStamp + " - "  + message);                                                
            }
        }       
    }
}