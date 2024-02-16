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
             * 
             * (Mode1がある場合:)
             * int/string PBType //通常は5。OldDataの場合はwarningを出す
             * int,int... PitchBend
             * float PBStart
             * 
             * (Mode2がある場合:)
             * (int,float=0) PBS
             * float[] PBW
             * float[] PBY
             * float[7]+任意(未使用) VBR
             */
        }

        public static Note[] ConvertUstToNotes(List<Dictionary<string, dynamic>> UstData)
        {
            List<Note> notes = new List<Note>();
            for (int i = 1; i < UstData.Count; i++)
            {
                notes.Add(new Note(UstData[i]["Lyric"].ToString() == "R" ? null : int.Parse(UstData[i]["NoteNum"]), float.Parse(UstData[i]["Length"]) / 480f, UstData[i]["Lyric"].ToString() == "R" ? "" : (string)UstData[i]["Lyric"]));
                
                if (UstData[i].ContainsKey("PBType"))//Mode1
                {
                    notes[i - 1].Mode1.PBType = UstData[i]["PBType"].ToString();
                    List<int> temp = new List<int>();
                    foreach(string j in UstData[i]["Piches"].ToString().Split(','))
                    {
                        temp.Add(j == "" ? 0 : int.Parse(j));
                    }
                    notes[i - 1].Mode1.Pitches = temp.ToArray();
                    notes[i - 1].Mode1.PBType = UstData[i]["PBType"].ToString();
                }

                //Mode2
                if (UstData[i].ContainsKey("PBS"))
                {
                    notes[i - 1].Mode2.PBS = ((int PosMs, float PitchShift))(UstData[i]["PBS"].ToString().Contains(';') ? 
                        (Int32.Parse(UstData[i]["PBS"].ToString().Split(';')[0]), float.Parse(UstData[i]["PBS"].ToString().Split(';')[1])) : 
                        (Int32.Parse(UstData[i]["PBS"].ToString()), 0f));
                    {
                        List<float> temp = new List<float>();
                        foreach (string j in UstData[i]["PBW"].ToString().Split(','))
                        {
                            temp.Add(float.Parse(j));
                        }
                        notes[i - 1].Mode2.PBW = temp.ToArray();
                    }
                    if (UstData[i].ContainsKey("PBY"))
                    {
                        List<float> temp = new List<float>();
                        foreach (string j in UstData[i]["PBY"].ToString().Split(','))
                        {
                            temp.Add(j == "" ? 0 : float.Parse(j));
                        }
                        notes[i - 1].Mode2.PBY = temp.ToArray();
                    }
                    if (UstData[i].ContainsKey("PBM"))
                    {
                        List<char?> temp = new List<char?>();
                        foreach (string j in UstData[i]["PBM"].ToString().Split(','))
                        {
                            temp.Add(j == "" ? null : char.Parse(j));
                        }
                        notes[i - 1].Mode2.PBM = temp.ToArray();
                    }
                    if (UstData[i].ContainsKey("VBR"))
                    {
                        List<float> temp = new List<float>();
                        foreach(float j in UstData[i]["VBR"].ToString().Split(',')[0..^2])
                        {
                            temp.Add(j);
                        }
                        notes[i - 1].Mode2.VBR = temp.ToArray();
                    }
                }
            }
            return notes.ToArray();
        }
    }
}
