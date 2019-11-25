using System;
using System.Collections.Generic;
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

        public Form1()
        {
            InitializeComponent();
            Text = Globals.toolName + " " + Globals.toolVersion;

            this.AllowDrop = true;
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
            this.DragEnter += new DragEventHandler(Form1_DragEnter);

            Globals.extractionAnalyse.files = new List<AnalyseFile>();

            checkedButtons.Add("Checked buttons:");

            fileCount = 0;
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

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
                    outFileName = Path.GetFileNameWithoutExtension(txtBoxOutFile.Text);

                inRootFolder = txtBoxInRoot.Text;
                outRootFolder = txtBoxOutRoot.Text;

                outFile = Path.Combine(outFolder, outFileName + "_" + GetTimeStamp() + ".xml");

                Console.WriteLine("File name: " + fileName);

                try
                {
                    ReadFile();
                    if (fileCount > 0)
                        txtBoxInfoText.AppendText("File has been read with " + fileCount + " files!\r\n");
                    else 
                        txtBoxInfoText.AppendText("File has been read!\r\n");
                }
                catch (Exception ex)
                {
                    txtBoxInfoText.AppendText(ex.Message);
                }

                Console.WriteLine("outFile: " + outFile);

            }
        }

        private void ReadFile()
        {
            if (rBtnInfoXml.Checked)
            {
                logReader.OnProgressUpdate += reader_OnProgressUpdate;

                txtBoxInfoText.AppendText("info.xml: " + fileName + "\r\n");
                xmlReader.ReadInfoXml(fileName);// readInfoXml
                checkedButtons.Add("X - Info xml");
            }
            else if (rBtnDcmBlbRpt.Checked)
            {
                Console.WriteLine("Dcm blobreport");
                txtBoxInfoText.AppendText("Decom Blobs report: " + fileName + "\r\n");
                logReader.OnProgressUpdate += reader_OnProgressUpdate;
                logReader.ReadDcmBlbRpt(fileName, inRootFolder, outRootFolder);// readDcmBlbRpt
                checkedButtons.Add("X - Decom blob report");

            }
            else if (rBtnDcmLog.Checked)
            {
                logReader.OnProgressUpdate += reader_OnProgressUpdate;

                logReader.ReadDcmLog(fileName, inRootFolder, outRootFolder); // readDcmLog
                txtBoxInfoText.AppendText("Decom log: " + fileName + "\r\n");
                checkedButtons.Add("X - Decom log");
            }
            else if (rBtnDrdFiles.Checked)
            {
                logReader.OnProgressUpdate += reader_OnProgressUpdate;

                Console.WriteLine("Droid files");
                txtBoxInfoText.AppendText("Droid files.csv: " + fileName + "\r\n");
                logReader.ReadDroidFiles(fileName, rBtnProd.Checked, inRootFolder, outRootFolder, chkBoxIncXsd.Checked);
                checkedButtons.Add("X - Droid files");

            }
            else if (rBtnIKAVALog.Checked)
                ; // readIKAVALog
            else if (rBtnIKAVANoConvFiles.Checked)
                ; // readIKAVANoConvFiles
            else if (rBtnVera.Checked)
            {
                xmlReader.OnProgressUpdate += reader_OnProgressUpdate;

                Console.WriteLine("veraPDF results");
                txtBoxInfoText.AppendText("veraPDF XML: " + fileName + "\r\n");
                xmlReader.ReadVeraPdf(fileName, outRootFolder, inRootFolder);
                checkedButtons.Add("X - veraPDF");
            }
                 // readVera
            else if (rBtnKOSTVal.Checked)
            {
                xmlReader.OnProgressUpdate += reader_OnProgressUpdate;
                Console.WriteLine("KOST-Val results");
                txtBoxInfoText.AppendText("KOST-Val: " + fileName + "\r\n");

                xmlReader.ReadKostVal(fileName, outRootFolder); // readArk5Xml
                checkedButtons.Add("X - KOST-Val");

            }
            else if (rBtnDcmN5val.Checked)
                ; // readDcmN5Val

            // osv for resten
        }

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

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtBoxInfoText.Clear();
            fileName = String.Empty;
            Globals.toolCounter = 0;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public string GetTimeStamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd_HHmm");
        }

        private void reader_OnProgressUpdate(int count)
        {
            base.Invoke((System.Action)delegate
            {
                fileCount = count;
                lblProgress.Text = fileCount.ToString();
                lblProgress.Refresh();
            });
        }

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
    }

    public static class UpdateStatus
    {
   }

    public static class Globals
    {
        public static readonly String toolName = "KDRS Analyse";
        public static readonly String toolVersion = "0.3";

        public static int toolCounter = 0;
        public static ExtractionAnalyse extractionAnalyse = new ExtractionAnalyse
        {
            name = toolName,
            version = toolVersion
        };


    }
}
