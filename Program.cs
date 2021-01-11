using System;
using System.IO;
using System.Collections.Generic;

namespace CMP1903M_3
{
    //Compare class that handles the comparison of the two files
    class Compare
    {
        //Fields encapulated by using properties and private access modifiers
        public static StreamReader TextFile1 { get; set; }
        public static StreamReader TextFile2 { get; set; }
        private static string[] FirstTextLines { get; set; }
        private static string[] SecondTextLines { get; set; }
        private static string FirstFileContents { get; set; }
        private static string SecondFileContents { get; set; }
        private static string Directory { get; set; }
        private static string FilePath { get; set; }

        private static List<string> TotalAdded = new List<string>();
        private static List<string> TotalRemoved = new List<string>();
        private static List<int> DifferentLines = new List<int>();

        private static string LogFileName;
        

        //Constructor for Compare class to define fields using parameters
        public Compare(StreamReader File1, StreamReader File2, string[] FirstText, string[] SecondText, string Direct, string FilesPath, string File1Name, string File2Name)
        {
            TextFile1 = File1;
            TextFile2 = File2;
            FirstFileContents = File1.ReadToEnd();
            SecondFileContents = File2.ReadToEnd();
            FirstTextLines = FirstText;
            SecondTextLines = SecondText;
            Directory = Direct;
            FilePath = FilesPath;
            LogFileName = "diff " + File1Name + ".txt " + File2Name + ".txt";
            CompareFiles();
        }

        public string ReturnLogFilePath()
        {
            string LogFilePath = FilePath + LogFileName + ".txt";
            return LogFilePath;
        }
        //All methods set as private for abstraction

        //Method of initial check to see if both files are identical
        private static void CompareFiles()
        {
            if (FirstFileContents != SecondFileContents)
            {
                CompareLines();
            }
            else
            {
                Console.WriteLine("[Output] These text files are not different");
            }
        }

        //Function that Compares the files line by line. Lines which are different then use the CompareWords function
        private static void CompareLines()
        {
            int Counter = 0;

            int MaxIteration = Math.Max(FirstTextLines.Length, SecondTextLines.Length);

            for (int i = 0; i < MaxIteration; i++)
            {
                try
                {
                    if (FirstTextLines[i] != SecondTextLines[i])
                    {
                        //Stores the number of the different line
                        DifferentLines.Add(i + 1);
                        //Passes the current lines to the CompareWords function
                        CompareWords(FirstTextLines[i], SecondTextLines[i], Counter);
                        Counter++;
                    }
                }
                catch
                {
                    try
                    {
                        if ("" != SecondTextLines[i])
                        {
                            //Stores the number of the different line
                            DifferentLines.Add(i + 1);
                            //Passes the current lines to the CompareWords function
                            CompareWords("", SecondTextLines[i], Counter);
                            Counter++;
                        }
                    }
                    catch
                    {
                        if (FirstTextLines[i] != "")
                        {
                            //Stores the number of the different line
                            DifferentLines.Add(i + 1);
                            //Passes the current lines to the CompareWords function
                            CompareWords(FirstTextLines[i], "", Counter);
                            Counter++;
                        }
                    }
                }
            }
        }

