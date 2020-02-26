﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KDRS_Analyse
{
    public partial class Form1 : Form
    {
        LogReader logReader = new LogReader();
        XMLReader xmlReader = new XMLReader();
        XMLWriter writer = new XMLWriter();

        string fileName = String.Empty;
        string outFolder = String.Empty;
        string outFileName = "analyse_v" + Globals.toolVersion;
        string inRootFolder = String.Empty;
        string outRootFolder = String.Empty;
        string outFile = String.Empty;

        int fileCount;

        List<string> checkedButtons = new List<string>();
        //******************************************************************

        public Form1()
        {
            InitializeComponent();

            //Globals.extractionAnalyse.tools = new ToolsWrapper();
            //Globals.extractionAnalyse.tools.tools = new List<AnalyseTool>();

            Text = Globals.toolName + " " + Globals.toolVersion;

            this.AllowDrop = true;
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
            this.DragEnter += new DragEventHandler(Form1_DragEnter);

            checkedButtons.Add("Checked buttons:");

            fileCount = 0;

            FillDict("pdf-to-puid.ini", Globals.puIdDict, "PDF-TO-PUID");
            FillDict("decom-log-sequences.ini", Globals.taskDict, "DECOM-LOG-TASK");
            FillDict("decom-log-sequences.ini", Globals.seqDict, "DECOM-LOG-SEQUENCE");
            FillDict("puid-to-type.ini", Globals.typeDict, "PUID-TO-TYPE");
            FillDict("mime-synonym.ini", Globals.mimeDict, "MIME-SYNONYM");
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
        //******************************************************************

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            fileCount = 0;

            lblProgress.Text = "";
            lblProgress.Refresh();

            fileName = files[0];
            if (files.Count() > 1)
                MessageBox.Show("One file at the time");
            else
            {
                outFolder = Path.GetDirectoryName(fileName);
                if ("" != txtBoxOutFolder.Text)
                    outFolder = txtBoxOutFolder.Text;

                if ("" != txtBoxOutFile.Text)
                    outFileName = txtBoxOutFile.Text;

                inRootFolder = txtBoxInRoot.Text;
                outRootFolder = txtBoxOutRoot.Text;

                outFile = Path.Combine(outFolder, outFileName + "_" + GetTimeStamp() + ".xml");

                Console.WriteLine("File name: " + fileName);

                this.AllowDrop = false;
                btnWriteXml.Enabled = false;
                btnReset.Enabled = false;

                backgroundWorker1 = new BackgroundWorker();
                backgroundWorker1.DoWork += backgroundWorker1_DoWork;
                backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
                backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
                backgroundWorker1.WorkerReportsProgress = true;
                backgroundWorker1.RunWorkerAsync();
            }
        }
        //******************************************************************

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

            try
            {
                Console.WriteLine("Reading file");

                ReadFile();

            }
            catch (Exception ex)
            {
                backgroundWorker1.ReportProgress(0, ex.Message);
            }

            Console.WriteLine("outFile: " + outFile);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            txtBoxInfoText.AppendText(e.UserState.ToString());
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (fileCount > 0)
                txtBoxInfoText.AppendText("File has been read with " + fileCount + " files!\r\n");
            else
                txtBoxInfoText.AppendText("File has been read!\r\n");

            this.AllowDrop = true;
            btnWriteXml.Enabled = true;
            btnReset.Enabled = true;

        }
        //******************************************************************

        private void ReadFile()
        {
            if (rBtnInfoXml.Checked)
            {
                logReader.OnProgressUpdate += reader_OnProgressUpdate;

                backgroundWorker1.ReportProgress(0, "info.xml: " + fileName + "\r\n");
                checkedButtons.Add("X - Info xml");

                xmlReader.ReadInfoXml(fileName);// readInfoXml
            }
            else if (rBtnDcmBlbRpt.Checked)
            {
                InitFiles();

                logReader.OnProgressUpdate += reader_OnProgressUpdate;

                Console.WriteLine("Dcm blobreport");
                backgroundWorker1.ReportProgress(0, "Decom Blobs report: " + fileName + "\r\n");
               // txtBoxInfoText.AppendText("Decom Blobs report: " + fileName + "\r\n");
                checkedButtons.Add("X - Decom blob report");

                logReader.ReadDcmBlbRpt(fileName, inRootFolder, outRootFolder);// readDcmBlbRpt
            }
            else if (rBtnDcmLog.Checked)
            {
                InitFiles();

                logReader.OnProgressUpdate += reader_OnProgressUpdate;

                backgroundWorker1.ReportProgress(0, "Decom log: " + fileName + "\r\n");
                checkedButtons.Add("X - Decom log");

                logReader.ReadDcmLog(fileName, inRootFolder, outRootFolder); // readDcmLog
            }
            else if (rBtnDrdFiles.Checked)
            {
                InitFiles();

                logReader.OnProgressUpdate += reader_OnProgressUpdate;

                Console.WriteLine("Droid files");
                backgroundWorker1.ReportProgress(0, "Droid files.csv: " + fileName + "\r\n");
                checkedButtons.Add("X - Droid files");

                logReader.ReadDroidFiles(fileName, rBtnProd.Checked, inRootFolder, outRootFolder, chkBoxIncXsd.Checked);
            }
            else if (rBtnIKAVALog.Checked)
                ; // readIKAVALog
            else if (rBtnIKAVANoConvFiles.Checked)
                ; // readIKAVANoConvFiles
            else if (rBtnVera.Checked)
            {
                InitFiles();

                xmlReader.OnProgressUpdate += reader_OnProgressUpdate;

                Console.WriteLine("veraPDF results");
                backgroundWorker1.ReportProgress(0, "veraPDF XML: " + fileName + "\r\n");
                xmlReader.ReadVeraPdf(fileName, outRootFolder, inRootFolder);
                checkedButtons.Add("X - veraPDF");
            }
            // readVera
            else if (rBtnKOSTVal.Checked)
            {
                InitFiles();
                xmlReader.OnProgressUpdate += reader_OnProgressUpdate;
                Console.WriteLine("KOST-Val results");
                backgroundWorker1.ReportProgress(0, "KOST-Val: " + fileName + "\r\n");

                xmlReader.ReadKostVal(fileName, outRootFolder); // readArk5Xml
                checkedButtons.Add("X - KOST-Val");
            }
            else if (rBtnAnalyseXML.Checked)
            {
                backgroundWorker1.ReportProgress(0, "analyse.xml: " + fileName + "\r\n");

                xmlReader.ReadXML(fileName); // readDcmN5Val
                checkedButtons.Add("X - Analyse.xml");
            }
            // osv for resten
        }
        //******************************************************************

        private void btnWriteXml_Click(object sender, EventArgs e)
        {
            try
            {
                writer.WriteXml(outFile);
            }
            catch (Exception ex)
            {
                txtBoxInfoText.AppendText(ex.Message);
            }

            txtBoxInfoText.AppendText("JOB COMPLETE! \r\n");
            txtBoxInfoText.AppendText("Resultfile: " + outFile + "\r\n");
        }
        //******************************************************************
        // Reset form and all results.
        private void btnReset_Click(object sender, EventArgs e)
        {
            backgroundWorker1.Dispose();
            txtBoxInfoText.Clear();
            lblProgress.Text = String.Empty;
            lblProgress.Refresh();

            fileName = String.Empty;
            Globals.toolCounter = 0;

            Properties.Settings.Default.Reset();

            Globals.extractionAnalyse = new ExtractionAnalyse
            {
                name = Globals.toolName,
                version = Globals.toolVersion,

                tools = new ToolsWrapper
                {
                    tools = new List<AnalyseTool>()
                }
            };

            Globals.fileDict.Clear();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        //******************************************************************

        public string GetTimeStamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd_HHmm");
        }
        //******************************************************************

        private void reader_OnProgressUpdate(int count)
        {
            base.Invoke((System.Action)delegate
            {
                fileCount = count;
                lblProgress.Text = fileCount.ToString();
                lblProgress.Refresh();
            });
        }
        //******************************************************************

        private void btnSaveLog_Click(object sender, EventArgs e)
        {
            string logFile = Path.Combine(outFolder, "analyse_log_" + DateTime.Now.ToString("yyyy-MM-dd-HHmm") + ".txt");

            string progInfo = Globals.toolName + " v" + Globals.toolVersion;

            string inRoot = "In root folder: " + inRootFolder;
            string outRoot = "Out root folder: " + outRootFolder;

            string analyseFolder = "Analyse folder: " + outFolder;
            string analyseFileName = "Analyse file name: " + outFileName;

            string[] names = { progInfo, inRoot, outRoot, analyseFolder, analyseFileName};

            System.IO.File.WriteAllLines(logFile, names);
            System.IO.File.AppendAllLines(logFile, checkedButtons);

            System.IO.File.AppendAllText(logFile, txtBoxInfoText.Text);

        }
        //******************************************************************

        private void FillDict(string file, Dictionary<string, string> dict, string section)
        {
            dict.Clear();
            Console.WriteLine("FillDict");

            string[] keyPair = null;

            if (File.Exists(file))
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains(section))
                        {
                            while ((line = sr.ReadLine()) != null)
                            {
                                if (line.StartsWith(";") || String.IsNullOrEmpty(line))
                                    continue;
                                if (line.StartsWith("["))
                                    break;
                                keyPair = line.Split('=');

                                Console.WriteLine(keyPair[0] + " " + keyPair[1]);
                            
                                dict.Add(keyPair[0], keyPair[1]);
                            }
                        }
                    }
                }
            }
            else
                txtBoxInfoText.AppendText("Ini file not found: " + file);
        }
        //******************************************************************

        private void InitFiles()
        {
            if (Globals.extractionAnalyse.files == null)
            {
                Globals.extractionAnalyse.files = new FilesWrapper();
                Globals.extractionAnalyse.files.files = new List<AnalyseFile>();
                Console.WriteLine("New Files");
            }
        }

        //******************************************************************
        private void btnSaveInput_Click(object sender, EventArgs e)
        {
            if ("" != txtBoxOutFolder.Text)
                outFolder = txtBoxOutFolder.Text;

            string saveInputFile = Path.Combine(outFolder, "analyse_input_parameters.txt");

            string inRoot = "In root folder: " + txtBoxInRoot.Text;
            string outRoot = "Out root folder: " + txtBoxOutRoot.Text;

            string analyseFolder = "Analyse folder: " + txtBoxOutFolder.Text;
            string analyseFileName = "Analyse file name: " + txtBoxOutFile.Text;

            string xmlChecked = chkBoxIncXsd.Checked.ToString();

            string[] names = { inRoot, outRoot, analyseFolder, analyseFileName, xmlChecked };

            File.WriteAllLines(saveInputFile, names);

            /*
            Properties.Settings.Default["inRootFolder"] = txtBoxInRoot.Text;
            Properties.Settings.Default["outRootFolder"] = txtBoxOutRoot.Text;
            Properties.Settings.Default["analyseFolder"] = txtBoxOutFolder.Text;
            Properties.Settings.Default["analyseFileName"] = txtBoxOutFile.Text;

            Properties.Settings.Default["incXsd"] = chkBoxIncXsd.Checked;

            Properties.Settings.Default.Save();*/
        }
        //******************************************************************

        private void btnLoadInput_Click(object sender, EventArgs e)
        {
            if ("" != txtBoxOutFolder.Text)
                outFolder = txtBoxOutFolder.Text;

            string[] input = null;
            string loadInputFile = Path.Combine(outFolder, "analyse_input_parameters.txt");
            if (File.Exists(loadInputFile))
            {
                input = File.ReadAllLines(loadInputFile);

                txtBoxInRoot.Text = input[0].Split(':')[1].Trim();
                txtBoxOutRoot.Text = input[1].Split(':')[1].Trim();
                txtBoxOutFolder.Text = input[2].Split(':')[1].Trim();
                txtBoxOutFile.Text = input[3].Split(':')[1].Trim();
                chkBoxIncXsd.Checked = Boolean.Parse(input[4].Trim());
            }
            /*
            txtBoxInRoot.Text = Properties.Settings.Default["inRootFolder"].ToString();
            txtBoxOutRoot.Text = Properties.Settings.Default["outRootFolder"].ToString();
            txtBoxOutFolder.Text = Properties.Settings.Default["analyseFolder"].ToString();
            txtBoxOutFile.Text = Properties.Settings.Default["analyseFileName"].ToString();

            chkBoxIncXsd.Checked = (bool) Properties.Settings.Default["incXsd"];*/
        }
    }

    //====================================================================================================

    public static class Globals
    {
        public static readonly String toolName = "KDRS Analyse";
        public static readonly String toolVersion = "0.5.1";

        public static int toolCounter = 0;
        public static ExtractionAnalyse extractionAnalyse = new ExtractionAnalyse
        {
            name = toolName,
            version = toolVersion,

            tools = new ToolsWrapper
            {
                tools = new List<AnalyseTool>()
            }
        };

        public static Dictionary<string, string> puIdDict = new Dictionary<string, string>();
        public static Dictionary<string, string> taskDict = new Dictionary<string, string>();
        public static Dictionary<string, string> seqDict = new Dictionary<string, string>();
        public static Dictionary<string, string> typeDict = new Dictionary<string, string>();
        public static Dictionary<string, string> mimeDict = new Dictionary<string, string>();

        public static Dictionary<string, AnalyseFile> fileDict = new Dictionary<string, AnalyseFile>();
    }
}
