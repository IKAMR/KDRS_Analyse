using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace KDRS_Analyse
{
    class XMLReader
    {
        public delegate void ProgressUpdate(int count);
        public event ProgressUpdate OnProgressUpdate;

        public bool newFile = false;

        public void ReadInfoXml(string fileName)
        {
            XPathNavigator nav;
            XPathDocument xDoc;
            XPathNodeIterator nodeIter;

            xDoc = new XPathDocument(fileName);

            nav = xDoc.CreateNavigator();

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nav.NameTable);
            // var nameSpace = nav.GetNamespace(nav.SelectSingleNode("arkiv").NamespaceURI);
            //nsmgr.AddNamespace("a", nameSpace);

            nsmgr.AddNamespace("mets", "http://www.loc.gov/METS/");
            nsmgr.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            Agents readAgents = new Agents();

            XPathNavigator node = nav.SelectSingleNode("//mets:agent[@ROLE = 'ARCHIVIST' and @TYPE = 'ORGANIZATION']/mets:name", nsmgr);

            readAgents.archivist = node.InnerXml;

            node = nav.SelectSingleNode("//mets:agent[@ROLE = 'IPOWNER' and @TYPE = 'ORGANIZATION']/mets:name", nsmgr);
            readAgents.ipowner = node.InnerXml;

            node = nav.SelectSingleNode("//mets:agent[@OTHERROLE = 'PRODUCER' and @TYPE = 'ORGANIZATION']/mets:name", nsmgr);
            readAgents.producer = node.InnerXml;

            node = nav.SelectSingleNode("//mets:agent[@ROLE = 'CREATOR' and @TYPE = 'ORGANIZATION']/mets:name", nsmgr);
            readAgents.creator = node.InnerXml;

            node = nav.SelectSingleNode("//mets:agent[@OTHERROLE = 'SUBMITTER' and @TYPE = 'ORGANIZATION']/mets:name", nsmgr);
            readAgents.submitter = node.InnerXml;

            node = nav.SelectSingleNode("//mets:agent[@ROLE = 'PRESERVATION' and @TYPE = 'ORGANIZATION']/mets:name", nsmgr);
            readAgents.preservation = node.InnerXml;

           // Console.WriteLine("ipowner: " + node.InnerXml);

            Globals.extractionAnalyse.agents = readAgents;
        }

        public void ReadVeraPdf(string fileName, string outRootFolder, string inRootFolder)
        {

            Globals.toolCounter++;
            Console.WriteLine(Globals.toolCounter);
            AnalyseTool veraTool = new AnalyseTool();

            veraTool.buildInformation = new List<AnalyseTool.VeraRelease>();

            veraTool.batchSummary = new AnalyseTool.VeraSummary();
            veraTool.files = new AnalyseTool.DcmFiles();
            veraTool.toolNo = Globals.toolCounter.ToString();
            veraTool.toolId = "103";
            veraTool.name = "veraPDF";
            veraTool.version = "";

            Console.WriteLine("Tool created");

            XPathNavigator nav;
            XPathDocument xDoc;
            XPathNodeIterator nodeIter;

            xDoc = new XPathDocument(fileName);

            nav = xDoc.CreateNavigator();

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nav.NameTable);
            nsmgr.AddNamespace("s", "");
            //nsmgr.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");

            veraTool.version = nav.SelectSingleNode("//report/buildInformation/releaseDetails[@id='gui']/@version").ToString();

            nodeIter = nav.Select("//report/buildInformation/releaseDetails ", nsmgr);

            Console.WriteLine("Biuldinfomations: " + nodeIter.Count);

            while (nodeIter.MoveNext())
            {
                AnalyseTool.VeraRelease detail = new AnalyseTool.VeraRelease();
               //string id = nodeIter.Current.GetAttribute("id", nsmgr.DefaultNamespace);
                detail.id = nodeIter.Current.GetAttribute("id", nsmgr.DefaultNamespace);
                detail.version = nodeIter.Current.GetAttribute("version", nsmgr.DefaultNamespace);
                detail.buildDate = nodeIter.Current.GetAttribute("buildDate", nsmgr.DefaultNamespace);

                veraTool.buildInformation.Add(detail);
            }

            nodeIter = nav.SelectSingleNode("//report/batchSummary", nsmgr).SelectChildren(XPathNodeType.All);

            Console.WriteLine("Biuldinfomations: " + nodeIter.Count);
/*
            while (nodeIter.MoveNext())
            {
                AnalyseTool.VeraSummary summary = new AnalyseTool.VeraSummary();
                //string id = nodeIter.Current.GetAttribute("id", nsmgr.DefaultNamespace);
                summary.id = nodeIter.Current.GetAttribute("id", nsmgr.DefaultNamespace);
                summary.version = nodeIter.Current.GetAttribute("version", nsmgr.DefaultNamespace);
                summary.buildDate = nodeIter.Current.GetAttribute("buildDate", nsmgr.DefaultNamespace);

                veraTool.buildInformation.Add(detail);
            }*/

            veraTool.outputPath = outRootFolder;

            Globals.extractionAnalyse.tools.Add(veraTool);
            Console.WriteLine("Tool added");


            nodeIter = nav.Select("//report/jobs/job", nsmgr);

            Console.WriteLine("File count: " + nodeIter.Count);

            XPathExpression getProfileName = nav.Compile("descendant::validationReport/profileName");

            while (nodeIter.MoveNext())
            {
                string name = nodeIter.Current.SelectSingleNode("descendant::item/name").Value;

                string fileId = LogReader.GetFileId(fileName, inRootFolder);
                Console.WriteLine("ID: " + fileId);

                AnalyseFile veraFile = LogReader.GetFile(fileId);

                if (String.IsNullOrEmpty(veraFile.id))
                {
                    newFile = true;
                    veraFile.id = fileId;
                }


                string profName = nodeIter.Current.SelectSingleNode("descendant::validationReport/@profileName").Value;
                Console.WriteLine("profileName: " + profName);

                string isCompliant = nodeIter.Current.SelectSingleNode("descendant::validationReport/@isCompliant").Value;

                Console.WriteLine("isCompliant: " + isCompliant);

                if (newFile)
                    Globals.extractionAnalyse.files.Add(veraFile);
            }

        }
    }
}
