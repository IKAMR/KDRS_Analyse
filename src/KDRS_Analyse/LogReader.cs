﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KDRS_Analyse
{
    class LogReader
    {
        public delegate void ProgressUpdate(int count);
        public event ProgressUpdate OnProgressUpdate;

        public Dictionary<string, int> seqCount = new Dictionary<string, int>();

        public bool newFile = false;
        //------------------------------------------------------------------------------------
        // Reading Decom Blobs report and assigning values to objects
        public void ReadDcmBlbRpt(string fileName, string inRootFolder, string outRootFolder)
        {

            Globals.toolCounter++;
            Console.WriteLine(Globals.toolCounter);
            AnalyseTool dcmTool = new AnalyseTool
            {
                dcmTool = new AnalyseTool.DcmTool(),
                files = new AnalyseTool.DcmFiles(),
                toolNo = Globals.toolCounter.ToString(),
                name = "Decom",
                version = "1.3.0",
                toolId = "101",
                role = "transform",
                subrole = "project"
            };

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
                        string[] inputSplit = { "input file:", "input file content type:", "conversion output file:", "conversion status:", "conversion end date:" };

                        string inputPath = firstSplit[1].Split(inputSplit, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                        Console.WriteLine("input path: " + inputPath);
                        Console.WriteLine("In root folder: " + inRootFolder);

                        // Get file id based on file path and input root folder.
                        string fileId = GetFileId(inputPath, inRootFolder);

                        // Get file element based on fileId. Create new if none exists.
                        AnalyseFile file = GetFile(fileId);
                        fileCount++;

                        string[] timeSplit = { "time:", "date:" };
                        string startTime = firstSplit[0].Split(timeSplit, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                        file.result.start = TimeConv(startTime);

                        file.inFile.path = GetFileId(inputPath, inRootFolder);

                        if (String.IsNullOrEmpty(file.id))
                        {
                            newFile = true;
                            file.id = fileId;
                        }
                        string readMime = firstSplit[2].Split(inputSplit, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                        string fileMime = file.inFile.mime;

                        // Check if file mime match mime found by other tools.
                        if (String.IsNullOrEmpty(fileMime))
                            file.inFile.mime = readMime;
                        else if (fileMime != readMime)
                            MimeWarning(file, true, readMime, dcmTool.toolNo);

                        string[] outSplit = { "file:" };
                        if (String.IsNullOrEmpty(file.outFile.path))
                        {
                            string outFilePath = firstSplit[3].Split(inputSplit, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                            file.outFile.path = GetFileId(outFilePath, outRootFolder);
                        }

                        if (String.IsNullOrEmpty(file.result.result))
                            file.result.result = firstSplit[4].Split(':')[1].Trim();

                        if (String.IsNullOrEmpty(file.result.toolId))
                            file.result.toolId = dcmTool.toolId;

                        string endDate = firstSplit[5].Split(timeSplit, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                        Console.WriteLine("End time: " + endDate);

                        if (String.IsNullOrEmpty(file.result.end))
                            file.result.end = TimeConv(endDate);

                        // If new file created add this to file list.
                        if (newFile)
                        {
                            Globals.extractionAnalyse.files.files.Add(file);
                            newFile = false;
                            //Console.WriteLine("File added");
                        }

                        lineCount++;
                        OnProgressUpdate?.Invoke(fileCount);
                    }
                    lineCount++;
                    OnProgressUpdate?.Invoke(fileCount);
                }
            }

            dcmTool.files.files = Int32.Parse(dcmTool.files.inProgress) + Int32.Parse(dcmTool.files.failed) + Int32.Parse(dcmTool.files.success) +
                Int32.Parse(dcmTool.files.idle) + Int32.Parse(dcmTool.files.notConverted);
            Console.WriteLine("dcmFiles: " + dcmTool.files.files);

            dcmTool.inputPath.Add(inRootFolder);
            dcmTool.outputPath = outRootFolder;

            // Add tool to tool list.
            Globals.extractionAnalyse.tools.tools.Add(dcmTool);
            Console.WriteLine("Tool added");
        }
        //------------------------------------------------------------------------------------
        // Read decom desktop.log file. If multiple files this wil find correct file to start with and reads all in correct order.
        public void ReadDcmLog(string fileName, string inRootFolder, string outRootFolder)
        {
            Globals.extractionAnalyse.sequences = new SequenceWrapper();

            int uniqeKey = 1;

            string[] fileList;

            // Get all files if more than one.
            fileList = Directory.GetFiles(Path.GetDirectoryName(fileName));
            bool usingTempFile = false;
            string tempFile = String.Empty;
            foreach (string f in fileList)
                if (f.Contains("decom_desktop.1.log"))
                {
                    tempFile = MergeDcmLogFiles(fileList, fileName);
                    fileName = tempFile;
                    usingTempFile = true;
                    Console.WriteLine("Using tempfile");
                }
            Console.WriteLine("Reading decom log");

            Globals.toolCounter++;
            Console.WriteLine(Globals.toolCounter);

            AnalyseTool dcmTool = new AnalyseTool
            {
                database = new AnalyseTool.DcmDB(),
                dcmTool = new AnalyseTool.DcmTool(),
                toolNo = Globals.toolCounter.ToString(),
                name = "Decom",
                version = "1.3.0",
                toolId = "101",
                role = "transform",
                subrole = "log"
            };

            string timeStamp = String.Empty;
            int lineCounter = 0;
            int startLine = 0;

            string dbName = String.Empty;
            string intBlob = String.Empty;
            string extBlob = String.Empty;
            string refXml = String.Empty;
            string extBlobMiss = String.Empty;
            string readProject = String.Empty;

            // Reading decom log file
            AnalyseFile file = null;
            using (StreamReader reader = new StreamReader(fileName))
            {
                string line;
                string sequence = String.Empty;

                while ((line = reader.ReadLine()) != null)
                {
                    lineCounter++;

                    // Extract project relevant information
                    if (line.Contains("Working with:"))
                    {
                        startLine = lineCounter;
                        timeStamp = line.Substring(0, 19);
                        Console.WriteLine("Startline: " + startLine);

                        string[] splitAt = { "Working with:", "Total internal blobs found:", "Total external blobs found:",
                        "Total referenced XML files found:", "Are external blobs missing:"};
                        dbName = line.Split(splitAt, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        line = reader.ReadLine();
                        lineCounter++;
                        intBlob = line.Split(splitAt, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        line = reader.ReadLine();
                        lineCounter++;

                        extBlob = line.Split(splitAt, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        line = reader.ReadLine();
                        lineCounter++;

                        refXml = line.Split(splitAt, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        line = reader.ReadLine();
                        lineCounter++;

                        extBlobMiss = line.Split(splitAt, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                    }

                    // Find confirmation of correct folder
                    if (line.Contains(outRootFolder) && !String.IsNullOrEmpty(dbName))
                    {
                        Console.WriteLine("Detected");

                        dcmTool.database.db = dbName;
                        dcmTool.database.intBlob = intBlob;
                        dcmTool.database.extBlob = extBlob;
                        dcmTool.database.refXml = refXml;
                        dcmTool.database.extBlobMiss = extBlobMiss;

                        int restartLine = (startLine + 5);
                        Console.WriteLine("Going back to line : " + restartLine);
                        // Start from beginning and skip to position of lineCounter
                        reader.DiscardBufferedData();
                        reader.BaseStream.Seek(0, SeekOrigin.Begin);
                        lineCounter = 0;
                        for (int i = 1; i < restartLine; i++)
                        {
                            reader.ReadLine();
                            lineCounter++;
                            if (reader.EndOfStream)
                            {
                                Console.WriteLine($"End of file.  The file only contains {i} lines.");
                                break;
                            }
                        }

                        //File region
                        #region

                        // Extract file relevant information
                        while ((line = reader.ReadLine()) != null && !line.Contains("Working with:"))
                        {
                            lineCounter++;
                            sequence = AddSequence(sequence, line);
                            //Console.WriteLine("Sequence: " + sequence);

                            string[] projSplit = { "User has started the conversion process for project with name" };
                            if (line.Contains(projSplit[0]))
                                readProject = line.Split(projSplit, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                            Console.WriteLine("Reading over");

                            // Start of blob conversion
                            if (line.Contains("Starting conversion of blob"))
                            {
                                Console.WriteLine("Sequence: " + sequence);

                                if (String.IsNullOrEmpty(sequence))
                                    sequence = "126";
                                else
                                    UpdateSeqCount(sequence);

                                // Add sequense ID to file
                                if (file != null)
                                {
                                    if (Globals.seqDict.ContainsKey(sequence))
                                        file.result.seqId = Globals.seqDict[sequence].Split(';')[0];
                                    else
                                    {
                                        string uniqueSeqID = "u" + uniqeKey;
                                        file.result.seqId = uniqueSeqID;
                                        Globals.seqDict.Add(sequence, uniqueSeqID);
                                        uniqeKey++;
                                    }
                                }

                                // Resetting sequence.
                                sequence = "126";

                                Console.WriteLine("Found 'Starting conversion of blob' at line: " + lineCounter);
                                string[] split = { "Starting conversion of blob:", "and current conversion status is:" };
                                string filePath = line.Split(split, 3, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                                Console.WriteLine("input path: " + filePath);

                                // Get file id based on file path and input root folder.
                                string fileId = GetFileId(filePath, inRootFolder);
                                Console.WriteLine("File id: " + fileId);

                                // Get file element based on fileId. Create new if non exists.
                                file = GetFile(fileId);

                                if (String.IsNullOrEmpty(file.id))
                                {
                                    Console.WriteLine("New file");
                                    newFile = true;
                                    file.id = fileId;
                                }

                                line = reader.ReadLine();
                                sequence = AddSequence(sequence, line);
                                lineCounter++;

                                line = reader.ReadLine();
                                sequence = AddSequence(sequence, line);
                                lineCounter++;

                                string[] splitMime = { "The detected MIME type of blob:", "is" };

                                string readMimeLine = reader.ReadLine();
                                sequence = AddSequence(sequence, readMimeLine);

                                while (!readMimeLine.Contains(splitMime[0]) && readMimeLine != null)
                                {
                                    readMimeLine = reader.ReadLine();
                                    sequence = AddSequence(sequence, readMimeLine);

                                    lineCounter++;
                                }

                                string[] errorSplit = { "BLOBConversionProcess:151 -", " - " };
                                string readMime = "";
                                if (CheckLine(readMimeLine))
                                    readMime = readMimeLine.Split(splitMime, 3, StringSplitOptions.RemoveEmptyEntries)[2].Trim();
                                else
                                {
                                    AnalyseFile.FileError error = new AnalyseFile.FileError();
                                    error.text = readMimeLine.Split(errorSplit, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                                    file.errors.Add(error);
                                }
                                string fileMime = file.inFile.mime;

                                Console.WriteLine("Checking mime");
                                if (String.IsNullOrEmpty(fileMime))
                                    file.inFile.mime = readMime;
                                else if (fileMime != readMime)
                                    MimeWarning(file, true, readMime, dcmTool.toolId);

                                Console.WriteLine("Checking project");
                                if (String.IsNullOrEmpty(dcmTool.project))
                                {
                                    Console.WriteLine("New project");
                                    dcmTool.project = readProject;
                                }
                                else if (dcmTool.project != readProject)
                                {
                                    Console.WriteLine("Project warning");
                                    ProjectWarning(file, dcmTool.project, readProject, dcmTool.toolId);
                                }

                                if (String.IsNullOrEmpty(file.result.toolId))
                                    file.result.toolId = dcmTool.toolId;
                                                               
                                if (newFile)
                                {
                                    Globals.extractionAnalyse.files.files.Add(file);
                                    newFile = false;
                                    Console.WriteLine("File added");
                                }
                            }
                            Console.WriteLine("Line count: " + lineCounter);
                            OnProgressUpdate?.Invoke(lineCounter);
                        }
                        #endregion

                        // Update sequence count for the last file.
                        UpdateSeqCount(sequence);

                        // Add sequense ID to the last file
                        if (file != null)
                        {
                            if (Globals.seqDict.ContainsKey(sequence))
                                file.result.seqId = Globals.seqDict[sequence].Split(';')[0];
                            else
                            {
                                string uniqueSeqID = "u" + uniqeKey;
                                file.result.seqId = uniqueSeqID;
                            }
                        }
                        break;
                    }
                    Console.WriteLine("Line count: " + lineCounter);
                    OnProgressUpdate?.Invoke(lineCounter);
                }
            }
            Console.WriteLine("Line count: " + lineCounter);

            dcmTool.inputPath.Add(inRootFolder);
            dcmTool.outputPath = outRootFolder;

            Globals.extractionAnalyse.tools.tools.Add(dcmTool);
            Console.WriteLine("Tool added");

            if (usingTempFile && File.Exists(tempFile))
                File.Delete(tempFile);

            Console.WriteLine("Read sequence dictionary");
            ReadSeqDict();
            Console.WriteLine("Read sequence task dictionary");

            ReadSeqTaskDict();

            Console.WriteLine("Adding seq count");
            foreach(var entry in seqCount)
            {
               foreach (SequenceWrapper.SequencesWrapper.Sequence seq in Globals.extractionAnalyse.sequences.sequences.sequenceList)
                {
                    if (seq.sequence.Equals(entry.Key))
                        seq.count = entry.Value;
                }
            }


        }
        //------------------------------------------------------------------------------------
        public string MergeDcmLogFiles(string[] fileList, string fileName)
        {
            //string[] fileList;
            string folder = Path.GetDirectoryName(fileName);

            List<string> logList = new List<string>();

            int fileNumber = 1;

            foreach (string f in fileList)
            {
                if (f.Contains("decom_desktop." + fileNumber + ".log"))
                {
                    logList.Add(f);
                }
                else if (f.Contains("decom_desktop.log"))
                {
                    logList.Add(f);
                }
            }
            string tempFile = Path.Combine(folder, "tempLogFile.log");
            using (var outputStream = File.Create(tempFile))
            {
                foreach (var file in logList)
                {
                    using (var inputStream = File.OpenRead(file))
                        inputStream.CopyTo(outputStream);
                }
            }
            return tempFile;
        }
        //------------------------------------------------------------------------------------
        // Read droid.csv file. Choose between droid scan made before or after conversion of files. Choose if siard format table.xml and table.xsd should be included.
        public void ReadDroidFiles(string fileName, bool inFiles, string inRootFolder, string outRootFolder, bool incTableXml)
        {
            // In or out files,
            string isIn = "in";
            if (!inFiles)
                isIn = "out";

            Globals.toolCounter++;
            Console.WriteLine(Globals.toolCounter);

            // Create new tool element.
            AnalyseTool droidTool = new AnalyseTool
            {
                toolNo = Globals.toolCounter.ToString(),
                toolId = "102",
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
                    // Split line on ",", to get correct information.
                    string[] split = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

                    if (!split[8].Contains("File"))
                    {
                        Console.WriteLine(split[8].ToString());
                        continue;
                    }

                    string filePath = split[3].Trim('"');

                    if (String.IsNullOrEmpty(filePath))
                        continue;

                    // Find fileid from file path and either in- or out-root folder
                    string fileId;
                    if (inFiles)
                        fileId = GetFileId(filePath, inRootFolder);
                    else
                        fileId = GetFileId(filePath, outRootFolder);

                    Console.WriteLine("FileID: " + fileId);

                    // Skip table .xml and .xsd if choosen
                    if (!incTableXml && filePath.Contains("table") && (filePath.Contains(".xml") || filePath.Contains(".xsd")))
                        continue;

                    string readFileName = split[16].Trim('"');

                    // Get file element based on fileId. Create new if non exists.
                    AnalyseFile droidFile = GetFile(fileId);
                    fileCount++;

                    if (String.IsNullOrEmpty(droidFile.id))
                    {
                        newFile = true;
                        droidFile.id = fileId;
                    }

                    if (inFiles)
                    {
                        droidFile.inFile.ext = split[9].Trim('"').Trim();
                        droidFile.inFile.puid = split[14].Trim('"').Trim();

                        string readMime = split[15].Trim('"').Trim();
                        string fileMime = droidFile.inFile.mime;

                        // Validate that file Mime is same as found by other tools.
                        if (String.IsNullOrEmpty(fileMime))
                            droidFile.inFile.mime = readMime;
                        else if (fileMime != readMime)
                        {
                            foreach (var key in Globals.mimeDict.Keys)
                            {
                                if (!Globals.mimeDict[key].Contains(readMime) && !Globals.mimeDict[key].Contains(fileMime))
                                    MimeWarning(droidFile, true, readMime, droidTool.toolId);
                                else if (Globals.mimeDict[key].Contains(readMime) && !Globals.mimeDict[key].Contains(fileMime))
                                    MimeWarning(droidFile, true, readMime, droidTool.toolId);
                                else if (!Globals.mimeDict[key].Contains(readMime) && Globals.mimeDict[key].Contains(fileMime))
                                    MimeWarning(droidFile, true, readMime, droidTool.toolId);
                            }
                        }

                        // Validate if file puId is same as found by other tools.
                        foreach (AnalyseFile.Valid valid in droidFile.valid)
                        {
                            string readPuId = valid.puid;
                            if (!String.IsNullOrEmpty(readPuId) && !String.IsNullOrEmpty(droidFile.inFile.puid))
                            {
                                if (!readPuId.Equals(droidFile.inFile.puid))
                                {
                                    XMLReader.PuIdWarning(droidFile, readPuId, droidFile.inFile.puid, droidTool.toolId);
                                }
                            }
                        }

                        droidFile.inFile.name = readFileName;

                        string fileVersion = split[17].Trim('"');
                        if (!String.IsNullOrEmpty(fileVersion))
                            droidFile.inFile.version = fileVersion;
                    }
                    else
                    {
                        droidFile.outFile.ext = split[9].Trim('"');
                        droidFile.outFile.puid = split[14].Trim('"');

                        string readMime = split[15].Trim('"');
                        string fileMime = droidFile.outFile.mime;

                        // Validate that file Mime is same as found by other tools.
                        if (String.IsNullOrEmpty(fileMime))
                            droidFile.outFile.mime = readMime;
                        else if (fileMime != readMime) {
                            foreach (var key in Globals.mimeDict.Keys)
                            {
                                if (!Globals.mimeDict[key].Contains(readMime) && !Globals.mimeDict[key].Contains(fileMime))
                                    MimeWarning(droidFile, true, readMime, droidTool.toolId);
                                else if (Globals.mimeDict[key].Contains(readMime) && !Globals.mimeDict[key].Contains(fileMime))
                                    MimeWarning(droidFile, true, readMime, droidTool.toolId);
                                else if (!Globals.mimeDict[key].Contains(readMime) && Globals.mimeDict[key].Contains(fileMime))
                                    MimeWarning(droidFile, true, readMime, droidTool.toolId);
                            }
                        }

                        // Validate if file puId is same as found by other tools.
                        foreach (AnalyseFile.Valid valid in droidFile.valid)
                        {
                            string readPuId = valid.puid;
                            if (!String.IsNullOrEmpty(readPuId) && !String.IsNullOrEmpty(droidFile.outFile.puid))
                            {
                                if (!readPuId.Equals(droidFile.outFile.puid))
                                {
                                    XMLReader.PuIdWarning(droidFile, readPuId, droidFile.outFile.puid, droidTool.toolId);
                                }
                            }
                        }

                        droidFile.outFile.name = readFileName;

                        string fileVersion = split[17].Trim('"');
                        if (!String.IsNullOrEmpty(fileVersion))
                            droidFile.outFile.version = fileVersion;
                    }
                    OnProgressUpdate?.Invoke(fileCount);
                    if (newFile)
                    {
                        Globals.extractionAnalyse.files.files.Add(droidFile);
                        newFile = false;
                    }
                }
            }
            Globals.extractionAnalyse.tools.tools.Add(droidTool);
            Console.WriteLine("Tool added");
        }
        //------------------------------------------------------------------------------------
        // Convert time format found in Dcm blobs report to desired format.
        public string TimeConv(string timeString)
        {
            DateTime parseDate = new DateTime();

            if (timeString.Contains("CET"))
                parseDate = DateTime.ParseExact(timeString, "ddd MMM dd HH:mm:ss 'CET' yyyy", CultureInfo.InvariantCulture);
            else if (timeString.Contains("CEST"))
                parseDate = DateTime.ParseExact(timeString, "ddd MMM dd HH:mm:ss 'CEST' yyyy", CultureInfo.InvariantCulture);

            return parseDate.ToString("yyyy-MM-dd HH:mm:ss");
        }
        //------------------------------------------------------------------------------------
        // Searches list of file objects and returns object with same fileId as input. If no file exist with fileId -> creates new file object.
        public static AnalyseFile GetFile(string fileId)
        {
            Console.WriteLine("Get file");
            try
            {
                Console.WriteLine("Files: " + Globals.extractionAnalyse.files.files.Count);
                //newFile = false;
                foreach (AnalyseFile file in Globals.extractionAnalyse.files.files)
                {
                    if (file.id.Contains(fileId) || fileId.Contains(file.id))
                    {
                        if (file.id.Equals(fileId))
                            return file;
                        else if (file.id.Equals(fileId.Remove(fileId.Length - 4)))
                            return file;
                        else if (file.id.Equals(fileId.Remove(fileId.Length - 5)))
                            return file;
                        else if (file.id.Equals(fileId.Remove(fileId.Length - 8)))
                            return file;
                        else if (file.id.Equals(fileId.Remove(fileId.Length - 9)))
                            return file;
                        else if (fileId.Equals(file.id.Remove(file.id.Length - 4)))
                            return file;
                        else if (fileId.Equals(file.id.Remove(file.id.Length - 8)))
                            return file;
                        else if (fileId.Equals(file.id.Remove(file.id.Length - 5)))
                            return file;
                        else if (fileId.Equals(file.id.Remove(file.id.Length - 9)))
                            return file;
                    }
                }
                return new AnalyseFile();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //------------------------------------------------------------------------------------
        // Returns tool based on toolId. (Not in use.)
        public AnalyseTool GetTool(string toolId)
        {
            foreach (AnalyseTool tool in Globals.extractionAnalyse.tools.tools)
            {
                if (tool.toolId.Equals(toolId))
                    return tool;
            }
            return new AnalyseTool
            {
                toolId = toolId
            };
        }
        //------------------------------------------------------------------------------------
        // Finds common part of path between filePath and input root path. Returns unique file path.
        public static string GetFileId(string filePath, string rootFolder)
        {
            Console.WriteLine("Get file ID");

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

            string[] splitWord = { (sequence + @"\") };

            Console.WriteLine("Common path: " + sequence);

            string[] filePathSplit = filePath.Split(splitWord, 2, StringSplitOptions.RemoveEmptyEntries);

            if (filePathSplit.Length > 1)
                fileId = filePathSplit[1];
            else
                fileId = filePathSplit[0];
            //fileId = filePath.Substring(sequence.Length + 1);

            return fileId;
        }
        //------------------------------------------------------------------------------------
        // Creates a warning if file mime doesn't match mime found by other tools.
        // file with the mismatch, if the file is in or out, new mime found by new tool, toolId of tool identifying new mime.
        public void MimeWarning(AnalyseFile file, bool inFile, string mime, string toolID)
        {
            AnalyseFile.AnalyseWarning warning = new AnalyseFile.AnalyseWarning();
            warning.toolId = toolID;

            if (inFile)
            {
                warning.element = "inFile";
                warning.value1 = file.inFile.mime;
            }
            warning.value2 = mime;
            warning.text = "MIME mismatch";

            file.warning.Add(warning);
        }
        //------------------------------------------------------------------------------------
        // Warning created if project found by new tool dosen't match project found by other tools.
        public void ProjectWarning(AnalyseFile file, string refProject, string project, string toolID)
        {
            AnalyseFile.AnalyseWarning warning = new AnalyseFile.AnalyseWarning();

            warning.toolId = toolID;
            warning.element = "project";
            warning.value1 = refProject;

            warning.value2 = project;
            warning.text = "PROJECT mismatch";

            file.warning.Add(warning);
        }
        //------------------------------------------------------------------------------------
        // Check if line contains spesific string. Return false if not.
        public bool CheckLine(string line)
        {
            if (line.Contains("INFO  com.documaster.decom.views.blobconversion.BLOBConversionProcess"))
                return true;
            Console.WriteLine("Error found");
            return false;
        }
        //------------------------------------------------------------------------------------
        // Read line on spesific position in file. (Not in use).
        static string ReadSpecificLine(string filePath, int lineNumber)
        {
            string content = String.Empty;
            try
            {
                using (StreamReader file = new StreamReader(filePath))
                {
                    for (int i = 1; i < lineNumber; i++)
                    {
                        file.ReadLine();

                        if (file.EndOfStream)
                        {
                            Console.WriteLine($"End of file.  The file only contains {i} lines.");
                            break;
                        }
                    }
                    content = file.ReadLine();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("There was an error reading the file: ");
                Console.WriteLine(e.Message);
            }
            return content;
        }
        //------------------------------------------------------------------------------------
        // Add new number to decom log file sequence
        private string AddSequence(string sequence, string line)
        {
            string[] splitArray = { "BLOBConversionProcess:", " - " };
            if (!line.Contains("BLOBConversionProcess:126"))
            {
                if (!line.Contains("BLOBConversionProcess") || (!line.Contains("INFO") && !line.Contains("ERROR") && !line.Contains("WARN")) || String.IsNullOrEmpty(sequence))
                    return sequence;
                else
                    return sequence = sequence + "-" + line.Split(splitArray, 3, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
            }
            return sequence;
        }
        //------------------------------------------------------------------------------------
        // Update decom log sequence count
        private void UpdateSeqCount(string sequence)
        {
            if (seqCount.ContainsKey(sequence))
            {
                try
                {
                    seqCount[sequence]++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to add sequence value");
                    Console.WriteLine(sequence);
                }
            }
            else
            {
                try
                {
                    seqCount.Add(sequence, 1);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to add sequence");
                    Console.WriteLine(sequence);
                }
            }
        }
        //------------------------------------------------------------------------------------
        // Read sequence task dictionary to objects
        private void ReadSeqTaskDict()
        {
            Globals.extractionAnalyse.sequences.tasks = new SequenceWrapper.SequenceTaskWrapper();
            Globals.extractionAnalyse.sequences.tasks.taskList = new List<SequenceWrapper.SequenceTaskWrapper.SequenceTask>();
            string[] values = null;

            foreach (KeyValuePair<string,string> pair in Globals.taskDict)
            {
                SequenceWrapper.SequenceTaskWrapper.SequenceTask task = new SequenceWrapper.SequenceTaskWrapper.SequenceTask();

                task.id = pair.Key;

                values = pair.Value.Split(';');

                Console.WriteLine("Values length: " + values.Length);

                task.type = values[0];
                if (values.Length > 1)
                {
                    if (!String.IsNullOrEmpty(values[1]))
                        task.name = values[1];
                    if (!String.IsNullOrEmpty(values[2]))
                        task.text1 = values[2].Trim();
                    if (!String.IsNullOrEmpty(values[3]))
                        task.element1 = values[3];
                    if (!String.IsNullOrEmpty(values[4]))
                        task.text2 = values[4].Trim();
                    if (!String.IsNullOrEmpty(values[5]))
                        task.element2 = values[5];
                    if (!String.IsNullOrEmpty(values[6]))
                        task.line2 = values[6];
                }
                Globals.extractionAnalyse.sequences.tasks.taskList.Add(task);
                Console.WriteLine("Task added");
            }
        }
        //------------------------------------------------------------------------------------
        // Read sequence dictionary to objects.
        private void ReadSeqDict()
        {
            Globals.extractionAnalyse.sequences.sequences = new SequenceWrapper.SequencesWrapper
            {
                sequenceList = new List<SequenceWrapper.SequencesWrapper.Sequence>()
            };
            string[] values = null;

            foreach (KeyValuePair<string, string> pair in Globals.seqDict)
            {
                SequenceWrapper.SequencesWrapper.Sequence sequence = new SequenceWrapper.SequencesWrapper.Sequence
                {
                    sequence = pair.Key
                };

                values = pair.Value.Split(';');

                Console.WriteLine("Values length: " + values.Length);

                sequence.id = values[0];
                if (values.Length > 1)
                {
                    if (!String.IsNullOrEmpty(values[1]))
                        sequence.result = values[1].Trim();
                    if (!String.IsNullOrEmpty(values[2]))
                        sequence.name = values[2];
                    if (!String.IsNullOrEmpty(values[3]))
                        sequence.description = values[3].Trim();
                }
                Globals.extractionAnalyse.sequences.sequences.sequenceList.Add(sequence);
            }
        }
    }
}
