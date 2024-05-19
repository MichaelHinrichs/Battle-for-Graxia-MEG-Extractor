//Written for games that use Megafiles.
//Battle for Graxia https://steamcommunity.com/app/90530/
//Victory Command https://steamcommunity.com/app/360480/
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Megafile_Extractor
{
    class Program
    {
        static BinaryReader br;
        static void Main(string[] args)
        {
            br = new BinaryReader(File.OpenRead(args[0]));

            if (new string(System.Text.Encoding.GetEncoding("ISO-8859-1").GetChars(br.ReadBytes(8))) != "ÿÿÿÿ¤p}?")
                throw new System.Exception("This is not a Megafile.");

            int fileDataTableStart = br.ReadInt32();
            int fileCount = br.ReadInt32();
            int namesCount = br.ReadInt32();
            int fileTableStart = br.ReadInt32();

            List<string> names = new();
            for (int i = 0; i < namesCount; i++)
                names.Add(new(br.ReadChars(br.ReadInt16())));

            br.ReadInt16();

            List<FileData> data = new();
            for (int i = 0; i < fileCount; i++)
                data.Add(new());

            string path = Path.GetDirectoryName(args[0]);
            int n = 0;
            foreach (FileData file in data)
            {
                br.BaseStream.Position = file.start;
                BinaryWriter bw;
                if (Path.GetDirectoryName(names[n]) != "")
                {
                    Directory.CreateDirectory(path + "//" + Path.GetDirectoryName(names[n]));
                    bw = new(File.Create(path + "//" + names[n]));
                }
                else
                {
                    Directory.CreateDirectory(path + "//Data//" + Path.GetFileNameWithoutExtension(args[0]));
                    bw = new(File.Create(path + "//Data//" + Path.GetFileNameWithoutExtension(args[0]) + "//" + names[n]));
                }
                bw.Write(br.ReadBytes(file.size));
                bw.Close();

                switch (Path.GetExtension(names[n]))
                {
                    case ".APF":
                    case ".CPD":
                    case ".GPD":
                    case ".SOB":
                    case ".TED":
                    case ".TER":
                    case ".ALO":
                    case ".ALA":
                        Decompress(path + "//" + names[n]);
                        break;

                }
                n++;
            }
        }

        class FileData
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
