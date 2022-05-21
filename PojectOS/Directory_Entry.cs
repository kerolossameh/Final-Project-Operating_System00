using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOS
{
    class Directory_Entry
    {
        // initiate size of array of char => 11 bytes of charaters
        public char[] fileorDirName = new char[11];
        // declare attr (file or folder)
        public byte filaAttribute;
        // initiate size of array of bytes => 12 bytes
        public byte[] fileEmpty = new byte[12];
        // declare file size (4 bytes becaues you will store int)
        public int fileSize;
        // declare fcluster (4 bytes becaues you will store int)
        public int fileFirstCluster;

        // Contractor
        public Directory_Entry(string name, byte attribute, int firstCluster, int fsize)
        {
            // store att as user input
            filaAttribute = attribute;
            // if 0x0 mean file 
            if (filaAttribute == 0x0)
            {
                // filename will store [name , ext]
                string[] filename = name.Split('.');
                // will check if filename meet my control
                assignFileName(filename[0].ToCharArray(), filename[1].ToCharArray());
            }
            // if 0x10 mean dir
            else
            {
                // will check if dirname meet my control
                assignDIRName(name.ToCharArray());
            }
            // store first cluster
            fileFirstCluster = firstCluster;
            // store size
            fileSize = fsize;
        }

        // take two array of char (name, ext) because each file has ext
        public void assignFileName(char[] name, char[] extension)
        {
            // if len of name = 7  and   len of ext = 3   (as require in section)
            if (name.Length <= 7 && extension.Length == 3)
            {
                // initiate fo use in loop (as counter)
                int j = 0;
                // loop in len of name
                for (int i = 0; i < name.Length; i++)
                {
                    // increasing j (as counter)
                    j++;
                    // store each name in array of char of dir name
                    this.fileorDirName[i] = name[i];
                }
                // to count the last letter after end loop
                j++;
                // after end name append . to store after it ext
                this.fileorDirName[j] = '.';
                // loop on len of ext
                for (int i = 0; i < extension.Length; i++)
                {
                    // counter
                    j++;
                    // add ext after . each name in array of list in dir name
                    this.fileorDirName[j] = extension[i];
                }
                // after j has count (name + . + ext)
                // loop on it
                for (int i = ++j; i < fileorDirName.Length; i++)
                {
                    // if 11 byte not complate store it ' ' (to complate 11)
                    this.fileorDirName[i] = ' ';
                    /*
                    ex 
                    test.txt (8 bytes)
                    this.fileorDirName[i] = ' ';
                    test.txt' ' ' '   (11 bytes) (without single q)
                    */
                }
            }
            // if control false
            else
            {
                // we will take as my require
                // loop first 7 char to take name
                for (int i = 0; i < 7; i++)
                {
                    this.fileorDirName[i] = name[i];
                }
                // after 7 char add . 
                this.fileorDirName[7] = '.';
                // loop after 8 char ext
                for (int i = 0, j = 8; i < extension.Length; j++, i++)
                {
                    // name.ext
                    this.fileorDirName[j] = extension[i];
                }
            }
        }

        // take one array of char (name) because no ext in dir
        public void assignDIRName(char[] name)
        {
            // if len of name = 11 or less  (as require in section)
            if (name.Length <= 11)
            {
                // create counter to control 
                int j = 0;
                // loop on len of name
                for (int i = 0; i < name.Length; i++)
                {
                    // increase counter for each step
                    j++;
                    // store name in list dir name
                    this.fileorDirName[i] = name[i];
                }
                // after it if not complate
                for (int i = ++j; i < fileorDirName.Length; i++)
                {
                    // complate it with space
                    this.fileorDirName[i] = ' ';
                }
            }
            // if control false
            else
            {
                // create counter
                int j = 0;
                // loop on 11 char
                for (int i = 0; i < 11; i++)
                {
                    // increase counter for each step
                    j++;
                    // store name in list dir name
                    this.fileorDirName[i] = name[i];
                }
            }
        }


        // convert record as bytes
        public byte[] GetBytes()
        {

            // size of record
            byte[] b = new byte[32];
            // 11 bytes to store Name
            for (int i = 0; i < 11; i++)
            {
                b[i] = (byte)fileorDirName[i];
            }

            // 11 bytes to store attr
            b[11] = filaAttribute;

            // 12 bytes to store entry
            for (int i = 12, j = 0; i < 24 && j < 12; i++, j++)
            {
                b[i] = fileEmpty[j];
            }

            // 4 bytes to store (int) of First Cluster
            for (int i = 24; i < 28; i++)
            {
                b[i] = (byte)fileFirstCluster;
            }

            // 4 bytes to store (int) of Size file
            for (int i = 28; i < 32; i++)
            {
                b[i] = (byte)fileSize;
            }

            return b;
        }

        public Directory_Entry GetDirectoryEntry(byte[] b)
        {

            for (int i = 0; i < 11; i++)
            {
                fileorDirName[i] = (char)b[i];
            }


            filaAttribute = b[11];

            for (int i = 12, j = 0; i < 24 && j < 12; i++, j++)
            {
                fileEmpty[j] = b[i];
            }

            for (int i = 24; i < 28; i++)
            {
                fileFirstCluster = b[i];
            }

            for (int i = 28; i < 32; i++)
            {
                fileSize = b[i];
            }

            Directory_Entry d1 = new Directory_Entry(new string(fileorDirName), filaAttribute, fileFirstCluster, fileSize);
            return d1;
        }
    }
}
