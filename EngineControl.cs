using System;
using System.Runtime.InteropServices;

namespace ustPasser
{
    public class EngineControl
    {
        //https://learn.microsoft.com/ja-jp/dotnet/api/system.diagnostics.process?view=net-8.0
        public static (int, System.Diagnostics.Process) BootEngine()//1:failed,0:scceeded
        {
            //string user = Environment.UserName;
            System.Diagnostics.Process result=null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))//windows
            {
                try
                {
                    result = System.Diagnostics.Process.Start(System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Programs\VOICEVOX\vv-engine\run.exe");
                }
                catch (Exception)
                {
                    result = null;
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))//MacOS
            {
                try
                {
                    result = System.Diagnostics.Process.Start(@"/Applications/VOICEVOX/vv-engine/run.app");
                }
                catch (Exception)
                {
                    try
                    {
                        result = System.Diagnostics.Process.Start(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"/Applications/VOICEVOX/vv-engine/run.app");
                    }
                    catch (Exception)
                    {
                        result = null;
                    }
                }
            }
            else return (1, null);//誰やねんお前
            if (result == null) return (1, null);
            else return (0, result);
        }
        public static int ShutDownEngine(System.Diagnostics.Process process)
        {
            try
            {
                process.Kill();
                Console.WriteLine(process);
            }
            catch (Exception)
            {
                return 1;
            }
            return 0;
        }
        public static int RebootEngine(System.Diagnostics.Process process)
        {
            try
            {
                process.WaitForExit();
                BootEngine();
            }
            catch (Exception)
            {
                return 1;
            }
            return 0;
        }
    }
}
