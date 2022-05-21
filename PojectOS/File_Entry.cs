using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOS
{
    class File_Entry : Directory_Entry
    {
        // declare variable to store content
        public string content;
        // declare variable (as object) to store parent
        public Directory parent;

        // Constractor 
        public File_Entry(string name, byte f_attr, int f_firstCluster, int f_size, string f_content, Directory pa) : base(name, f_attr, f_firstCluster, f_size)
        {
            content = f_content;
            if (pa != null)
                parent = pa;
        }

        // method to write file content
        public void writeFileContent()
        {
            // Take all of content and convert to bytes
            byte[] contentBYTES = Encoding.ASCII.GetBytes(content);
            // We will calc the number of blocks taken
            double numOfBlocks = contentBYTES.Length / 1024;
            // will get the smaller of int num of blocks
            int numOfRequiredBlock = Convert.ToInt32(Math.Ceiling(numOfBlocks));
            // will get the grater of num of blocks
            int numOfFullSizeBlock = Convert.ToInt32(Math.Floor(numOfBlocks));
            // will take the reminder of length of contentbytes
            double reminder = contentBYTES.Length % 1024;
            // initiate 
            int fatIndex = 0;
            // initiate 
            int lastIndex = -1;
            // will follow fall data by cluster
            if (numOfRequiredBlock <= Fat.GetAvilaibleBlocks())
            {
                // if you have a first cluster 
                if (fileFirstCluster != 0)
                {
                    // we will store it 
                    fatIndex = fileFirstCluster;
                }
                // if you dont have
                else
                {
                    // we will get first avaliable block to follow fall by clustering
                    fileFirstCluster = Fat.Getavaliableblock();
                    // store avaliable block in the second index ... (follow full by clustering)
                    fatIndex = Fat.Getavaliableblock();
                }
            }
            // Loop in number of block  
            for (int i = 0; i < numOfFullSizeBlock; i++)
            {
                // write data as bytes , in determine fat index
                VirtualDisk.writeBlock(contentBYTES, fatIndex);
                // fat index (full) => will equal -1
                Fat.SetNext(fatIndex, -1);
                // imposible != -1 , So will enter any way
                if (lastIndex != -1)
                {
                    // store the index has reach in last index
                    lastIndex = fatIndex;
                    // last index (cluster) => will equal fat index (has reach)
                    Fat.SetNext(lastIndex, fatIndex);
                }
                // we will get first avaliable block to follow fall by clustering
                fatIndex = Fat.Getavaliableblock();
                // and in the final, we will store in fat table
                Fat.Write_Fat_Table();
            }
        }

        // declare list(array) of bytes
        List<byte> ls;

        // to read(get) file content
        public void readFileContent()
        {
            // store first cluster in fatindex (temp)
            int fatIndex = fileFirstCluster;
            // get next this index (value of index) 
            int next = Fat.get_Next(fatIndex);
            // if you have a first cluster 
            if (fileFirstCluster != 0)
            {
                // Loop to get data from Virtual by clustering
                do
                {
                    // store in ls of bytes
                    ls.AddRange(VirtualDisk.readBlock(fatIndex));
                    // store value of index  in fat index
                    fatIndex = next;
                    // if not reach the final 
                    if (fatIndex != -1)
                    {
                        // will store value of the last index in next
                        next = Fat.get_Next(fatIndex);
                    }
                    // and countinue if the last index not equal -1 (mean has reach in endline)
                } while (next != -1);

            }
        }

        // Delete file Content
        public void deleteFileContent()
        {
            // if you have a first cluster 
            if (fileFirstCluster != 0)
            {
                // store first cluster in index (temp)
                int index = fileFirstCluster;
                // next (here mean the last index)
                int next = -1;
                // Loop to zeros data from Virtual by clustering
                do
                {
                    // we will set value index => 0 (mean not data here or has removed)
                    Fat.SetNext(index, 0);
                    // store index (last index) in next
                    next = index;
                    // if index not full
                    if (index != -1)
                        // we will store value of this index => index
                        index = Fat.get_Next(index);
                    // countinue
                } while (next != -1);
            }
            // to check if the parent 
            // if no parent
            if (parent != null)
            {
                parent.readDirectory();
                int Index = parent.searchDirectory(fileorDirName.ToString());
                if (Index != -1)
                {
                    parent.DirTable.RemoveAt(Index);
                    parent.Write_Directory();
                }
            }
        }

    }
}
