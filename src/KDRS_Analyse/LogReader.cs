using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace KDRS_Analyse
{
    class LogReader
    {
        public delegate void ProgressUpdate(int count);
        public event ProgressUpdate OnProgressUpdate;

        // Reading Decom Blob report and assigning values to objects
        public void ReadDcmBlbRpt(string fileName, string inRootFolder, string outRootFolder)
        {

            Globals.toolCounter++;
            Console.WriteLine(Globals.toolCounter);
            AnalyseTool dcmTool = GetTool("101");

            dcmTool.dcmTool = new AnalyseTool.DcmTool();
            dcmTool.files = new AnalyseTool.DcmFiles();
            dcmTool.toolNo = Globals.toolCounter.ToString();
            dcmTool.name = "Decom";
            dcmTool.version = "1.3.0";
            

            Console.WriteLine("Tool created");

            string line;
            using (StreamReader reader = new StreamReader(fileName))
            {
                Console.WriteLine("Start read");
                int lineCount = 0;
                int fileCount = 0;
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
                        //File file = new File();

                        string inputPath = firstSplit[1].Split(':')[1].Trim();

                        string fileId = GetFileId(inputPath, inRootFolder);
                        AnalyseFile file = GetFile(fileId);
                        Console.WriteLine("File created");
                        fileCount++;

                        string[] timeSplit = { "time:", "date:" };
                        file.start = TimeConv(firstSplit[0].Split(timeSplit, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim());

                        file.inFile.path = inputPath;
                        file.id = fileId;

                        string readMime = firstSplit[2].Split(':')[1].Trim();
                        string fileMime = file.inFile.mime;

                        if (String.IsNullOrEmpty(fileMime))
                            file.inFile.mime = readMime;
                        else if (fileMime != readMime)
                            MimeWarning(file, true, readMime, dcmTool.toolNo);

                        

                        string[] outSplit = { "file:" };
                        file.outFile.path = firstSplit[3].Split(outSplit, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        file.result.result = firstSplit[4].Split(':')[1].Trim();

                        file.result.toolNo = Globals.toolCounter;

                        file.end = TimeConv(firstSplit[5].Split(timeSplit, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim());

                        Globals.extractionAnalyse.files.Add(file);
                        Console.WriteLine("File added");

                        lineCount++;
                        OnProgressUpdate?.Invoke(fileCount);
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
            Console.WriteLine("Reading decom log");

            Globals.toolCounter++;
            Console.WriteLine(Globals.toolCounter);
            AnalyseTool dcmTool = GetTool("101");
            dcmTool.database = new AnalyseTool.DcmDB();
            dcmTool.dcmTool = new AnalyseTool.DcmTool();
            dcmTool.toolNo = Globals.toolCounter.ToString();
            dcmTool.name = "Decom";
            dcmTool.version = "1.3.0";

            Console.WriteLine("Reading file");

            int lineCounter = 0;
            foreach (string line in File.ReadLines(fileName))
            {
                if (!line.Contains("INFO") || !line.Contains("ERROR"))
                    continue;
                if (string.IsNullOrEmpty(line))
                    continue;

                string timeStamp = String.Empty;
                string dbName = String.Empty;

                if (line.Contains("Working with:")) {
                    timeStamp = line.Substring(0, 19);

                    string[] splitAt = { "Working with:" };
                    dbName = line.Split(splitAt, 2, StringSplitOptions.RemoveEmptyEntries)[1];
                }
                Console.WriteLine("Time: " + timeStamp);
                Console.WriteLine("DB name: " + dbName);
                lineCounter++;

                OnProgressUpdate?.Invoke(lineCounter);
            }
            Console.WriteLine("Line count: " + lineCounter);
        }

        public void ReadDroidFiles(string fileName, bool inFiles, string inRootFolder, string outRootFolder, bool incTableXml)
        {
            string isIn = "in";
            if (!inFiles)
                isIn = "out";

            Globals.toolCounter++;
            Console.WriteLine(Globals.toolCounter);
            AnalyseTool droidTool = new AnalyseTool
            {
                toolNo = Globals.toolCounter.ToString(),
                id = "102",
                name = "Droid",
                version = "6.4",
                role = "filetype",
                subrole = isIn
            };

            Console.WriteLine("Tool created");

            using (var reader = new StreamReader(fileName))
            {
                // Read all lines in file one at the time
                int fileCount = 0;

                string line = reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    string[] split = line.Split(',');

                    if (!split[8].Contains("File"))
                    {
                        Console.WriteLine(split[8].ToString());
                        continue;
                    }

                    //File droidFile = new File();
                    string filePath = split[3].Trim('"');
                    string fileId;
                    if (inFiles)
                        fileId = GetFileId(filePath, inRootFolder);
                    else 
                        fileId = GetFileId(filePath, outRootFolder);

                    string readFileName = split[16].Trim('"');
                    if (!incTableXml && readFileName.Contains("table") && (readFileName.Contains(".xml") || readFileName.Contains(".xsd")))
                        continue;

                    AnalyseFile droidFile = GetFile(fileId);
                    fileCount++;

                    droidFile.id = fileId;

                    if (inFiles)
                    {
                        droidFile.inFile.ext = split[9].Trim('"');
                        droidFile.inFile.puid = split[14].Trim('"');

                        string readMime = split[15].Trim('"');
                        string fileMime = droidFile.inFile.mime;

                        if (String.IsNullOrEmpty(fileMime))
                            droidFile.inFile.mime = readMime;
                        else if (fileMime != readMime)
                            MimeWarning(droidFile, true, readMime, droidTool.toolNo);

                        droidFile.inFile.name = readFileName;
                        droidFile.inFile.version = split[17].Trim('"');
                    }
                    else
                    {
                        droidFile.outFile.ext = split[9].Trim('"');
                        droidFile.outFile.puid = split[14].Trim('"');

                        string readMime = split[15].Trim('"');
                        string fileMime = droidFile.outFile.mime;

                        if (String.IsNullOrEmpty(fileMime))
                            droidFile.outFile.mime = readMime;
                        else if (fileMime != readMime)
                            MimeWarning(droidFile, false, readMime, droidTool.toolNo);

                        droidFile.outFile.name = readFileName;
                        droidFile.outFile.version = split[17].Trim('"');
                    }
                    OnProgressUpdate?.Invoke(fileCount);
                    Globals.extractionAnalyse.files.Add(droidFile);
                }
            }
            Globals.extractionAnalyse.tools.Add(droidTool);
            Console.WriteLine("Tool added");
        }

        public string TimeConv(string timeString)
        {
            DateTime parseDate = DateTime.ParseExact(timeString, "ddd MMM dd HH:mm:ss 'CET' yyyy", CultureInfo.InvariantCulture);
            return parseDate.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public AnalyseFile GetFile(string fileId)
        {
            foreach (AnalyseFile file in Globals.extractionAnalyse.files)
            {
                if (file.id.Equals(fileId))
                    return file;
                else if (file.id.Equals(fileId.Remove(fileId.Length - 4)))
                    return file;
                else if (file.id.Equals(fileId.Remove(fileId.Length - 8)))
                    return file;
                else if (fileId.Equals(file.id.Remove(file.id.Length - 4)))
                    return file;
                else if (fileId.Equals(file.id.Remove(file.id.Length - 8)))
                    return file;
            }
            return new AnalyseFile();
        }

        public AnalyseTool GetTool(string toolId)
        {
            foreach (AnalyseTool tool in Globals.extractionAnalyse.tools)
            {
                if (tool.id.Equals(toolId))
                    return tool;
            }
            return new AnalyseTool{
                id = toolId };
        }

        public string GetFileId(string filePath, string rootFolder)
        {
            string fileId = filePath;
            string newFilePath = filePath;

            // Find common part of rootFolder and filePath
            string sequence = string.Empty;
            int[,] num = new int[rootFolder.Length, filePath.Length];
            int maxlen = 0;
            int lastSubsBegin = 0;
            StringBuilder sequenceBuilder = new StringBuilder();

            for (int i = 0; i < rootFolder.Length; i++)
            {
                for (int j = 0; j < filePath.Length; j++)
                {
                    if (rootFolder[i] != filePath[j])
                        num[i, j] = 0;
                    else
                    {
                        if ((i == 0) || (j == 0))
                            num[i, j] = 1;
                        else
                            num[i, j] = 1 + num[i - 1, j - 1];

                        if (num[i, j] > maxlen)
                        {
                            maxlen = num[i, j];
                            int thisSubsBegin = i - num[i, j] + 1;
                            if (lastSubsBegin == thisSubsBegin)
                            {//if the current LCS is the same as the last time this block ran
                                sequenceBuilder.Append(rootFolder[i]);
                            }
                            else //this block resets the string builder if a different LCS is found
                            {
                                lastSubsBegin = thisSubsBegin;
                                sequenceBuilder.Length = 0; //clear it
                                sequenceBuilder.Append(rootFolder.Substring(lastSubsBegin, (i + 1) - lastSubsBegin));
                            }
                        }
                    }
                }
            }
            sequence = sequenceBuilder.ToString();

            string[] splitWord = { sequence };

            Console.WriteLine("Common path: " + sequence);

            fileId = filePath.Substring(sequence.Length + 1);

            return fileId;
        }

        public void MimeWarning(AnalyseFile file, bool inFile, string mime, string toolNo)
        {
            file.warning.toolNo = toolNo;

            if (inFile)
            {
                file.warning.element = "inFile";
                file.warning.value1 = file.inFile.mime;
            }
            file.warning.value2 = mime;
            file.warning.text = "MIME mismatch";
        }
    }
}
