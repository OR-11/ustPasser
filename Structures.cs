using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ustPasser
{
    class Structures
    {
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

        //Mode1
        //https://w.atwiki.jp/utaou/pages/64.html#id_8fbd1ddf
        public string PBType;
        public int[] Pitches;
        public float PBStart;
    }
}
