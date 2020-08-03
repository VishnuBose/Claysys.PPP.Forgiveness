using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Claysys.PPP.Forgiveness.Utility
{
    class Utility
    {
        public static bool IsEventLogged = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableEventLogging"]);
        public static string DocuSignAccountID = string.Empty;
        public static void LogAction(string action)
        {
            string logFilePath = ConfigurationManager.AppSettings["LogFileLocation"];
            if (!Directory.Exists(logFilePath))
            {
                Directory.CreateDirectory(logFilePath);
                DirectoryInfo directoryInfo = new DirectoryInfo(logFilePath);
                DirectorySecurity accessControl = directoryInfo.GetAccessControl();
                accessControl.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                directoryInfo.SetAccessControl(accessControl);
            }
            using (StreamWriter streamWriter = new StreamWriter(string.Concat(logFilePath, "\\", "Log.txt"), true))
            {
                streamWriter.WriteLine(string.Concat(new string[] { DateTime.Now.ToString(), ":", DateTime.Now.Millisecond.ToString(), " -@: ", action }));
                streamWriter.Close();
            }
        }
    }
}
