#define ENABLE_LOGS
using System.Diagnostics;

namespace ShmupBoss
{
    /// <summary>
    /// This can be used if you want to make logs for debugging or for whatever reason and would 
    /// like to be able to disable them easily instead of going to them one by one.<br></br>
    /// This is the equivalent of Unity's Debug.Log, only with an option to be disabled easily.<br></br>
    /// The reason we have used Unity's standard Debug.Log instead of this one, is that we wanted 
    /// users to be able to quickly know where a problem is. In addition if you have no errors no 
    /// debug logs should ever appear.<br></br>
    /// Debug.Log can decrease the performance if they exist in the build. If for whatever reason 
    /// you would like to see logs while still in development but still have the ability to quickly 
    /// disable them you should consider using this logger instead of Unity's.<br></br> <br></br>
    /// By commenting out the first line of #define ENABLE_LOGS all logs using this logger will be disabled.
    /// </summary>
    public static class Logger
    {
        [Conditional("ENABLE_LOGS")]
        public static void Print(string logMessage)
        {
            UnityEngine.Debug.Log(logMessage);
        }
    }
}