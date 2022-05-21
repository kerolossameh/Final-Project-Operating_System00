using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOS
{
    class Directory : Directory_Entry
    {
        // object of Array to store Directory Table
        public List<Directory_Entry> DirTable;
        // object to store parent (ckeck if there is parent)
        public Directory parent = null;
        // Constractor (by inhretuns)
        public Directory(string namefile, byte attributefile, int firstClusturfile, int sizefile, Directory p)
                    : base
                    (namefile,
                     attributefile,
                     firstClusturfile,
                     sizefile)
        {
            // Store all data of Dir_Entry in Dir_table
            DirTable = new List<Directory_Entry>();
            if (p != null)
            {
                parent = p;
            }
        }

        // return Data of Dir_Entey ()
        public Directory_Entry GetDirectory_Entry()
        {
            // Take object of Dir_Entry => store in me
            Directory_Entry me = new Directory_Entry(new string(fileorDirName), filaAttribute, fileFirstCluster, fileSize);
            return me;
        }


        // Write Directory 
        public void Write_Directory()
        {
            // last index (reach finsh) => -1
            int lastIndex = -1;
            // Initiate Size of Directory Table Bytes
            byte[] DirTableBytes = new byte[32 * DirTable.Count];
            // Initiate size of Directory Entry Bytes
            byte[] DirEntryBytes = new byte[32];
            // Loop dir table's records to convert to array of bytes
            for (int i = 0; i < DirTable.Count; i++)
            {
                // To get each records as bytes
                DirEntryBytes = DirTable[i].GetBytes();
                // Loop on to take DE to store DT (after convert all array of bytes)
                for (int j = i * 32, c = 0; c < 32; c++, j++)
                {
                    // To store records of Dir Entry Bytes in Dir Table Bytes
                    DirTableBytes[j] = DirEntryBytes[c];
                }
            }
            // We will calc the number of blocks taken
            double numOfBlocks = (DirTableBytes.Length / 1024);
            //numOfRequiredBlock (will get the smaller of int num of blocks)
            int numOfRequiredBlock = Convert.ToInt32(Math.Ceiling(numOfBlocks));
            // will take the reminder of length of contentbytes
            int Reminder = (DirTableBytes.Length % 1024);
            int fatIndex; // Temp to Store Cluster ...

            if (numOfRequiredBlock <= Fat.GetAvilaibleBlocks())
            {
                // if you have a first cluster 
                if (this.fileFirstCluster != 0)
                {
                    // we will store it 
                    fatIndex = this.fileFirstCluster;
                }
                // if you dont have
                else
                {
                    // we will get first avaliable block to follow fall by clustering
                    fatIndex = Fat.Getavaliableblock();
                    this.fileFirstCluster = fatIndex;
                }
                // declare list(array) of bytes (all Blocks)
                List<byte[]> Blocks = new List<byte[]>();
                // declare list(array) of bytes (one block)
                byte[] block = new byte[1024];
                // Loop in the size of data will taken (n * 1024 mean each block in one block)
                for (int j = 0; j < numOfRequiredBlock * 1024; j++)
                {

                    if (j % 1024 == 0 && j != 0)
                    {
                        // store each list block in list of BLocks
                        Blocks.Add(block);
                    }
                    block[j % 1024] = DirTableBytes[j];
                }
                // Loop in the size of data (n of block)
                for (int i = 0; i < numOfRequiredBlock; i++)
                {
                    // equal data of each block => fatindex (mean first cluster)
                    VirtualDisk.writeBlock(Blocks[i], fatIndex);
                    // fat index (full) => will equal -1
                    Fat.SetNext(fatIndex, -1);
                    // imposible != -1 , So will enter any way
                    if (lastIndex != -1)
                    {
                        // last index (cluster) => will equal fat index (has reach)
                        Fat.SetNext(lastIndex, fatIndex);
                    }
                    // store the index has reach in last index
                    lastIndex = fatIndex;
                    // store avaliable block in the second index ... (follow full by clustering)
                    fatIndex = Fat.Getavaliableblock();

                }
                // and in the final, we will store in fat table
                Fat.Write_Fat_Table();

            }
        }


        // to read(get)  content
        public void readDirectory()
        {
            // if you have a first cluster 
            if (this.fileFirstCluster != 0)
            {
                // store first cluster in fat index (temp)
                int fatIndex = this.fileFirstCluster;
                // get next this index (value of index)
                int next = Fat.get_Next(fatIndex);
                // declare list(array) of bytes
                List<byte> ls = new List<byte>();
                // object(from Dir_entry) list(array) of bytes
                List<Directory_Entry> dt = new List<Directory_Entry>();
                // Loop to zeros data from Virtual by clustering
                do
                {
                    // store in ls of bytes
                    ls.AddRange(VirtualDisk.readBlock(fatIndex));
                    // store value of next  in fat index (lastindex has reach or temp)
                    fatIndex = next;
                    // if fatndex(last index) not full
                    if (fatIndex != -1)
                    {
                        // will store value of the last index in next
                        next = Fat.get_Next(fatIndex);
                    }
                    // and countinue if the last index not equal -1 (mean has reach in endline)
                } while (next != -1);


                for (int i = 0; i < ls.Count; i++)
                {
                    byte[] b = new byte[32];
                    for (int k = i * 32, m = 0; m < b.Length && k < ls.Count; m++, k++)
                    {
                        b[m] = ls[k];
                    }
                    if (b[0] == 0)
                        break;
                    dt.Add(GetDirectoryEntry(b));
                }

            }

        }

        // to search directory by name and return index
        public int searchDirectory(string name)
        {
            // if name smaller then 11 (as we determine)
            if (name.Length < 11)
            {
                // !!!!!!!!!!!!!
                name += "\0";
                // loop in name of string as charaters
                for (int i = name.Length + 1; i < 12; i++)
                    // store name 
                    name += " ";
            }
            // if greater than 11
            else
            {
                // we will take just first 11 bytes or characters
                name = name.Substring(0, 11);
            }
            // loop in directory table
            for (int i = 0; i < DirTable.Count; i++)
            {
                // store name of each dir => n
                string n = new string(DirTable[i].fileorDirName);
                // if n == name (we research it)
                if (n.Equals(name))
                    // return it
                    return i;
            }
            // if not find return -1 
            return -1;
        }

        // update content by take object from dir entry
        public void updateContent(Directory_Entry d)
        {
            // store name from any dir entry
            string name = new string(d.fileorDirName);
            // get Directories
            readDirectory();
            // serach by name to get index
            int index = searchDirectory(name);
            // if find it
            if (index != -1)
            {
                // remove record of this index 
                DirTable.RemoveAt(index);
                // and insert it again
                DirTable.Insert(index, d);
            }
            // save this change in Fat
            Write_Directory();
        }

        // Delete file Content
        public void deleteDirectory()
        {
            // if you have a first cluster 
            if (this.fileFirstCluster != 0)
            {
                // store first cluster in index (temp )
                int index = this.fileFirstCluster;
                // next (here mean the last index)
                int next = -1;
                // Loop to zeros data from Virtual by clustering
                do
                {
                    // we will set value index => 0 (mean not data here or has removed)
                    Fat.SetNext(index, 0);
                    // store index (last index) in next
                    next = index;
                    // if index not full (mean reach eandline)
                    if (index != -1)
                        // we will store value of this index (last index) => index 
                        index = Fat.get_Next(index);
                    // and countinue if the last index not equal -1 (mean has reach in endline)
                } while (next != -1);
            }
            if (this.parent != null)
            {
                parent.readDirectory();
                int Index = parent.searchDirectory(new string(fileorDirName));
                if (Index != -1)
                {
                    this.parent.DirTable.RemoveAt(Index);
                    this.parent.Write_Directory();
                }
            }
        }

    }
}
