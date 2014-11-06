using System;
using System.IO;

class FileStream
{
    //counts the number of sections
    static int countSec(string fileName)
    {
        StreamReader file = new StreamReader(fileName);
        int num = 0;
        while (!file.EndOfStream)
        {
            string line = file.ReadLine().Trim();
            if (line.StartsWith("{{") && line.EndsWith("}}") && !line.StartsWith(@"{{ \"))
                num++;
        }
        return num;
    }

    //returns the titles of all the sections
    static string[] findTitle(string fileName, int size)
    {
        StreamReader file = new StreamReader(fileName);
        string[] ar = new string[size];
        int counter = 0;
        while (!file.EndOfStream)
        {
            string line = file.ReadLine().Trim();
            if (line.StartsWith("{{") && line.EndsWith("}}") && !line.StartsWith(@"{{ \"))
            {
                string[] splitLine = line.Split(' ');
                int max = splitLine.Length - 1;
                for (int i = 1; i < max; i++)
                    ar[counter] += (splitLine[i] + " ");
                ar[counter] = ar[counter].Trim();
                counter++;
            }
        }
        return ar;
    }
    static void Main()
    {
        string bf = @"C:\Users\Alex Wen\Desktop\nginx-1.7.6\html\body.html";
        StreamReader bodyFile = new StreamReader(bf);
        
        //this finds how many sections there are
        int numSections = countSec(bf); //this works
        Console.WriteLine(numSections);

        //this finds the names of all the titles
        string[] sectionTitle = findTitle(bf, numSections); //this works
        
        for (int i = 0; i < numSections; i++)
        {
            string outFile = @"C:\Users\Alex Wen\Desktop\nginx-1.7.6\html\" + sectionTitle[i] + @"\";
            Directory.CreateDirectory(outFile);
            outFile += @"index.html";

            StreamWriter writeFile = new StreamWriter(outFile);
            StreamReader templateFile = new StreamReader(@"C:\Users\Alex Wen\Desktop\nginx-1.7.6\html\index.templ");

            while (!templateFile.EndOfStream)
            {
                string line = templateFile.ReadLine();
                string trimmedLine = line.Trim();
                if (trimmedLine == "{{ body }}")
                {
                    bodyFile = new StreamReader(bf);
                    while (!bodyFile.EndOfStream)
                    {
                        line = bodyFile.ReadLine();
                        trimmedLine = line.Trim();
                        if (trimmedLine == ("{{ " + sectionTitle[i] + " }}")){
                            line = bodyFile.ReadLine();
                            trimmedLine = line.Trim();
                            while (trimmedLine != (@"{{ \" + sectionTitle[i] + " }}"))
                            {
                                writeFile.WriteLine(line);
                                line = bodyFile.ReadLine();
                                trimmedLine = line.Trim();
                            }
                            break;
                        }
                    }
                }
                else if (trimmedLine == "{{ menu }}")
                {
                    for (int j = 0; j < numSections; j++)
                    {
                        writeFile.WriteLine("<tr class=\"menu\">");
                        writeFile.WriteLine("<td class=\"menu\">");
                        writeFile.WriteLine("<a class=\"button\" href=\"/" + sectionTitle[j] + "/\">");
                        writeFile.WriteLine("<span style=\"width: 128px; display:inline-block;\">" + sectionTitle[j] + "</span>");
                        writeFile.WriteLine(@"</a>");
                        writeFile.WriteLine(@"</td>");
                        writeFile.WriteLine(@"</tr>");
                    }
                }
                else
                {
                    writeFile.WriteLine(line);
                }
            }

            writeFile.Close();
        }
}