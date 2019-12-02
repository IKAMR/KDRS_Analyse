using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace KDRS_Analyse
{
    class XMLReader
    {
        public delegate void ProgressUpdate(int count);
        public event ProgressUpdate OnProgressUpdate;

        public bool newFile = false;
        //------------------------------------------------------------------------------------
        public void ReadXML(string fileName)
        {
            Console.WriteLine("Reading xml");
            XmlSerializer ser = new XmlSerializer(typeof(ExtractionAnalyse));
            using (TextReader reader = new StreamReader(fileName))
            {
                try
                {
                    Globals.extractionAnalyse = (ExtractionAnalyse)ser.Deserialize(reader);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            /*
            using (XmlReader reader = XmlReader.Create(fileName))
            {

            }*/
        }

        //------------------------------------------------------------------------------------
        public void ReadInfoXml(string fileName)
        {
            XPathNavigator nav;
            XPathDocument xDoc;

            xDoc = new XPathDocument(fileName);

            nav = xDoc.CreateNavigator();

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nav.NameTable);

            nsmgr.AddNamespace("mets", "http://www.loc.gov/METS/");
            nsmgr.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");

            AnalyseInfo info = new AnalyseInfo();

            XPathNavigator node = nav.SelectSingleNode("//mets:mets/@OBJID", nsmgr);

            string uuid = node.InnerXml;
            info.ipUuid = uuid.Split(':')[1];

            node = nav.SelectSingleNode("//mets:mets/@LABEL", nsmgr);
            info.deliveryspecification = GetInnerXml(node);

            node = nav.SelectSingleNode("//mets:mets/@TYPE", nsmgr);
            info.ipType = GetInnerXml(node);

            node = nav.SelectSingleNode("//mets:altRecordID[@TYPE='DELIVERYSPECIFICATION']", nsmgr);
            info.extractionDateTime = GetInnerXml(node);

            node = nav.SelectSingleNode("//mets:altRecordID[@TYPE='SUBMISSIONAGREEMENT']", nsmgr);
            info.submissionagreement = GetInnerXml(node);

            node = nav.SelectSingleNode("//mets:altRecordID[@TYPE='STARTDATE']", nsmgr);
            info.startdate = GetInnerXml(node);

            node = nav.SelectSingleNode("//mets:altRecordID[@TYPE='ENDDATE']", nsmgr);
            info.enddate = GetInnerXml(node);

            Globals.extractionAnalyse.info = info;

            Agents readAgents = new Agents();

            node = nav.SelectSingleNode("//mets:agent[@ROLE = 'ARCHIVIST' and @TYPE = 'ORGANIZATION']/mets:name", nsmgr);

            readAgents.archivist = GetInnerXml(node);

            node = nav.SelectSingleNode("//mets:agent[@ROLE = 'IPOWNER' and @TYPE = 'ORGANIZATION']/mets:name", nsmgr);
            readAgents.ipowner = GetInnerXml(node);

            node = nav.SelectSingleNode("//mets:agent[@OTHERROLE = 'PRODUCER' and @TYPE = 'ORGANIZATION']/mets:name", nsmgr);
            readAgents.producer = GetInnerXml(node);

            node = nav.SelectSingleNode("//mets:agent[@ROLE = 'CREATOR' and @TYPE = 'ORGANIZATION']/mets:name", nsmgr);
            readAgents.creator = GetInnerXml(node);

            node = nav.SelectSingleNode("//mets:agent[@OTHERROLE = 'SUBMITTER' and @TYPE = 'ORGANIZATION']/mets:name", nsmgr);
            readAgents.submitter = GetInnerXml(node);

            node = nav.SelectSingleNode("//mets:agent[@ROLE = 'PRESERVATION' and @TYPE = 'ORGANIZATION']/mets:name", nsmgr);
            readAgents.preservation = GetInnerXml(node);

            Globals.extractionAnalyse.agents = readAgents;

            SystemInfo system = new SystemInfo();

            node = nav.SelectSingleNode("//mets:agent[@ROLE = 'ARCHIVIST' and @TYPE = 'OTHER' and @OTHERTYPE = 'SOFTWARE']/mets:name", nsmgr);
            system.name.name = GetInnerXml(node);

            node = nav.SelectSingleNode("//mets:agent[@ROLE = 'ARCHIVIST' and @TYPE = 'OTHER' and @OTHERTYPE = 'SOFTWARE']/mets:note", nsmgr);
            system.version = GetInnerXml(node);

            node = nav.SelectSingleNode("//mets:agent[@ROLE = 'ARCHIVIST' and @TYPE = 'OTHER' and @OTHERTYPE = 'SOFTWARE']/mets:note[2]", nsmgr);
            system.type = GetInnerXml(node);

            node = nav.SelectSingleNode("//mets:agent[@ROLE = 'ARCHIVIST' and @TYPE = 'OTHER' and @OTHERTYPE = 'SOFTWARE']/mets:note[3]", nsmgr);
            system.typeVersion = GetInnerXml(node);

            Globals.extractionAnalyse.system = system;

            ExtractorSoftware extractorInfo = new ExtractorSoftware();

            node = nav.SelectSingleNode("//mets:agent[@ROLE = 'OTHER' and @OTHERROLE='PRODUCER'  and @TYPE = 'OTHER' and @OTHERTYPE = 'SOFTWARE']/mets:name", nsmgr);
            extractorInfo.name = GetInnerXml(node);

            node = nav.SelectSingleNode("//mets:agent[@ROLE = 'OTHER' and @OTHERROLE='PRODUCER'  and @TYPE = 'OTHER' and @OTHERTYPE = 'SOFTWARE']/mets:note", nsmgr);
            extractorInfo.version = GetInnerXml(node);

            node = nav.SelectSingleNode("//mets:agent[@ROLE = 'OTHER' and @OTHERROLE='PRODUCER'  and @TYPE = 'OTHER' and @OTHERTYPE = 'SOFTWARE']/mets:note[2]", nsmgr);
            extractorInfo.type = GetInnerXml(node);

            node = nav.SelectSingleNode("//mets:agent[@ROLE = 'OTHER' and @OTHERROLE='PRODUCER'  and @TYPE = 'OTHER' and @OTHERTYPE = 'SOFTWARE']/mets:note[3]", nsmgr);
            extractorInfo.typeVersion = GetInnerXml(node);

            Globals.extractionAnalyse.extractorSoftware = extractorInfo;

        }
        //------------------------------------------------------------------------------------
        public string GetInnerXml(XPathNavigator node)
        {
            if (node != null)
                return node.InnerXml;
            return null;
        }

        //------------------------------------------------------------------------------------

        public void ReadVeraPdf(string fileName, string outRootFolder, string inRootFolder)
        {
            Globals.toolCounter++;
           // Console.WriteLine(Globals.toolCounter);
            AnalyseTool veraTool = new AnalyseTool();

            veraTool.buildInformation = new List<AnalyseTool.VeraRelease>();

            veraTool.batchSummary = new AnalyseTool.VeraSummary();
            //veraTool.files = new AnalyseTool.DcmFiles();
            veraTool.toolNo = Globals.toolCounter.ToString();
            veraTool.toolId = "103";
            veraTool.name = "veraPDF";
            veraTool.version = "";

          //  Console.WriteLine("Tool created");

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

           // Console.WriteLine("Val report");
            summary.validationReports = new AnalyseTool.VeraSummary.VeraValReport();
            summary.validationReports.compliant = nav.SelectSingleNode("//report/batchSummary/validationReports/@compliant").ToString();
            summary.validationReports.nonCompliant = nav.SelectSingleNode("//report/batchSummary/validationReports/@nonCompliant").ToString();
            summary.validationReports.failedJobs = nav.SelectSingleNode("//report/batchSummary/validationReports/@failedJobs").ToString();
            summary.validationReports.total = nav.SelectSingleNode("//report/batchSummary/validationReports").Value;

            //Console.WriteLine("feature report");
            summary.featureReports = new AnalyseTool.VeraSummary.FeatureReports();
            summary.featureReports.failedJobs= nav.SelectSingleNode("//report/batchSummary/featureReports/@failedJobs").ToString();
            summary.featureReports.total= nav.SelectSingleNode("//report/batchSummary/featureReports").Value;

           // Console.WriteLine("repair report");
            summary.repairReports = new AnalyseTool.VeraSummary.ReparirReports();
            summary.repairReports.failedJobs = nav.SelectSingleNode("//report/batchSummary/repairReports/@failedJobs").ToString();
            summary.repairReports.total = nav.SelectSingleNode("//report/batchSummary/repairReports").Value;

            summary.duration = new AnalyseTool.VeraSummary.VeraDuration();
            summary.duration.start = nav.SelectSingleNode("//report/batchSummary/duration/@start").ToString();
            summary.duration.finish = nav.SelectSingleNode("//report/batchSummary/duration/@finish").ToString();
            summary.duration.total = nav.SelectSingleNode("//report/batchSummary/duration").Value;

            veraTool.batchSummary = summary;

            veraTool.outputPath = outRootFolder;

            Globals.extractionAnalyse.tools.tools.Add(veraTool);
            
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

                AnalyseFile.Valid fileValid = new AnalyseFile.Valid();

                fileValid.toolId = veraTool.toolId;
                string isCompliant = nodeIter.Current.SelectSingleNode("descendant::validationReport/@isCompliant").Value;
                fileValid.isValid = isCompliant;
               // Console.WriteLine("Is compliant: " + isCompliant);

                fileValid.passedRules = nodeIter.Current.SelectSingleNode("descendant::validationReport/details/@passedRules").Value;
                fileValid.failedRules = nodeIter.Current.SelectSingleNode("descendant::validationReport/details/@failedRules").Value;
                fileValid.passedChecks = nodeIter.Current.SelectSingleNode("descendant::validationReport/details/@passedChecks").Value;
                fileValid.failedChecks = nodeIter.Current.SelectSingleNode("descendant::validationReport/details/@failedChecks").Value;
                
                string profName = nodeIter.Current.SelectSingleNode("descendant::validationReport/@profileName").Value;

                string profile = profName.Split(' ')[0];
                fileValid.type = profile;

                if (profile.Contains("PDF"))
                {
                    if (Globals.puIdDict.ContainsKey(profile.ToLower()))
                        fileValid.puid = Globals.puIdDict[profile.ToLower()];
                }

                string readPuId = veraFile.outFile.puid;
                if (!String.IsNullOrEmpty(readPuId) && !String.IsNullOrEmpty(fileValid.puid))
                {
                    if (!readPuId.Equals(fileValid.puid))
                    {
                        PuIdWarning(veraFile, readPuId, fileValid.puid, veraTool.toolId);
                    }
                }

                veraFile.valid.Add(fileValid);

                if (newFile)
                    Globals.extractionAnalyse.files.files.Add(veraFile);
                fileCount++;
                OnProgressUpdate?.Invoke(fileCount);
            }
        }
        //------------------------------------------------------------------------------------

        public void ReadKostVal(string fileName, string outRootFolder)
        {
            Globals.toolCounter++;
           // Console.WriteLine(Globals.toolCounter);
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
            Console.WriteLine("New summary");
            XPathNavigator node = nav.SelectSingleNode("//KOSTValLog/Format/Infos/Summary");

            string kostSummary = GetInnerXml(node);

            kostValTool.KOSTValSummary.Summary = kostSummary;

            if (!String.IsNullOrEmpty(kostSummary))
            {
                string[] summarySplit = { "From the", "files", "valid,", "invalid and", "(" };
                kostValTool.KOSTValSummary.totalFiles = kostSummary.Split(summarySplit, 7, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                kostValTool.KOSTValSummary.valid = kostSummary.Split(summarySplit, 7, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                kostValTool.KOSTValSummary.invalid = kostSummary.Split(summarySplit, 7, StringSplitOptions.RemoveEmptyEntries)[3].Trim();
                kostValTool.KOSTValSummary.notValidated = kostSummary.Split(summarySplit, 7, StringSplitOptions.RemoveEmptyEntries)[5].Trim();
            }

            Console.WriteLine("Get message");
            node = nav.SelectSingleNode("//KOSTValLog/Format/Infos/Info/Message");
            string notValFiles = GetInnerXml(node);
            kostValTool.KOSTValSummary.Info = notValFiles;

            Console.WriteLine("Selecting files");

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
                
                AnalyseFile.Valid fileValid = new AnalyseFile.Valid();

                fileValid.toolId = kostValTool.toolId;

                string isCompliant = "";
                XPathNavigator valid = nodeIter.Current.SelectSingleNode("descendant::Valid");
                if (valid != null)
                    isCompliant = "true";
                else if((valid = nodeIter.Current.SelectSingleNode("descendant::Invalid")) != null)
                {
                    isCompliant = "false";
                    Console.WriteLine("Error found");
                    AnalyseFile.FileError error = new AnalyseFile.FileError();
                    error.id = kostValTool.toolId;
                    error.kostErrors = new List<AnalyseFile.KostError>();

                    XPathNodeIterator errorNav = nodeIter.Current.Select("descendant::Error");

                    while (errorNav.MoveNext())
                    {
                        AnalyseFile.KostError kostError = new AnalyseFile.KostError
                        {
                            modul = errorNav.Current.SelectSingleNode("descendant::Modul").Value,
                            message = errorNav.Current.SelectSingleNode("descendant::Message").Value
                        };

                        error.kostErrors.Add(kostError);
                    }
                    kostValFile.errors.Add(error);
                }

                Console.WriteLine("Is compliant: " + isCompliant);

                if (String.IsNullOrEmpty(isCompliant))
                {
                    isCompliant = nodeIter.Current.SelectSingleNode("descendant::Invalid").Value;
                }
                fileValid.isValid = isCompliant;
                
                string valType = nodeIter.Current.SelectSingleNode("descendant::ValType").Value;
                fileValid.type = valType.Split(':')[1].ToString().Trim();
         
                if (valType.Contains("PDF"))
                {
                    XPathNavigator typeNode = nodeIter.Current.SelectSingleNode("descendant::FormatVL");
                    string pdfAtype = GetInnerXml(typeNode);

                    string fileType = "PDF/A" + pdfAtype;
                    fileValid.type = fileType;

                    if (Globals.puIdDict.ContainsKey(fileType.ToLower()))
                        fileValid.puid = Globals.puIdDict[fileType.ToLower()];
                }

                string readPuId = kostValFile.outFile.puid;
                if (!String.IsNullOrEmpty(readPuId) )
                {
                    if (String.IsNullOrEmpty(fileValid.puid))
                    {
                        if (Globals.typeDict.ContainsKey(readPuId))
                        {
                            string readType = Globals.typeDict[readPuId];
                            if (!readType.Equals(fileValid.type.ToLower()))
                                PuIdWarning(kostValFile, readType, fileValid.type, kostValTool.toolId);

                        }
                    } else if (!readPuId.Equals(fileValid.puid))
                    {
                        PuIdWarning(kostValFile, readPuId, fileValid.puid, kostValTool.toolId);
                    }
                }

                kostValFile.valid.Add(fileValid);

                if (newFile)
                    Globals.extractionAnalyse.files.files.Add(kostValFile);
                fileCount++;
                OnProgressUpdate?.Invoke(fileCount);
            }

            Globals.extractionAnalyse.tools.tools.Add(kostValTool);
           // Console.WriteLine("Tool added");
        }
        //------------------------------------------------------------------------------------

        public static void PuIdWarning(AnalyseFile file, string origPuId, string newPuID, string toolID)
        {
            AnalyseFile.AnalyseWarning warning = new AnalyseFile.AnalyseWarning();
            warning.toolId = toolID;

            warning.value1 = origPuId;
            warning.value2 = newPuID;
            warning.text = "PUID mismatch";

            file.warning.Add(warning);
        }
    }
}
