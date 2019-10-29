using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KDRS_Analyse
{
    public partial class Form1 : Form
    {
        LogReader logReader = new LogReader();
        XMLWriter writer = new XMLWriter();
        
        string fileName = String.Empty;
        string outFolder = String.Empty;
        string outFileName = "extractionAnalyse.xml";

        public Form1()
        {
            InitializeComponent();
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
                outFolder = txtBoxOutFolder.Text;

                if ("" != txtBoxOutFile.Text)
                    outFileName = txtBoxOutFile.Text;

                string outFile = Path.Combine(outFolder, outFileName);

                ReadFile();
                writer.writeXml(outFile);
            }
        }

        private void ReadFile()
        {
            if (rBtnInfoXml.Checked)
                ;// readInfoXml
            else if (rBtnDcmBlbRpt.Checked)
                logReader.ReadDcmBlbRpt(fileName);// readDcmBlbRpt
            else if (rBtnDcmLog.Checked)
                ; // readDcmLog
            else if (rBtnDrdFiles.Checked)
            {
                if (rBtnArch.Checked)
                    ; // readDrdFilesArch
                else
                    ; // readDrdFilesProd
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
    }

    public static class Globals
    {
        public static int toolCounter;
        public static ExtractionAnalyse extractionAnalyse;
    }
}
