
namespace ProjectOS
{
    class commands
    {
        public static void cls()
        {
            Console.Clear();
        }

        public static void help()
        {
            Console.WriteLine("*cd\t\t Display the name of or change the current directory");
            Console.WriteLine("*cls\t\t Clear the screen");
            Console.WriteLine("*dir\t\t List the contents of directory");
            Console.WriteLine("*quit\t\t Quits the CMD.exe program");
            Console.WriteLine("*help\t\t Provides help information for windows command");
            Console.WriteLine("*md\t\t Creates a directory");
            Console.WriteLine("*rd\t\t Removes a directory");
            Console.WriteLine("copy\t\t Copies one or more files to another location");
            Console.WriteLine("del\t\t Deletes one or more files");
            Console.WriteLine("rf\t\t Removes a file");
            Console.WriteLine("type\t\t Displays the content of a text file");
            Console.WriteLine("import\t\t import text file from your computer");
            Console.WriteLine("export\t\t export text file to your computer");

        }

        public static void quit()
        {
            System.Environment.Exit(1);
        }

        public static void md(string name)
        {
            if (Program.CurrentDir.searchDirectory(name) == -1)
            {
                Directory_Entry newdirectory = new Directory_Entry(name, 0x10, 0, 0);
                Program.CurrentDir.DirTable.Add(newdirectory);
                Program.CurrentDir.Write_Directory();
                if (Program.CurrentDir.parent != null)
                {
                    Program.CurrentDir.parent.updateContent(Program.CurrentDir.parent);
                    Program.CurrentDir.parent.Write_Directory();
                }
            }
            else
            {
                Console.WriteLine("A subdirectory or file " + name + " already exists.");
            }
        }

        public static void rd(string name)
        {
            int index = Program.CurrentDir.searchDirectory(name);
            if (index != -1)
            {
                int firstCluster = Program.CurrentDir.DirTable[index].fileFirstCluster;
                Directory d1 = new Directory(name, 0x10, firstCluster, 0, Program.CurrentDir);
                d1.deleteDirectory();
                Program.currentPath = new string(Program.CurrentDir.fileorDirName).Trim();
            }
            else
            {
                Console.WriteLine("The system cannot find the path specified.");
            }
        }

