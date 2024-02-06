using System;
using System.Runtime.InteropServices;

namespace ustPasser
{
    public class EngineControl
    {
        public static int BootEngine()//1:failed,0:scceeded
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
            else return 1;//誰やねんお前
            if (result == null) return 1;
            else return 0;
        }
        
    }
}
