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

        public Form1()
        {
            InitializeComponent();
            Text = Globals.toolName + " " + Globals.toolVersion;

            this.AllowDrop = true;
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
            this.DragEnter += new DragEventHandler(Form1_DragEnter);

            Globals.extractionAnalyse.files = new List<File>();

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
                txtBoxInfoText.AppendText("info.xml: " + fileName + "\r\n");
                xmlReader.ReadInfoXml(fileName);// readInfoXml
            }
            else if (rBtnDcmBlbRpt.Checked)
            {
                Console.WriteLine("Dcm blobreport");
                txtBoxInfoText.AppendText("Decom Blobs report: " + fileName + "\r\n");

                logReader.ReadDcmBlbRpt(fileName, inRootFolder, outRootFolder);// readDcmBlbRpt
            }
            else if (rBtnDcmLog.Checked)
                ; // readDcmLog
            else if (rBtnDrdFiles.Checked)
            {
                Console.WriteLine("Droid files");
                txtBoxInfoText.AppendText("Droid files.csv: " + fileName + "\r\n");
                logReader.ReadDroidFiles(fileName, rBtnProd.Checked, inRootFolder, outRootFolder);
            }
            else if (rBtnIKAVALog.Checked)
                ; // readIKAVALog
            else if (rBtnIKAVANoConvFiles.Checked)
                ; // readIKAVANoConvFiles
            else if (rBtnVera.Checked)
                ; // readVera
            else if (rBtnArk5Xml.Checked)
                ; // readArk5Xml
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
    }

    public static class Globals
    {
        public static readonly String toolName = "KDRS Analyse";
        public static readonly String toolVersion = "0.1";

        public static int toolCounter = 0;
        public static ExtractionAnalyse extractionAnalyse = new ExtractionAnalyse();
    }
}
