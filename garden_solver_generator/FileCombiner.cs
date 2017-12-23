using System.IO;

namespace garden_solver_generator
{
    static class FileCombiner
    {
        //put together the separately created map databases
        public static void CombineFiles(int mapCount)
        {
            BinaryReader bReader;
            BinaryWriter bWriter = new BinaryWriter(File.OpenWrite(mapCount * 4 + "mixed.dat"));

            bReader = new BinaryReader(File.OpenRead(mapCount + "easy.dat"));
            var mapByteArray = bReader.ReadBytes(mapCount * 54);
            bWriter.Write(mapByteArray);
            bReader.Close();

            bReader = new BinaryReader(File.OpenRead(mapCount + "hard.dat"));
            mapByteArray = bReader.ReadBytes(mapCount * 54);
            bWriter.Write(mapByteArray);
            bReader.Close();

            bReader = new BinaryReader(File.OpenRead(mapCount + "easyquint.dat"));
            mapByteArray = bReader.ReadBytes(mapCount * 54);
            bWriter.Write(mapByteArray);
            bReader.Close();

            bReader = new BinaryReader(File.OpenRead(mapCount + "hardquint.dat"));
            mapByteArray = bReader.ReadBytes(mapCount * 54);
            bWriter.Write(mapByteArray);
            bReader.Close();
            
            bWriter.Close();
        }
    }
}
