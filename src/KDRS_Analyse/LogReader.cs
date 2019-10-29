using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDRS_Analyse
{
    class LogReader
    {
        public void ReadDcmBlbRpt(string fileName)
        {
            Globals.toolCounter++;
            Tool dcmTool = new Tool(Globals.toolCounter.ToString(), "101", "Decom", "1.3.0");

            string line;
            using (StreamReader reader = new StreamReader(fileName))
            {
                int lineCount = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    if (lineCount == 0)
                    {
                        string[] splitter = { "was" };
                        dcmTool.dcmTool.name = line.Split(splitter, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                        lineCount++;
                        break;
                    }

                    if (lineCount == 1)
                    {
                        string[] splitter = { "name" };

                        dcmTool.project = line.Split(splitter, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                        break;
                    }

                    string[] firstSplit = line.Split(',');
                    File file = new File();
                    file.start = firstSplit[0].Split(':')[1].Trim();
                    file.input = firstSplit[1].Split(':')[1].Trim();
                    file.mime = firstSplit[2].Split(':')[1].Trim();
                    file.output = firstSplit[3].Split(':')[1].Trim();
                    file.result = firstSplit[4].Split(':')[1].Trim();
                    file.end = firstSplit[5].Split(':')[1].Trim();

                    Globals.extractionAnalyse.files.Add(file);

                    lineCount++;
                }
            }

            Globals.extractionAnalyse.tools.Add(dcmTool);
        }

        public void ReadDcmLog(string fileName)
        {
        }

        public string timeConv(string timeString)
        {
            DateTime parseDate = DateTime.Parse(timeString);

            return parseDate.ToString("yyyy-MM-dd HH:mm:mm");
        }
    }

}
