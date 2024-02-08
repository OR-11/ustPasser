using System;
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
                using (var stream = File.Open(filePass, FileMode.Create, FileAccess.ReadWrite))
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
    }
}
