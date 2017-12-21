using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace garden_solver_generator
{
    static class FileCombiner
    {
        //put together the separately created map databases
        public static void CombineFiles()
        {
            BinaryReader bReader;
            BinaryWriter bWriter = new BinaryWriter(File.OpenWrite("40kmixed.dat"));
            for (int i = 1; i <= 8; i++)
            {
                bReader = new BinaryReader(File.OpenRead("1250easy" + i + ".dat"));
                var mapByteArray = bReader.ReadBytes(67500);
                bWriter.Write(mapByteArray);
                bReader.Close();
            }
            for (int i = 1; i <= 8; i++)
            {
                bReader = new BinaryReader(File.OpenRead("1250hard" + i + ".dat"));
                var mapByteArray = bReader.ReadBytes(67500);
                bWriter.Write(mapByteArray);
                bReader.Close();
            }
            for (int i = 1; i <= 8; i++)
            {
                bReader = new BinaryReader(File.OpenRead("1250easyquint" + i + ".dat"));
                var mapByteArray = bReader.ReadBytes(67500);
                bWriter.Write(mapByteArray);
                bReader.Close();
            }
            for (int i = 1; i <= 8; i++)
            {
                bReader = new BinaryReader(File.OpenRead("1250hardquint" + i + ".dat"));
                var mapByteArray = bReader.ReadBytes(67500);
                bWriter.Write(mapByteArray);
                bReader.Close();
            }
            bWriter.Close();
        }
    }
}
