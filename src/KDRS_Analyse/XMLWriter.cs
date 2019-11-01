using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace KDRS_Analyse
{
    class XMLWriter
    {
        public void WriteXml(string fileName)
        {
            Console.WriteLine("writing to: " + fileName);
            XmlSerializer ser = new XmlSerializer(typeof(ExtractionAnalyse));

            using (TextWriter writer = new StreamWriter(fileName))
            {
                Console.WriteLine("serializing");
                ser.Serialize(writer, Globals.extractionAnalyse);
            }
        }
    }

    public class ExtractionAnalyse
    {
        public ExtractionAnalyse()
        {
            files = new List<File>();
            tools = new List<Tool>();
        }

        public List<Tool> tools { get; set; }
        public Info info { get; set; }
        public Agents agents { get; set; }
        public SystemInfo system { get; set; }
        public List<File> files { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property,
    Inherited = false,
    AllowMultiple = false)]
    internal sealed class OptionalAttribute : Attribute
    {
    }

    public class Tool
    {
        public Tool()
        {
            inputPath = new List<string>();
        }

        [XmlAttribute]
        public string toolNo { get; set; }

        [XmlAttribute]
        public string id { get; set; }

        [XmlAttribute]
        public string name { get; set; }

        [XmlAttribute]
        public string version { get; set; }

        [XmlAttribute]
        public string role { get; set; }

        [XmlAttribute]
        public string subrole { get; set; }

        public string project { get; set; }

        [Optional]
        public DcmTool dcmTool { get; set; }
        [Optional]
        public DcmFiles files { get; set; }

        public List<string> inputPath { get; set; }
        public string outputPath { get; set; }

        public class DcmFiles
        {
            [XmlAttribute]
            public string success { get; set; }

            [XmlAttribute]
            public string failed { get; set; }

            [XmlAttribute]
            public string notConverted { get; set; }

            [XmlAttribute]
            public string idle { get; set; }

            [XmlAttribute]
            public string inProgress { get; set; }

            [XmlText]
            public int files { get; set; }
        }

        public class DcmTool
        {
            [XmlAttribute]
            public string version { get; set; }

            public string name { get; set; }
        }
    }

    public class Info
    {
        public string deliveryspecification { get; set;}
        public string submissionagreement { get; set; }
        public string startdate { get; set; }
        public string enddate { get; set; }
        public string ipType { get; set; }
        public string ipUuid { get; set; }
        public string extractionDateTime { get; set; }
        public string sipDateTime { get; set; }
    }

    public class Agents
    {
        public string archivist { get; set; }
        public string ipowner { get; set; }
        public string producer { get; set; }
        public string creator { get; set; }
        public string submitter { get; set; }
        public string preservation { get; set; }
    }

    public class SystemInfo
    {
        public SystemInfo()
        {
            name = new Id_name();
        }

        public string id { get; set; }
        public Id_name name { get; set; }
        public string version { get; set; }
        public string type { get; set; }
        public string typeVersion { get; set; }
        public string vendor { get; set; }
        public string vendorOriginal { get; set; }

        public class Id_name
        {
            [XmlAttribute]
            public string id { get; set; }

            [XmlText]
            public string name { get; set; }
        }
    }

    public class ExtractorSoftware
    {
        public string name { get; set; }
        public string version { get; set; }
        public string type { get; set; }
        public string typeVersion { get; set; }
        public string vendor { get; set; }
    }

    public class File
    {
        public File()
        {
            result = new Result();
            inFile = new FileInfo();
            outFile = new FileInfo();
            warning = new Warning();
            error = new FileError();
        }

        [XmlAttribute]
        public string id { get; set; }
        public string ext { get; set; }
        public Result result { get; set; }
        public FileInfo inFile { get; set; }
        public FileInfo outFile { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public Warning warning { get; set; }
        public FileError error { get; set; }

        public class Result
        {
            [XmlAttribute]
            public int tool { get; set; }

            [XmlText]
            public string result { get; set; }
        }

        public class Warning
        {
            [XmlAttribute]
            public string toolNo { get; set; }

            [XmlAttribute]
            public string element { get; set; }

            [XmlAttribute]
            public string value1 { get; set; }

            [XmlAttribute]
            public string value2 { get; set; }

            [XmlText]
            public string text { get; set; }
        }

        public class FileError
        {
            [XmlAttribute]
            public string id { get; set; }

            [XmlText]
            public string text { get; set; }
        }

        public class FileInfo
        {
            [XmlAttribute]
            public string mime { get; set; }
            [XmlAttribute]
            public string ext { get; set; }
            [XmlAttribute]
            public string puid { get; set; }
            [XmlAttribute]
            public string name { get; set; }
            [XmlAttribute]
            public string version { get; set; }

            [XmlText]
            public string path { get; set; }
        }
    }
}
