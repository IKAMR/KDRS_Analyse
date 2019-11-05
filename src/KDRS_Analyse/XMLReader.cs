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
    }
}
