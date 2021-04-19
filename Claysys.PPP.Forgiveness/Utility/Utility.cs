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
            if (IsEventLogged)
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
                using (StreamWriter streamWriter = new StreamWriter(string.Concat(logFilePath, "\\", "Log-" + DateTime.Now.Date.Day + "." + DateTime.Now.Date.Month + "." + DateTime.Now.Date.Year + ".txt"), true))
                {
                    streamWriter.WriteLine(string.Concat(new string[] { DateTime.Now.ToString(), ":", DateTime.Now.Millisecond.ToString(), " -@: ", action }));
                    streamWriter.Close();
                }
            }
        }
    }
}
