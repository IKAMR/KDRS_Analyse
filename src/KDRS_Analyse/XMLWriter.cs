using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KDRS_Analyse
{
    class XMLWriter
    {
        public void writeXml(string fileName)
        {
            XmlSerializer ser = new XmlSerializer(typeof(ExtractionAnalyse));
            using (TextWriter writer = new StreamWriter(fileName))
            {
                ser.Serialize(writer, Globals.extractionAnalyse);
            }
        }
    }

    public class ExtractionAnalyse
    {
        public List<Tool> tools { get; set; }
        public Info info { get; set; }
        public Agents agents { get; set; }
        public SystemInfo system { get; set; }
        public List<File> files { get; set; }

    }
    /*
    public class Tools
    {
        public List<Tool> tool { get; set; }
    }
    */

    [AttributeUsage(AttributeTargets.Property,
    Inherited = false,
    AllowMultiple = false)]
    internal sealed class OptionalAttribute : Attribute
    {
    }

    public class Tool
    {
        public Tool(string no, string id, string name, string version)
        {
            this.no = no;
            this.id = id;
            this.name = name;
            this.version = version;
        }

        [XmlAttribute]
        public string no { get; set; }

        [XmlAttribute]
        public string id { get; set; }

        [XmlAttribute]
        public string name { get; set; }

        [XmlAttribute]
        public string version { get; set; }

        [Optional]
        public string project { get; set; }

        [Optional]
        public DcmTool dcmTool { get; set; }

        [Optional]
        public DcmFiles files { get; set; }

        public List<string> inputPath { get; set; }
        public string outputPath { get; set; }


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
        public string id { get; set; }
        public Id_name name { get; set; }
        public string version { get; set; }
        public string type { get; set; }
        public string typeVersion { get; set; }
        public string vendor { get; set; }
        public string vendorOriginal { get; set; }
    }

    public class Id_name
    {
        [XmlAttribute]
        public string id { get; set; }

        public string name { get; set; }
    }

    public class ExtractorSoftware
    {
        public string name { get; set; }
        public string version { get; set; }
        public string type { get; set; }
        public string typeVersion { get; set; }
        public string vendor { get; set; }
    }
    /*
    public class Files
    {
        public File[] file { get; set; }
    }
    */
    public class File
    {
        public string mime { get; set; }
        public string ext { get; set; }
        public string result { get; set; }
        public string input { get; set; }
        public string output { get; set; }
        public string start { get; set; }
        public string end { get; set; }

    }

    public class DcmTool
    {

        [XmlAttribute]
        public string version { get; set; }

        public string name { get; set; }
    }

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

        public string files { get; set; }
    }
}
