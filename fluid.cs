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

    //makes the first letter uppercase
    static string upperCaseFirst(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        char[] a = s.ToCharArray();
        a[0] = char.ToUpper(a[0]);
        return new string(a);
    }

    static void Main()
    {
        string bf = @"C:\Users\Alex Wen\Desktop\nginx-1.7.6\html\body.html";
        StreamReader bodyFile = new StreamReader(bf);

        //this finds how many sections there are
        int numSections = countSec(bf); //this works

        //this finds the names of all the titles
        string[] sectionTitle = findTitle(bf, numSections); //this works
        foreach (string word in sectionTitle)
            Console.WriteLine(word);

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
                        if (trimmedLine == ("{{ " + sectionTitle[i] + " }}"))
                        {
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
                        writeFile.Write("<tr class=\"menu\"");
                        if (i == j)
                            writeFile.Write("style=\"background-color: #444;\"");
                        writeFile.WriteLine(">");
                        writeFile.WriteLine("   <td class=\"menu\">");
                        if (sectionTitle[j] != "resume")
                            writeFile.WriteLine("       <a class=\"button\" href=\"/" + sectionTitle[j] + "/\">");
                        else
                            writeFile.WriteLine("       <a class=\"button\" href=\"../" + sectionTitle[j] + ".pdf\">");
                        writeFile.WriteLine("           <span style=\"width: 128px; display:inline-block;\">" + upperCaseFirst(sectionTitle[j]) + "</span>");
                        writeFile.WriteLine(@"      </a>");
                        writeFile.WriteLine(@"  </td>");
                        writeFile.WriteLine(@"</tr>");
                    }

                }
                else
                {
                    writeFile.WriteLine(line);
                }
            }

            writeFile.Close();
            if (i == 0)
            {
                File.Copy(outFile,@"C:\Users\Alex Wen\Desktop\nginx-1.7.6\html\index.html",true);
            }
        }
    }
}