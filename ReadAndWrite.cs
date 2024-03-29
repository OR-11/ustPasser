﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ustPasser
{
    public class ReadAndWrite
    {
        public static int WriteFile(string filePass, byte[] content)
        {
            try
            {
                //File.WriteAllText(filePass, content);
                using (var stream = File.Open(filePass, FileMode.Create, FileAccess.Write))
                {
                    using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                    {
                        writer.Write(content);
                    }
                }
            }
            catch (Exception)
            {
                return 1;
            }
            return 0;
        }
        public static string ReadUstFile(string filePass)
        {
            try
            {
                EncodingProvider provider = System.Text.CodePagesEncodingProvider.Instance;
                return File.ReadAllText(filePass, provider.GetEncoding("shift_jis"));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