        //Subroutine to compare the words from the different lines to determine how the lines are different
        private static void CompareWords(string FirstLine, string SecondLine, int Counter)
        {
            List<string> TempAdded = new List<string>();
            List<string> TempRemoved = new List<string>();

            string[] FirstLineWords = FirstLine.Split(" ");
            string[] SecondLineWords = SecondLine.Split(" ");

            int MaxIteration = Math.Max(FirstLineWords.Length, SecondLineWords.Length);

            /*So I'm going to be honest, this is far from an elegant and optimised solution to the problem.
             * I was unsure how to code a longest common subsequence algorithm properly
             * so I developed this rudimentary try-catch-mess method which (I believe) does the job.*/

            for (int i = 0; i < MaxIteration; i++)
            {
                try
                {
                    //Iterates through the lines and checks if the words are the same
                    if (FirstLineWords[i] != SecondLineWords[i])
                    {
                        //If not the same, the temp list is checked to see if the word appeared beforehand. If so, it is removed from the temp list
                        if (TempRemoved.Contains(SecondLineWords[i]))
                        {
                            TempRemoved.Remove(SecondLineWords[i]);
                        }
                        else if (TempAdded.Contains(FirstLineWords[i]))
                        {
                            TempAdded.Remove(FirstLineWords[i]);
                        }
                        //If the word is not found in a temp list, it is added to the correct one
                        else
                        {
                            TempRemoved.Add(FirstLineWords[i]);
                            TempAdded.Add(SecondLineWords[i]);
                        }
                    }
                /*This try-catch method/mess is my solution to if the lines are different lengths.
                 If one line is longer then the other, this exception handling is continued until the whole of both lines are read*/
                }
                catch
                {
                    try
                    {
                        if (" " != SecondLineWords[i])
                        {
                            if (TempRemoved.Contains(SecondLineWords[i]))
                            {
                                TempRemoved.Remove(SecondLineWords[i]);
                            }
                            else if (TempAdded.Contains(""))
                            {
                                TempAdded.Remove(FirstLineWords[i]);
                            }
                            else
                            {
                                TempRemoved.Add(FirstLineWords[i]);
                                TempAdded.Add(SecondLineWords[i]);
                            }
                        }
                    }
                    catch
                    {
                        try
                        {
                            if (FirstLineWords[i] != " ")
                            {
                                if (TempRemoved.Contains(" "))
                                {
                                    TempRemoved.Remove(SecondLineWords[i]);
                                }
                                else if (TempAdded.Contains(FirstLineWords[i]))
                                {
                                    TempAdded.Remove(FirstLineWords[i]);
                                }
                                else
                                {
                                    TempRemoved.Add(FirstLineWords[i]);
                                    TempAdded.Add("");
                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }

            //Adds the temp list to the total list then clears the temp list ready for the next line to compare
            foreach (string Word in TempRemoved)
            {
                TotalRemoved.Add(Word);
            }
            TempRemoved.Clear();

            foreach (string Word in TempAdded)
            {
                TotalAdded.Add(Word);
            }
            TempAdded.Clear();

            //Displays the output to the user and writes it to the log file
            DisplayComparison(Counter, FirstLineWords, SecondLineWords);
            WriteComparison(Counter, FirstLineWords, SecondLineWords);
            
        }

        private static void WriteComparison(int Counter, string[] FirstLineWords, string[] SecondLineWords)
        {
            //This function uses the same method as the "DisplayComparisons" method except the output is stored in the log file rather than displayed in the console

            //These next 4 5 lines are all setup to ensure the console can be logged to the text file
            string LogFilePath = FilePath + LogFileName + ".txt";
            FileStream fileStream = new FileStream(LogFilePath, FileMode.Append, FileAccess.Write);
            TextWriter OldOut = Console.Out;
            StreamWriter Writer = new StreamWriter(fileStream);
            Console.SetOut(Writer);

            Console.WriteLine("\n[Output] Line: {0}", DifferentLines[Counter]);

            if (TotalAdded.Count > 0)
            {
                Console.Write("[Output] + ");
                foreach (string x in SecondLineWords)
                {
                    if (TotalAdded.Contains(x))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("{0} ", x);
                    }
                    else
                    {
                        Console.Write("{0} ", x);
                    }
                    Console.ResetColor();
                }
                
            }
            if (TotalRemoved.Count > 0)
            {
                Console.Write("\n[Output] - ");
                foreach (string x in FirstLineWords)
                {
                    if (TotalRemoved.Contains(x))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("{0} ", x);
                    }
                    else
                    {
                        Console.Write("{0} ", x);
                    }
                    Console.ResetColor();
                }
            }

            
            //Stores the console output in the text file and closes it
            Console.SetOut(OldOut);
            Writer.Close();
            fileStream.Close();
            

            TotalAdded.Clear();
            TotalRemoved.Clear();
        }

        //Function to display output to the user in the console
        private static void DisplayComparison(int Counter, string[] FirstLineWords, string[] SecondLineWords)
        {
            //Displays what line the following differences are on
            Console.WriteLine("\n[Output] Line: {0}", DifferentLines[Counter]);

            //Only continues if there are additions to the line
            if (TotalAdded.Count > 0)
            {
                Console.Write("[Output] + ");

                //Iterates through the whole line
                foreach (string x in SecondLineWords)
                {
                    //If the word is in the list of added items, the colour of the text is changed to green
                    if (TotalAdded.Contains(x))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("{0} ", x);
                    }
                    //If not, then the text remains white
                    else
                    {
                        Console.Write("{0} ", x);
                    }
                    //Resets the colour to ensure default is white
                    Console.ResetColor();
                }

            }
            //Only continues if items from the line have been removed
            if (TotalRemoved.Count > 0)
            {
                Console.Write("\n[Output] - ");

                //Iterates through the whole line
                foreach (string x in FirstLineWords)
                {
                    //If the word is in the list of removed items, the colour of the text is changed to red
                    if (TotalRemoved.Contains(x))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("{0} ", x);
                    }
                    //If not, then the text remains white
                    else
                    {
                        Console.Write("{0} ", x);
                    }
                    //Resets the colour to ensure the default is white
                    Console.ResetColor();
                }
            }
        }
    }

    class MainClass
    {
        static void Main()
        { 
            //User enters the name of the two text files to compare, names are stored in string format
            Console.Write("\nEnter the first text file to compare (Do not include .txt): ");
            string File1Name = Console.ReadLine();

            Console.Write("Enter the second text file to compare (Do not include .txt): ");
            string File2Name = Console.ReadLine();

            //Gets the current directory
            string Directory = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            string FilePath = Path.GetFullPath(Path.Combine(Directory, @"..\"));

            //Stores the path of the files the user searched for
            string File1Path = FilePath + File1Name + ".txt";
            string File2Path = FilePath + File2Name + ".txt";

            //Displays the input to the user
            Console.WriteLine("[Input] diff {0}.txt {1}.txt", File1Name, File2Name);

            //Try catch in case the user enters an invalid text file
            try
            {
                System.IO.StreamReader File1 = File.OpenText(File1Path);
                System.IO.StreamReader File2 = File.OpenText(File2Path);

                //Here the Comparison object is instantiated using the Compare class and the file details as parameters for the constructor
                Compare Comparison1 = new Compare(File1, File2, File.ReadAllLines(File1Path), File.ReadAllLines(File2Path), Directory, FilePath, File1Name, File2Name);
                string LogFilePath = Comparison1.ReturnLogFilePath();
                Console.WriteLine("\n[Output] Log file stored at {0}", LogFilePath);
            }
            catch
            {
                Console.WriteLine("\nPlease enter the name of a valid text file in the current directory");
                Main();
            }

            
        }
    }
}
