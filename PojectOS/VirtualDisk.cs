using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOS
{
    class VirtualDisk
    {
        // object from File Stream to open file 
        public static FileStream VDisk;
        // Create VDisk by take path (name.ext)
        public static void CreateDisk(string path)
        {
            VDisk = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            VDisk.Close();
        }

        // public static int getFreeSpace()
        // {
        //     return (1024 * 1024) - (int)VDisk.Length;
        // }

        public static void initalize(string path)
        {

            if (!File.Exists(path))
            {
                CreateDisk(path);
                byte[] superblock = new byte[1024];
                for (int i = 0; i < superblock.Length; i++)
                {
                    superblock[i] = 0;
                }
                writeBlock(superblock, 0);
                Fat f = new Fat();
                Fat.Initialize();
                Directory root = new Directory("MyProject:>", 0x10, 5, 0, null);
                root.Write_Directory();
                Fat.SetNext(5, -1);
                Program.CurrentDir = root;
                Fat.Write_Fat_Table();
            }
            else
            {

                Fat.get_fat_table();

                Directory root = new Directory("MyProject:>", 0x10, 5, 0, null);

                root.readDirectory();

                Program.CurrentDir = root;

            }

        }

        public static void writeBlock(byte[] data, int Index, int offset = 0, int count = 1024)
        {
            VDisk = new FileStream("File.txt", FileMode.Open, FileAccess.Write);
            VDisk.Seek(Index * 1024, SeekOrigin.Begin);
            VDisk.Write(data, offset, count);
            VDisk.Flush();
            VDisk.Close();
        }

        public static byte[] readBlock(int clusterIndex)
        {
            VDisk = new FileStream("File.txt", FileMode.Open, FileAccess.Read);
            VDisk.Seek(clusterIndex * 1024, SeekOrigin.Begin);
            byte[] bytes = new byte[1024];
            VDisk.Read(bytes, 0, 1024);
            VDisk.Close();
            return bytes;
        }
    }
}
