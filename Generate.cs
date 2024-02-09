using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace ustPasser
{
    public class Generate
    {
        public static string GetSingers(string core_version)
        {
            using var Http = new HttpClient();
            return Http.GetStringAsync(@"http://127.0.0.1:50021/singers?core_version=" + core_version).Result;
        }

        //frame_length仕様
        //https://github.com/VOICEVOX/voicevox/blob/main/src/sing/domain.ts#L105
        public static string SingFrameAudioQuery(int speaker, string core_version, Note[] notes)
        {//https://qiita.com/rawr/items/f78a3830d894042f891b
            using var Http = new HttpClient();
            string jsonContent =@"{""notes"":[";

            int restDuration = 1;//とりあえず1らしい
            //最初に休符を追加

            for (int i = 0; i < notes.Length; i++)
            {
                var j = notes[i];
                if (i != 0) jsonContent += ",";
                jsonContent += @"{""key"":" + j.key + @",""frame_length"":" + j.frame_length + @",""lyric"":""" + j.lyric + @"""}";
            }
            jsonContent += @"]}";
            Console.WriteLine(jsonContent);
            var content = new StringContent(jsonContent, Encoding.UTF8, @"application/json");
            return Http.PostAsync(@"http://127.0.0.1:50021/sing_frame_audio_query?speaker=" + speaker + @"&core_version=" + core_version, content).Result.Content.ReadAsStringAsync().Result;
        }

        public static byte[] FrameSynthesis(int speaker, string core_version, string bodyData)
        {//https://qiita.com/c-yan/items/6e506399675e3cc56732
            using var Http = new HttpClient();
            //var content = new StringContent(bodyData, Encoding.UTF8, @"application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, @"http://127.0.0.1:50021/frame_synthesis?speaker=" + speaker + @"&core_version=" + core_version);
            request.Headers.Add("accept", "audio/wav");
            //request.Headers.Add("Content-Type", "application/json");
            request.Content = new StringContent(bodyData,Encoding.UTF8, @"application/json");
            return Http.SendAsync(request).Result.Content.ReadAsByteArrayAsync().Result;
        }
    }

    public class Note
    {
        public Note(int key, int frame_length, string lyric)
        {
            this.key = key;
            this.frame_length = frame_length;
            this.lyric = lyric;
        }
        public int key;
        public int frame_length;
        public string lyric;
    }
}
