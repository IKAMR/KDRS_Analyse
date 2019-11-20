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

            xDoc = new XPathDocument(fileName);

            nav = xDoc.CreateNavigator();

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nav.NameTable);

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

            Globals.extractionAnalyse.agents = readAgents;
        }

        public void ReadVeraPdf(string fileName, string outRootFolder, string inRootFolder)
        {
            Globals.toolCounter++;
            Console.WriteLine(Globals.toolCounter);
            AnalyseTool veraTool = new AnalyseTool();

            veraTool.buildInformation = new List<AnalyseTool.VeraRelease>();

            veraTool.batchSummary = new AnalyseTool.VeraSummary();
            //veraTool.files = new AnalyseTool.DcmFiles();
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

            veraTool.version = nav.SelectSingleNode("//report/buildInformation/releaseDetails[@id='gui']/@version").ToString();

            nodeIter = nav.Select("//report/buildInformation/releaseDetails ", nsmgr);

            while (nodeIter.MoveNext())
            {
                AnalyseTool.VeraRelease detail = new AnalyseTool.VeraRelease();
                detail.id = nodeIter.Current.GetAttribute("id", nsmgr.DefaultNamespace);
                detail.version = nodeIter.Current.GetAttribute("version", nsmgr.DefaultNamespace);
                detail.buildDate = nodeIter.Current.GetAttribute("buildDate", nsmgr.DefaultNamespace);

                veraTool.buildInformation.Add(detail);
            }

            AnalyseTool.VeraSummary summary = new AnalyseTool.VeraSummary();
            summary.totalJobs = nav.SelectSingleNode("//report/batchSummary/@totalJobs").ToString();
            summary.failedToParse = nav.SelectSingleNode("//report/batchSummary/@failedToParse").ToString();
            summary.encrypted = nav.SelectSingleNode("//report/batchSummary/@encrypted").ToString();

            Console.WriteLine("Val report");
            summary.validationReports = new AnalyseTool.VeraSummary.VeraValReport();
            summary.validationReports.compliant = nav.SelectSingleNode("//report/batchSummary/validationReports/@compliant").ToString();
            summary.validationReports.nonCompliant = nav.SelectSingleNode("//report/batchSummary/validationReports/@nonCompliant").ToString();
            summary.validationReports.failedJobs = nav.SelectSingleNode("//report/batchSummary/validationReports/@failedJobs").ToString();
            summary.validationReports.total = nav.SelectSingleNode("//report/batchSummary/validationReports").Value;

            Console.WriteLine("feature report");
            summary.featureReports = new AnalyseTool.VeraSummary.FeatureReports();
            summary.featureReports.failedJobs= nav.SelectSingleNode("//report/batchSummary/featureReports/@failedJobs").ToString();
            summary.featureReports.total= nav.SelectSingleNode("//report/batchSummary/featureReports").Value;

            Console.WriteLine("repair report");
            summary.repairReports = new AnalyseTool.VeraSummary.ReparirReports();
            summary.repairReports.failedJobs = nav.SelectSingleNode("//report/batchSummary/repairReports/@failedJobs").ToString();
            summary.repairReports.total = nav.SelectSingleNode("//report/batchSummary/repairReports").Value;

            summary.duration = new AnalyseTool.VeraSummary.VeraDuration();
            summary.duration.start = nav.SelectSingleNode("//report/batchSummary/duration/@start").ToString();
            summary.duration.finish = nav.SelectSingleNode("//report/batchSummary/duration/@finish").ToString();
            summary.duration.total = nav.SelectSingleNode("//report/batchSummary/duration").Value;

            veraTool.batchSummary = summary;

            veraTool.outputPath = outRootFolder;

            Globals.extractionAnalyse.tools.Add(veraTool);
            
            nodeIter = nav.Select("//report/jobs/job", nsmgr);

            XPathExpression getProfileName = nav.Compile("descendant::validationReport/profileName");

            int fileCount = 0;

            while (nodeIter.MoveNext())
            {
                string name = nodeIter.Current.SelectSingleNode("descendant::item/name").Value;
                string fileId = LogReader.GetFileId(name, outRootFolder);

                AnalyseFile veraFile = LogReader.GetFile(fileId);

                if (String.IsNullOrEmpty(veraFile.id))
                {
                    newFile = true;
                    veraFile.id = fileId;
                }

                veraFile.valid = new AnalyseFile.Valid();

                veraFile.valid.toolId = veraTool.toolId;
                string isCompliant = nodeIter.Current.SelectSingleNode("descendant::validationReport/@isCompliant").Value;
                veraFile.valid.isValid = isCompliant;
                Console.WriteLine("Is compliant: " + isCompliant);
                
                veraFile.valid.passedRules = nodeIter.Current.SelectSingleNode("descendant::validationReport/details/@passedRules").Value;
                veraFile.valid.failedRules = nodeIter.Current.SelectSingleNode("descendant::validationReport/details/@failedRules").Value;
                veraFile.valid.passedChecks = nodeIter.Current.SelectSingleNode("descendant::validationReport/details/@passedChecks").Value;
                veraFile.valid.failedChecks = nodeIter.Current.SelectSingleNode("descendant::validationReport/details/@failedChecks").Value;
                
                string profName = nodeIter.Current.SelectSingleNode("descendant::validationReport/@profileName").Value;

                string profile = profName.Split(' ')[0];
                string pdfAtype = "";
                if (profile.Contains("PDF"))
                {
                    veraFile.valid.type = profile.Split('-')[0];
                    pdfAtype = profile.Split('-')[1];
                }

                if (newFile)
                    Globals.extractionAnalyse.files.Add(veraFile);
                fileCount++;
                OnProgressUpdate?.Invoke(fileCount);

            }

        }

        public void ReadKostVal(string fileName, string outRootFolder)
        {
            Globals.toolCounter++;
            Console.WriteLine(Globals.toolCounter);
            AnalyseTool kostValTool = new AnalyseTool();


            kostValTool.toolNo = Globals.toolCounter.ToString();
            kostValTool.toolId = "104";
            kostValTool.name = "KOST-Val";
            kostValTool.version = "";

            Console.WriteLine("Tool created");

            XPathNavigator nav;
            XPathDocument xDoc;
            XPathNodeIterator nodeIter;

            xDoc = new XPathDocument(fileName);

            nav = xDoc.CreateNavigator();

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nav.NameTable);
            nsmgr.AddNamespace("s", "");

            kostValTool.outputPath = outRootFolder;

            kostValTool.KOSTValInfo = new AnalyseTool.KOSTValToolInfo();

            kostValTool.KOSTValInfo.Start = nav.SelectSingleNode("//KOSTValLog/Infos/Start").Value;
            kostValTool.KOSTValInfo.End = nav.SelectSingleNode("//KOSTValLog/Infos/End").Value;

            kostValTool.KOSTValInfo.FormatValOn = nav.SelectSingleNode("//KOSTValLog/Infos/FormatValOn").Value;
            kostValTool.KOSTValInfo.Info = nav.SelectSingleNode("//KOSTValLog/Infos/Info").Value;
            kostValTool.KOSTValInfo.configuration = nav.SelectSingleNode("//KOSTValLog/configuration").Value;

            kostValTool.KOSTValSummary = new AnalyseTool.KostValSummary();

            kostValTool.KOSTValSummary.Summary = nav.SelectSingleNode("//KOSTValLog/Format/Infos/Summary").Value;
            string notValFiles = nav.SelectSingleNode("//KOSTValLog/Format/Infos/Info/Message").Value;
            kostValTool.KOSTValSummary.Info = notValFiles;


            nodeIter = nav.Select("//KOSTValLog/Format/Validation", nsmgr);

            int fileCount = 0;

            while (nodeIter.MoveNext())
            {
                string name = nodeIter.Current.SelectSingleNode("descendant::ValFile").Value;
                string fileId = LogReader.GetFileId(name, outRootFolder);
                Console.WriteLine("File ID: " + fileId);

                AnalyseFile kostValFile = LogReader.GetFile(fileId);

                if (String.IsNullOrEmpty(kostValFile.id))
                {
                    newFile = true;
                    kostValFile.id = fileId;
                }
                
                kostValFile.valid = new AnalyseFile.Valid();


                kostValFile.valid.toolId = kostValTool.toolId;

                string isCompliant = "";
                XPathNavigator valid = nodeIter.Current.SelectSingleNode("descendant::Valid");
                if (valid != null)
                    isCompliant = valid.Value;
                else if((valid = nodeIter.Current.SelectSingleNode("descendant::Invalid")) != null)
                {
                    isCompliant = valid.Value;
                    Console.WriteLine("Error found");
                    kostValFile.valid.error = new List<AnalyseFile.Valid.KostError>();

                    XPathNodeIterator errorNav = nodeIter.Current.Select("descendant::Error");

                    while (errorNav.MoveNext())
                    {
                        AnalyseFile.Valid.KostError error = new AnalyseFile.Valid.KostError
                        {
                            modul = errorNav.Current.SelectSingleNode("descendant::Modul").Value,
                            message = errorNav.Current.SelectSingleNode("descendant::Message").Value
                        };

                        kostValFile.valid.error.Add(error);
                    }
                }

                Console.WriteLine("Is compliant: " + isCompliant);

                if (String.IsNullOrEmpty(isCompliant))
                {
                    isCompliant = nodeIter.Current.SelectSingleNode("descendant::Invalid").Value;
                }
                kostValFile.valid.isValid = isCompliant;
                
                if (isCompliant.Equals("invalid"))
                {
                    
                }
            
                string valType = nodeIter.Current.SelectSingleNode("descendant::ValType").Value;
                kostValFile.valid.type = valType.Split(':').ToString().Trim();
         
                if (valType.Contains("PDF"))
                {
                    string pdfAtype = nodeIter.Current.SelectSingleNode("descendant::FormatVL").Value;
                    
                }

                if (newFile)
                    Globals.extractionAnalyse.files.Add(kostValFile);
                fileCount++;
                OnProgressUpdate?.Invoke(fileCount);
            }

            Globals.extractionAnalyse.tools.Add(kostValTool);
            Console.WriteLine("Tool added");
        }
    }
}
