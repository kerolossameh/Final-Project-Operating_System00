using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOS
{
    class Fat
    {
        static int[] FatTable = new int[1024];
        static byte[] arrOfByte = new byte[4 * 1024];

        public Fat()
        {
            FatTable = new int[1024];
        }

        public static void Initialize()
        {
            for (int i = 0; i < FatTable.Length; i++)
            {
                if (i < 5)
                    FatTable[i] = -1;
                else
                    FatTable[i] = 0;
            }



        }

        public static void Write_Fat_Table()
        {
            VirtualDisk.VDisk = new FileStream("File.txt", FileMode.Open, FileAccess.Write);
            VirtualDisk.VDisk.Seek(1024, SeekOrigin.Begin);

            // Buffer.BlockCopy(FatTable, 0, arrOfByte, 0, arrOfByte.Length);

            VirtualDisk.VDisk.Write(arrOfByte, 0, arrOfByte.Length);
            VirtualDisk.VDisk.Close();


        }

        public static int[] get_fat_table()
        {
            VirtualDisk.VDisk = new FileStream("File.txt", FileMode.Open, FileAccess.Read);
            VirtualDisk.VDisk.Seek(1024, SeekOrigin.Begin);
            VirtualDisk.VDisk.Read(arrOfByte, 0, arrOfByte.Length);
            Buffer.BlockCopy(arrOfByte, 0, FatTable, 0, 4096);
            VirtualDisk.VDisk.Close();
            return (FatTable);
        }

        public void Print_fat_table()
        {

            get_fat_table();

            for (int i = 0; i < FatTable.Length; i++)
            {
                Console.WriteLine((i + 1) + "\t-->\t" + FatTable[i]);
            }

        }

        public static int Getavaliableblock()
        {
            int freeIndex = -1;
            for (int i = 0; i < 1024; i++)
            {
                if (FatTable[i] == 0)
                {
                    freeIndex = i;
                    break;
                }

            }
            return freeIndex;
        }

        public static int GetAvilaibleBlocks()
        {
            int count = 0;
            for (int i = 0; i < FatTable.Length; i++)
            {
                if (FatTable[i] == 0)
                {
                    count++;
                }
            }
            return count;
        }

        public static int get_Next(int index)
        {
            return (FatTable[index]);
        }

        public static void SetNext(int index, int value)
        {
            FatTable[index] = value;
        }

        public static int GetAvailableBlocks()
        {
            return GetAvilaibleBlocks() * 1024;
        }
    }
}
