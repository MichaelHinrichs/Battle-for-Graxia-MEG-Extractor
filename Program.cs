//Written for Battle for Graxia. https://steamcommunity.com/app/90530/
using System.IO;
using System.IO.Compression;

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

            System.Collections.Generic.List<FileData> data = new();
            for (int i = 0; i < fileCount; i++)
                data.Add(new());

            string path = Path.GetDirectoryName(args[0]) + "//" + Path.GetFileNameWithoutExtension(args[0]) + "//";
            for (int i = 0; i < fileCount; i++)
            {
                br.BaseStream.Position = data[i].start;
                Directory.CreateDirectory(path + Path.GetDirectoryName(names[i]));
                BinaryWriter bw = new(File.Create(path + "//" + names[i]));
                bw.Write(br.ReadBytes(data[i].size));
                bw.Close();

                switch (Path.GetExtension(names[i]))
                {
                    case ".APF":
                    case ".CPD":
                    case ".GPD":
                    case ".SOB":
                    case ".TED":
                    case ".TER":
                    case ".ALO":
                    case ".ALA":
                        Decompress(path + "//" + names[i]);
                        break;

                }
            }
        }

        public class FileData
        {
            float unknown = br.ReadSingle();
            int number = br.ReadInt32();
            public int size = br.ReadInt32();
            public int start = br.ReadInt32();
            int unknown2 = br.ReadInt32();
        }

        public static void Decompress(string file)
        {
            BinaryReader decmps = new(File.OpenRead(file));
            decmps.BaseStream.Position = 16;
            int size = decmps.ReadInt32();
            decmps.BaseStream.Position += 18;
            string path = Path.GetDirectoryName(file) + "\\extracted\\";
            Directory.CreateDirectory(path);
            FileStream fs = File.Create(path + Path.GetFileName(file));
            using (var ds = new DeflateStream(new MemoryStream(decmps.ReadBytes((size - 2))), CompressionMode.Decompress))
                ds.CopyTo(fs);

            fs.Close();
        }
    }
}
