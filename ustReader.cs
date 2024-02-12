using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ustPasser
{
    public class ustConverter
    {
        public static List<Dictionary<string, dynamic>> Structurize(string data)//[#SETTING]→[#0000][#0001]...
        {
            List<Dictionary<string, dynamic>> DictedUst = new List<Dictionary<string, dynamic>>();
            string[] Lines = data.Split('\n');
            for (int i = 0; i < Lines.Length; i++)
            {
                string line = Lines[i];
                if (line.Length > 1 && line[0] == '[' && line[^2] == ']')
                {
                    if (line[1..^2] == "#TRACKEND") break;//読み取りたいところが終了
                    DictedUst.Add(new Dictionary<string, dynamic>());
                }
                else if (line.Length > 1)
                {
                    //Console.WriteLine(line.Split('=')[0] + "," + line.Split('=')[1][0..^1]);
                    DictedUst[^1].Add(line.Split('=')[0], line.Split('=')[1].Length > 1 ? line.Split('=')[1][0..^1] : null);
                }
            }
            return DictedUst;
            //https://w.atwiki.jp/utaou/pages/64.html#id_e7beb30c
            /*[#SETTING]
             * string(float) UstVersion
             * float Tempo
             * int Tracks
             * string ProjectName
             * string VoiceDir
             * string OutFile
             * string CacheDir
             * string Tool1
             * string Tool2
             * 
             * [#XXXX]
             * int Length
             * string Lyric
             * int NoteNum
             * float Intensity //音の強さ
             * float Modulation
             * float Moduration
             */
        }

        public static Note[] ConvertUstToNotes(List<Dictionary<string, dynamic>> UstData)
        {
            List<Note> notes = new List<Note>();
            Console.WriteLine(UstData[^1].Keys.Count);
            for (int i = 1; i < UstData.Count; i++)
            {
                Console.WriteLine(UstData[i].Keys.ToArray()[2] +","+UstData[i][UstData[i].Keys.ToArray()[2]]);
                notes.Add(new Note(UstData[i]["Lyric"].ToString() == "R" ? null : int.Parse(UstData[i]["NoteNum"]), float.Parse(UstData[i]["Length"]) / 480f, UstData[i]["Lyric"].ToString() == "R" ? "" : (string)UstData[i]["Lyric"]));
            }
            return notes.ToArray();
        }
    }
}
