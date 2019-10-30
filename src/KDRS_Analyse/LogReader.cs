using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDRS_Analyse
{
    class LogReader
    {
        // Reading Decom Blob report and assigning values to objects
        public void ReadDcmBlbRpt(string fileName, string inRootFolder, string outRootFolder)
        {

            Globals.toolCounter++;
            Console.WriteLine(Globals.toolCounter);
            Tool dcmTool = new Tool
            {
                no = Globals.toolCounter.ToString(),
                id = "101",
                name = "Decom",
                version = "1.3.0"
            };

            Console.WriteLine("Tool created");

            string line;
            using (StreamReader reader = new StreamReader(fileName))
            {
                Console.WriteLine("Start read");
                int lineCount = 0;

                // Read all lines in file one at the time
                while ((line = reader.ReadLine()) != null)
                {
                    // Skip empty lines
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (lineCount == 0)
                        {
                            string[] splitter = { "was" };
                            dcmTool.dcmTool.name = line.Split(splitter, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim().TrimEnd('.');
                            lineCount++;
                            continue;
                        }

                        if (lineCount == 1)
                        {
                            string[] splitter = { "name" };
                            dcmTool.project = line.Split(splitter, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim().TrimEnd('.');
                            lineCount++;
                            continue;
                        }
                        
                        if (line.Contains("IN_PROGRESS:"))
                        {
                            dcmTool.files.inProgress = line.Split(':')[1].Trim();
                            continue;
                        }
                        else if (line.Contains("FAILED:"))
                        {
                            dcmTool.files.failed = line.Split(':')[1].Trim();
                            continue;
                        }
                        else if (line.Contains("SUCCESSFUL:"))
                        {
                            dcmTool.files.success = line.Split(':')[1].Trim();
                            continue;
                        }
                        else if (line.Contains("IDLE:"))
                        {
                            dcmTool.files.idle = line.Split(':')[1].Trim();
                            continue;
                        }
                        else if (line.Contains("NOT_CONVERTED:"))
                        {
                            dcmTool.files.notConverted = line.Split(':')[1].Trim();
                            continue;
                        }
                        
                        string[] firstSplit = line.Split(',');
                        File file = new File();
                        Console.WriteLine("File created");

                        string[] timeSplit = { "time:", "date:" };
                        file.start = TimeConv(firstSplit[0].Split(timeSplit, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim());

                        string inputPath = firstSplit[1].Split(':')[1].Trim();
                        file.input = inputPath;
                        file.id = inputPath;

                        file.mime = firstSplit[2].Split(':')[1].Trim();

                        string[] outSplit = { "file:" };
                        file.output = firstSplit[3].Split(outSplit, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        file.result.result = firstSplit[4].Split(':')[1].Trim();

                        file.result.tool = Globals.toolCounter;

                        file.end = TimeConv(firstSplit[5].Split(timeSplit, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim());

                        Globals.extractionAnalyse.files.Add(file);
                        Console.WriteLine("File added");

                        lineCount++;

                    }
                    lineCount++;
                }
            }

            dcmTool.files.files = Int32.Parse(dcmTool.files.inProgress) + Int32.Parse(dcmTool.files.failed) + Int32.Parse(dcmTool.files.success) +
                Int32.Parse(dcmTool.files.idle) + Int32.Parse(dcmTool.files.notConverted);
            Console.WriteLine("dcmFiles: " + dcmTool.files.files);

            dcmTool.inputPath.Add(inRootFolder);
            dcmTool.outputPath = outRootFolder;

            Globals.extractionAnalyse.tools.Add(dcmTool);
            Console.WriteLine("Tool added");

        }

        public void ReadDcmLog(string fileName)
        {
        }

        public string TimeConv(string timeString)
        {
            DateTime parseDate = DateTime.ParseExact(timeString, "ddd MMM dd HH:mm:ss 'CET' yyyy", CultureInfo.InvariantCulture);
            return parseDate.ToString("yyyy-MM-dd HH:mm:mm");
        }
    }

}
