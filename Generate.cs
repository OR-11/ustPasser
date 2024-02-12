using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;

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
        public static string SingFrameAudioQuery(int speaker, string core_version, Note[] notes, float tempo, float StartRestLengthSec = 1, float EndRestLengthSec = 1)
        {//https://qiita.com/rawr/items/f78a3830d894042f891b
            using var Http = new HttpClient();
            string jsonContent =@"{""notes"":[";

            //https://github.com/VOICEVOX/voicevox/blob/55d3df8712b54360d2428a0954fcd9d33f67dd16/src/store/singing.ts#L877
            (float start, float end) restDurationSeconds = (StartRestLengthSec, EndRestLengthSec);//とりあえず1らしい
            decimal frameRate = JsonToLibary.ParseJson(VoiceVoxEngineControl.EngineManifest())["frame_rate"];
            (int start, int end) restFrameLength = (Convert.ToInt32(Math.Round((decimal)restDurationSeconds.start * frameRate,MidpointRounding.AwayFromZero)), Convert.ToInt32(Math.Round((decimal)restDurationSeconds.end * frameRate, MidpointRounding.AwayFromZero)));
            List<Note> notesForRequestToEngine = new List<Note>();
            //最初に休符を追加
            notesForRequestToEngine.Add(new Note(null, restFrameLength.start, ""));

            foreach (Note note in notes)
            {
                //Console.WriteLine(Convert.ToInt32(Math.Round(60 / (decimal)tempo * (decimal)note.length * frameRate, MidpointRounding.AwayFromZero)));
                notesForRequestToEngine.Add(new Note(note.key,Convert.ToInt32(Math.Round(60/(decimal)tempo*(decimal)note.length*frameRate,MidpointRounding.AwayFromZero)),note.lyric));
            }
            //最後にも休符を追加
            notesForRequestToEngine.Add(new Note(null, restFrameLength.end, ""));

            for (int i = 0; i < notesForRequestToEngine.Count; i++)
            {
                var j = notesForRequestToEngine[i];
                if (i != 0) jsonContent += ",";
                jsonContent += @"{""key"":" + (j.key==null?"null":j.key.Value) + @",""frame_length"":" + j.length + @",""lyric"":""" + j.lyric + @"""}";
            }

            jsonContent += @"]}";
            Console.WriteLine(jsonContent);
            var content = new StringContent(jsonContent, Encoding.UTF8, @"application/json");
            var result = Http.PostAsync(@"http://127.0.0.1:50021/sing_frame_audio_query?speaker=" + speaker + @"&core_version=" + core_version, content).Result;
            if (((int)result.StatusCode)==200) return result.Content.ReadAsStringAsync().Result;
            else
            {
                Console.WriteLine(result.Content.ReadAsStringAsync().Result);
                return "";
            }
        }
        
        public static byte[] FrameSynthesis(int speaker, string core_version, string bodyData, int key = 0)
        {
            if (key != 0)
            {
                var temp = JsonToLibary.ParseJson(bodyData);
                Console.WriteLine(temp.GetType());
                double[] fs = new double[temp["f0"].Count];
                for (int i = 0;i< temp["f0"].Count; i++)
                {
                    fs[i] = Decimal.ToDouble(temp["f0"].ToArray()[i]);
                }
                double[] changedFs = new double[fs.Length];
                for (int i = 0; i < fs.Length; i++)
                {
                    changedFs[i] = fs[i] * Math.Pow(2, key / 12);
                }
                temp["f0"] = changedFs;
                bodyData = JsonSerializer.Serialize(temp);
            }

            //https://qiita.com/c-yan/items/6e506399675e3cc56732
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
    {//keyはmidi準拠、lengthは、sec*frame rate(95くらいのやつ)=length
        public Note(int? key, float length, string lyric)
        {
            this.key = key;
            this.length = length;
            this.lyric = lyric;
        }
        public int? key;
        public float length;
        public string lyric;
    }
}
