using System;
using System.Runtime.InteropServices;
using System.Net.Http;

namespace ustPasser
{
    public class VoiceVoxEngineControl
    {
        public static int ServerPort()
        {
            return 50021;//とりあえず。仕組みができたらつなげる
        }
        public static string EngineManifest()
        {
            return new HttpClient().GetStringAsync(new Uri(@"http://127.0.0.1:" + ServerPort().ToString() + @"/engine_manifest")).Result;
        }
        public static string GetEngineVersion()
        {
            return new HttpClient().GetStringAsync(new Uri(@"http://127.0.0.1:" + ServerPort().ToString() + @"/version")).Result;
        }
        public static string GetCoreVersion()
        {
            return new HttpClient().GetStringAsync(new Uri(@"http://127.0.0.1:" + ServerPort().ToString() + @"/core_versions")).Result;
        }
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
                process.Kill();//VOICEVOXも多分この方法でやってる
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
