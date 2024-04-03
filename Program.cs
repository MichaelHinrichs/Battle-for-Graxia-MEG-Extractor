//Written for Battle for Graxia. https://steamcommunity.com/app/90530/
using System.IO;

namespace Battle_for_Graxia_MEG_Extractor
{
    class Program
    {
        static BinaryReader br;
        static void Main(string[] args)
        {
            br = new BinaryReader(File.OpenRead(args[0]));
            br.BaseStream.Position = 8;
            int fileDataTableStart = br.ReadInt32();
            int fileCount = br.ReadInt32();
            int namesCount = br.ReadInt32();
            int fileTableStart = br.ReadInt32();

            System.Collections.Generic.List<string> names = new();
            for (int i = 0; i < namesCount; i++)
                names.Add(new(br.ReadChars(br.ReadInt16())));

            br.ReadInt16();

            System.Collections.Generic.List< fileData > data = new();
            for (int i = 0; i < fileCount; i++)
                data.Add(new());
            
            for (int i = 0; i < fileCount; i++)
            {
                br.BaseStream.Position = data[i].start;
                Directory.CreateDirectory(Path.GetDirectoryName(args[0]) + "//" + Path.GetFileNameWithoutExtension(args[0]) + "//" + Path.GetDirectoryName(names[i]));
                BinaryWriter bw = new(File.Create(Path.GetDirectoryName(args[0]) + "//" + Path.GetFileNameWithoutExtension(args[0]) + "//" + names[i]));
                bw.Write(br.ReadBytes(data[i].size));
                bw.Close();
            }
        }

        public class fileData
        {
            float unknown = br.ReadSingle();
            int number = br.ReadInt32();
            public int size = br.ReadInt32();
            public int start = br.ReadInt32();
            int unknown2 = br.ReadInt32();
        }
    }
}