        public static void cd(string name)
        {
            int index = Program.CurrentDir.searchDirectory(name);

            if (index != -1)
            {
                int firstCluster = Program.CurrentDir.DirTable[index].fileFirstCluster;
                Directory d1 = new Directory(name, 0x10, firstCluster, 0, Program.CurrentDir);


                Program.currentPath = new string(Program.CurrentDir.fileorDirName).Trim() + "\\" + new string(d1.fileorDirName).Trim() + ">";
                Program.CurrentDir.readDirectory();
            }
            else
            {
                Console.WriteLine("The system cannot find the path specified.");
            }
        }
        public static void dir()
        {
            int counterDirectory = 0, counterfiles = 0, filesizecounter = 0;
            Console.WriteLine("Directory of " + Program.currentPath);
            for (int i = 0; i < Program.CurrentDir.DirTable.Count; i++)
            {
                if (Program.CurrentDir.filaAttribute == 0x0)
                {
                    Console.WriteLine(Program.CurrentDir.DirTable[i].fileSize + "  " + Program.CurrentDir.DirTable[i].fileorDirName);
                    counterfiles++;
                    filesizecounter += Program.CurrentDir.DirTable[i].fileSize;
                }
                else
                {
                    Console.Write("<dir>" + "      ");
                    Console.WriteLine(Program.CurrentDir.DirTable[i].fileorDirName);
                    counterDirectory++;
                }
            }
            Console.WriteLine(counterfiles + " File(s)       " + filesizecounter + " bytes");
            Console.WriteLine(counterDirectory + " Dir(s)   " + Fat.GetAvailableBlocks() + "  bytes Free");
        }
        public static void import(string path)
        {
            if (File.Exists(path))
            {
                int start_name = path.LastIndexOf("\\");
                string name = path.Substring(start_name + 1);
                string content = File.ReadAllText(path);
                int size = content.Length;
                int index = Program.CurrentDir.searchDirectory(name);
                if (index == -1)
                {
                    int firstCluster;
                    if (size > 0)
                    {

                        firstCluster = Fat.Getavaliableblock();
                    }
                    else
                    {
                        firstCluster = 0;
                    }
                    File_Entry f1 = new File_Entry(name, 0x0, firstCluster, 0, content, Program.CurrentDir);
                    f1.writeFileContent();
                    Directory_Entry d1 = new Directory_Entry(name, 0x0, firstCluster, 0);
                    Program.CurrentDir.DirTable.Add(d1);
                    Program.CurrentDir.Write_Directory();
                }
                else
                {
                    Console.WriteLine("This file already exist");
                }
            }
            else
            {
                Console.WriteLine("This file is not exist");
            }
        }
        public static void type(string name)
        {
            int index = Program.CurrentDir.searchDirectory(name);
            if (index != -1)
            {
                int first_cluster = Program.CurrentDir.DirTable[index].fileFirstCluster;
                int filesize = Program.CurrentDir.DirTable[index].fileSize;
                string content = null;
                File_Entry f = new File_Entry(name, 0x0, first_cluster, filesize, content, Program.CurrentDir);

                f.readFileContent();
                Console.WriteLine(f.content);
            }
            else
            {
                Console.WriteLine("The system can't find the file ");
            }
        }
        public static void export(string source, string destination)
        {
            int index = Program.CurrentDir.searchDirectory(source);
            if (index != -1)
            {
                if (System.IO.Directory.Exists(destination))
                {
                    int first_cluster = Program.CurrentDir.DirTable[index].fileFirstCluster;
                    int filesize = Program.CurrentDir.DirTable[index].fileSize;
                    string temp = null;
                    File_Entry f = new File_Entry(source, 0x0, first_cluster, filesize, temp, Program.CurrentDir);
                    f.readFileContent();

                    StreamWriter sw = new StreamWriter(destination + "\\" + source);
                    sw.Write(f.content);
                    sw.Flush();
                    sw.Close();
                }
                else
                {
                    Console.WriteLine("the system can't find this path in computer Disk");
                }
            }
            else
            {
                Console.WriteLine("This file doesn't exist");
            }
        }
        // public static void rename(string oldname, string newname)
        // {
        //     int index = Program.CurrentDir.searchDirectory(oldname);
        //     if (index != -1)
        //     {
        //         int n = Program.CurrentDir.searchDirectory(newname);
        //         if (n != -1)
        //         {

        //             Directory_Entry d = new Directory_Entry();
        //             d = Program.CurrentDir.DirTable[index];
        //             d.file_name = newname;
        //             Program.CurrentDir.DirTable.RemoveAt(index);
        //             Program.CurrentDir.DirTable.Insert(n, d);
        //             Program.CurrentDir.Write_Directory();
        //         }
        //         else
        //         {
        //             Console.WriteLine("A duplicate file name exists, or the file cannot be found.");
        //         }

        //     }
        //     else
        //     {
        //         Console.WriteLine(" The system cannot find the file specified.");
        //     }

        // }
        public static void del(string name)
        {
            int index = Program.CurrentDir.searchDirectory(name);
            if (index != -1)
            {
                int f = Program.CurrentDir.DirTable[index].filaAttribute;
                if (f == 0x0)
                {
                    int first_cluster = Program.CurrentDir.DirTable[index].fileFirstCluster;
                    int file_size = Program.CurrentDir.DirTable[index].fileSize;
                    File_Entry d = new File_Entry(name, 0x0, first_cluster, 0, null, Program.CurrentDir);
                    d.deleteFileContent();
                }
                else
                {
                    Console.WriteLine(" The system cannot find the file specified. ");
                }
            }
            else
            {
                Console.WriteLine(" The system cannot find the file specified. ");
            }
        }
        public static void copy(string num1, string num2)
        {
            int index1 = Program.CurrentDir.searchDirectory(num1);
            if (index1 != -1)
            {
                int start_index = num2.LastIndexOf("\\");
                string name = num2.Substring(start_index + 1);

                int index_destenation = Program.CurrentDir.searchDirectory(name);
                if (index_destenation == -1)
                {

                    if (num2 != Program.CurrentDir.fileorDirName.ToString())
                    {
                        int firstcluster = Program.CurrentDir.DirTable[index1].fileFirstCluster;
                        int f_size = Program.CurrentDir.DirTable[index1].fileSize;
                        Directory_Entry entry = new Directory_Entry(num1.ToString(), 0x0, firstcluster, f_size);
                        Directory dir = new Directory(num2.ToString(), 0x10, firstcluster, f_size, Program.CurrentDir.parent);
                        dir.DirTable.Add(entry);


                    }
                    else Console.WriteLine("not fff");

                }
            }
        }
    }

    class Program
    {
        public static Directory CurrentDir;
        public static string currentPath;

        static void Main(string[] args)
        {
            String test = "File.txt";
            File.Delete(test);
            VirtualDisk.initalize(test);
            currentPath = new string(CurrentDir.fileorDirName);
            while (true)
            {
                Console.Write(currentPath.Trim());
                string inputuser = Console.ReadLine();
                if (!inputuser.Contains(" "))
                {
                    if (inputuser.ToLower() == "help")
                    {
                        commands.help();
                    }
                    else if (inputuser.ToLower() == "quit")
                    {
                        commands.quit();
                    }
                    else if (inputuser.ToLower() == "cls")
                    {
                        commands.cls();
                    }
                    else if (inputuser.ToLower() == "md")
                    {
                        Console.WriteLine("The syntax of the command is incorrect.");
                    }
                    else if (inputuser.ToLower() == "rd")
                    {
                        Console.WriteLine("The syntax of the command is incorrect.");
                    }
                    else if (inputuser.ToLower() == "dir")
                    {
                        commands.dir();
                    }
                    else if (inputuser.ToLower() == "import")
                    {
                        Console.WriteLine("The syntax of the command is incorrect.");
                    }
                    else if (inputuser.ToLower() == "type")
                    {
                        Console.WriteLine("The syntax of the command is incorrect.");
                    }
                    else if (inputuser.ToLower() == "export")
                    {
                        Console.WriteLine("The syntax of the command is incorrect.");
                    }
                    else if (inputuser.ToLower() == "del")
                    {
                        Console.WriteLine("The syntax of the command is incorrect.");
                    }
                    else if (inputuser.ToLower() == "copy")
                    {
                        Console.WriteLine("The syntax of the command is incorrect.");
                    }
                    else
                    {
                        Console.WriteLine("Invald Command");
                    }

                }
                else if (inputuser.Contains(" "))
                {
                    string[] arrInput = inputuser.Split(" ");
                    if (arrInput[0] == "md")
                    {
                        commands.md(arrInput[1]);
                    }
                    else if (arrInput[0] == "rd")
                    {
                        commands.rd(arrInput[1]);
                    }
                    else if (arrInput[0] == "cd")
                    {
                        commands.cd(arrInput[1]);
                    }
                    else if (arrInput[0] == "import")
                    {
                        commands.import(arrInput[1]);
                    }
                    else if (arrInput[0] == "type")
                    {
                        commands.type(arrInput[1]);
                    }
                    else if (arrInput[0] == "export")
                    {
                        commands.export(arrInput[1], arrInput[1]);
                    }
                    else if (arrInput[0] == "del")
                    {
                        commands.del(arrInput[1]);
                    }
                    else if (arrInput[0] == "copy")
                    {
                        commands.copy(arrInput[1], arrInput[1]);
                    }
                    else
                    {
                        Console.WriteLine("Invald Command");
                    }
                }
            }
        }
    }
}