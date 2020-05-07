namespace KDRS_Analyse
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rBtnInfoXml = new System.Windows.Forms.RadioButton();
            this.rBtnDcmBlbRpt = new System.Windows.Forms.RadioButton();
            this.rBtnDcmLog = new System.Windows.Forms.RadioButton();
            this.rBtnDrdFiles = new System.Windows.Forms.RadioButton();
            this.rBtnIKAVALog = new System.Windows.Forms.RadioButton();
            this.rBtnIKAVAConvFiles = new System.Windows.Forms.RadioButton();
            this.rBtnIKAVANoConvFiles = new System.Windows.Forms.RadioButton();
            this.rBtnVera = new System.Windows.Forms.RadioButton();
            this.rBtnKOSTVal = new System.Windows.Forms.RadioButton();
            this.rBtnAnalyseXML = new System.Windows.Forms.RadioButton();
            this.rBtnProd = new System.Windows.Forms.RadioButton();
            this.rBtnArch = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBoxInfoText = new System.Windows.Forms.TextBox();
            this.txtBoxOutFolder = new System.Windows.Forms.TextBox();
            this.txtBoxOutFile = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtBoxInRoot = new System.Windows.Forms.TextBox();
            this.txtBoxOutRoot = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnWriteXml = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.lblProgress = new System.Windows.Forms.Label();
            this.chkBoxIncXsd = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnSaveLog = new System.Windows.Forms.Button();
            this.btnSaveInput = new System.Windows.Forms.Button();
            this.btnLoadInput = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rBtnInfoXml
            // 
            this.rBtnInfoXml.AutoSize = true;
            this.rBtnInfoXml.Location = new System.Drawing.Point(12, 34);
            this.rBtnInfoXml.Name = "rBtnInfoXml";
            this.rBtnInfoXml.Size = new System.Drawing.Size(60, 17);
            this.rBtnInfoXml.TabIndex = 0;
            this.rBtnInfoXml.TabStop = true;
            this.rBtnInfoXml.Text = "info.xml";
            this.rBtnInfoXml.UseVisualStyleBackColor = true;
            // 
            // rBtnDcmBlbRpt
            // 
            this.rBtnDcmBlbRpt.AutoSize = true;
            this.rBtnDcmBlbRpt.Checked = true;
            this.rBtnDcmBlbRpt.Location = new System.Drawing.Point(12, 57);
            this.rBtnDcmBlbRpt.Name = "rBtnDcmBlbRpt";
            this.rBtnDcmBlbRpt.Size = new System.Drawing.Size(152, 17);
            this.rBtnDcmBlbRpt.TabIndex = 1;
            this.rBtnDcmBlbRpt.TabStop = true;
            this.rBtnDcmBlbRpt.Text = "Decom project blobs report";
            this.rBtnDcmBlbRpt.UseVisualStyleBackColor = true;
            // 
            // rBtnDcmLog
            // 
            this.rBtnDcmLog.AutoSize = true;
            this.rBtnDcmLog.Location = new System.Drawing.Point(12, 80);
            this.rBtnDcmLog.Name = "rBtnDcmLog";
            this.rBtnDcmLog.Size = new System.Drawing.Size(92, 17);
            this.rBtnDcmLog.TabIndex = 2;
            this.rBtnDcmLog.TabStop = true;
            this.rBtnDcmLog.Text = "Decom full log";
            this.rBtnDcmLog.UseVisualStyleBackColor = true;
            // 
            // rBtnDrdFiles
            // 
            this.rBtnDrdFiles.AutoSize = true;
            this.rBtnDrdFiles.Location = new System.Drawing.Point(12, 103);
            this.rBtnDrdFiles.Name = "rBtnDrdFiles";
            this.rBtnDrdFiles.Size = new System.Drawing.Size(81, 17);
            this.rBtnDrdFiles.TabIndex = 3;
            this.rBtnDrdFiles.TabStop = true;
            this.rBtnDrdFiles.Text = "DROID files";
            this.rBtnDrdFiles.UseVisualStyleBackColor = true;
            // 
            // rBtnIKAVALog
            // 
            this.rBtnIKAVALog.AutoSize = true;
            this.rBtnIKAVALog.Location = new System.Drawing.Point(12, 173);
            this.rBtnIKAVALog.Name = "rBtnIKAVALog";
            this.rBtnIKAVALog.Size = new System.Drawing.Size(73, 17);
            this.rBtnIKAVALog.TabIndex = 4;
            this.rBtnIKAVALog.Text = "IKAVA log";
            this.rBtnIKAVALog.UseVisualStyleBackColor = true;
            this.rBtnIKAVALog.Visible = false;
            // 
            // rBtnIKAVAConvFiles
            // 
            this.rBtnIKAVAConvFiles.AutoSize = true;
            this.rBtnIKAVAConvFiles.Location = new System.Drawing.Point(12, 196);
            this.rBtnIKAVAConvFiles.Name = "rBtnIKAVAConvFiles";
            this.rBtnIKAVAConvFiles.Size = new System.Drawing.Size(128, 17);
            this.rBtnIKAVAConvFiles.TabIndex = 5;
            this.rBtnIKAVAConvFiles.Text = "IKAVA converted files";
            this.rBtnIKAVAConvFiles.UseVisualStyleBackColor = true;
            this.rBtnIKAVAConvFiles.Visible = false;
            // 
            // rBtnIKAVANoConvFiles
            // 
            this.rBtnIKAVANoConvFiles.AutoSize = true;
            this.rBtnIKAVANoConvFiles.Location = new System.Drawing.Point(12, 218);
            this.rBtnIKAVANoConvFiles.Name = "rBtnIKAVANoConvFiles";
            this.rBtnIKAVANoConvFiles.Size = new System.Drawing.Size(146, 17);
            this.rBtnIKAVANoConvFiles.TabIndex = 6;
            this.rBtnIKAVANoConvFiles.Text = "IKAVA files not converted";
            this.rBtnIKAVANoConvFiles.UseVisualStyleBackColor = true;
            this.rBtnIKAVANoConvFiles.Visible = false;
            // 
            // rBtnVera
            // 
            this.rBtnVera.AutoSize = true;
            this.rBtnVera.Location = new System.Drawing.Point(12, 126);
            this.rBtnVera.Name = "rBtnVera";
            this.rBtnVera.Size = new System.Drawing.Size(67, 17);
            this.rBtnVera.TabIndex = 7;
            this.rBtnVera.TabStop = true;
            this.rBtnVera.Text = "veraPDF";
            this.rBtnVera.UseVisualStyleBackColor = true;
            // 
            // rBtnKOSTVal
            // 
            this.rBtnKOSTVal.AutoSize = true;
            this.rBtnKOSTVal.Location = new System.Drawing.Point(12, 149);
            this.rBtnKOSTVal.Name = "rBtnKOSTVal";
            this.rBtnKOSTVal.Size = new System.Drawing.Size(72, 17);
            this.rBtnKOSTVal.TabIndex = 8;
            this.rBtnKOSTVal.TabStop = true;
            this.rBtnKOSTVal.Text = "KOST-Val";
            this.rBtnKOSTVal.UseVisualStyleBackColor = true;
            // 
            // rBtnAnalyseXML
            // 
            this.rBtnAnalyseXML.AutoSize = true;
            this.rBtnAnalyseXML.Location = new System.Drawing.Point(12, 241);
            this.rBtnAnalyseXML.Name = "rBtnAnalyseXML";
            this.rBtnAnalyseXML.Size = new System.Drawing.Size(87, 17);
            this.rBtnAnalyseXML.TabIndex = 9;
            this.rBtnAnalyseXML.TabStop = true;
            this.rBtnAnalyseXML.Text = "Analyse XML";
            this.rBtnAnalyseXML.UseVisualStyleBackColor = true;
            // 
            // rBtnProd
            // 
            this.rBtnProd.AutoSize = true;
            this.rBtnProd.Checked = true;
            this.rBtnProd.Location = new System.Drawing.Point(3, 27);
            this.rBtnProd.Name = "rBtnProd";
            this.rBtnProd.Size = new System.Drawing.Size(94, 17);
            this.rBtnProd.TabIndex = 0;
            this.rBtnProd.TabStop = true;
            this.rBtnProd.Text = "Production (In)";
            this.rBtnProd.UseVisualStyleBackColor = true;
            // 
            // rBtnArch
            // 
            this.rBtnArch.AutoSize = true;
            this.rBtnArch.Location = new System.Drawing.Point(3, 50);
            this.rBtnArch.Name = "rBtnArch";
            this.rBtnArch.Size = new System.Drawing.Size(89, 17);
            this.rBtnArch.TabIndex = 1;
            this.rBtnArch.TabStop = true;
            this.rBtnArch.Text = "Archival (Out)";
            this.rBtnArch.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Input type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "File type";
            // 
            // txtBoxInfoText
            // 
            this.txtBoxInfoText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBoxInfoText.Location = new System.Drawing.Point(304, 18);
            this.txtBoxInfoText.Multiline = true;
            this.txtBoxInfoText.Name = "txtBoxInfoText";
            this.txtBoxInfoText.ReadOnly = true;
            this.txtBoxInfoText.Size = new System.Drawing.Size(488, 136);
            this.txtBoxInfoText.TabIndex = 22;
            this.txtBoxInfoText.TabStop = false;
            // 
            // txtBoxOutFolder
            // 
            this.txtBoxOutFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBoxOutFolder.Location = new System.Drawing.Point(157, 464);
            this.txtBoxOutFolder.Name = "txtBoxOutFolder";
            this.txtBoxOutFolder.Size = new System.Drawing.Size(635, 20);
            this.txtBoxOutFolder.TabIndex = 14;
            // 
            // txtBoxOutFile
            // 
            this.txtBoxOutFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBoxOutFile.Location = new System.Drawing.Point(157, 490);
            this.txtBoxOutFile.Name = "txtBoxOutFile";
            this.txtBoxOutFile.Size = new System.Drawing.Size(635, 20);
            this.txtBoxOutFile.TabIndex = 15;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rBtnProd);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.rBtnArch);
            this.panel1.Location = new System.Drawing.Point(12, 265);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(188, 80);
            this.panel1.TabIndex = 10;
            // 
            // txtBoxInRoot
            // 
            this.txtBoxInRoot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBoxInRoot.Location = new System.Drawing.Point(157, 412);
            this.txtBoxInRoot.Name = "txtBoxInRoot";
            this.txtBoxInRoot.Size = new System.Drawing.Size(635, 20);
            this.txtBoxInRoot.TabIndex = 12;
            // 
            // txtBoxOutRoot
            // 
            this.txtBoxOutRoot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBoxOutRoot.Location = new System.Drawing.Point(157, 438);
            this.txtBoxOutRoot.Name = "txtBoxOutRoot";
            this.txtBoxOutRoot.Size = new System.Drawing.Size(635, 20);
            this.txtBoxOutRoot.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 412);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Input root folder";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 438);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "Output root folder";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 467);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Analyse folder";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 490);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 13);
            this.label6.TabIndex = 25;
            this.label6.Text = "Analyse filename";
            // 
            // btnWriteXml
            // 
            this.btnWriteXml.Location = new System.Drawing.Point(646, 172);
            this.btnWriteXml.Name = "btnWriteXml";
            this.btnWriteXml.Size = new System.Drawing.Size(142, 80);
            this.btnWriteXml.TabIndex = 16;
            this.btnWriteXml.Text = "Create result file";
            this.btnWriteXml.UseVisualStyleBackColor = true;
            this.btnWriteXml.Click += new System.EventHandler(this.btnWriteXml_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(646, 265);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(142, 44);
            this.btnReset.TabIndex = 17;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblProgress.Location = new System.Drawing.Point(382, 172);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(2, 15);
            this.lblProgress.TabIndex = 28;
            // 
            // chkBoxIncXsd
            // 
            this.chkBoxIncXsd.AutoSize = true;
            this.chkBoxIncXsd.Location = new System.Drawing.Point(207, 265);
            this.chkBoxIncXsd.Name = "chkBoxIncXsd";
            this.chkBoxIncXsd.Size = new System.Drawing.Size(129, 17);
            this.chkBoxIncXsd.TabIndex = 11;
            this.chkBoxIncXsd.Text = "Include table.xsd/.xml";
            this.chkBoxIncXsd.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(304, 173);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 13);
            this.label7.TabIndex = 30;
            this.label7.Text = "Files handled:";
            // 
            // btnSaveLog
            // 
            this.btnSaveLog.Location = new System.Drawing.Point(646, 315);
            this.btnSaveLog.Name = "btnSaveLog";
            this.btnSaveLog.Size = new System.Drawing.Size(142, 23);
            this.btnSaveLog.TabIndex = 18;
            this.btnSaveLog.Text = "Save LOG";
            this.btnSaveLog.UseVisualStyleBackColor = true;
            this.btnSaveLog.Click += new System.EventHandler(this.btnSaveLog_Click);
            // 
            // btnSaveInput
            // 
            this.btnSaveInput.Location = new System.Drawing.Point(646, 344);
            this.btnSaveInput.Name = "btnSaveInput";
            this.btnSaveInput.Size = new System.Drawing.Size(142, 23);
            this.btnSaveInput.TabIndex = 19;
            this.btnSaveInput.Text = "Save input";
            this.btnSaveInput.UseVisualStyleBackColor = true;
            this.btnSaveInput.Click += new System.EventHandler(this.btnSaveInput_Click);
            // 
            // btnLoadInput
            // 
            this.btnLoadInput.Location = new System.Drawing.Point(646, 373);
            this.btnLoadInput.Name = "btnLoadInput";
            this.btnLoadInput.Size = new System.Drawing.Size(142, 23);
            this.btnLoadInput.TabIndex = 20;
            this.btnLoadInput.Text = "Load input";
            this.btnLoadInput.UseVisualStyleBackColor = true;
            this.btnLoadInput.Click += new System.EventHandler(this.btnLoadInput_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 539);
            this.Controls.Add(this.btnLoadInput);
            this.Controls.Add(this.btnSaveInput);
            this.Controls.Add(this.btnSaveLog);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.chkBoxIncXsd);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnWriteXml);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtBoxInRoot);
            this.Controls.Add(this.txtBoxOutRoot);
            this.Controls.Add(this.txtBoxOutFile);
            this.Controls.Add(this.txtBoxOutFolder);
            this.Controls.Add(this.txtBoxInfoText);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rBtnAnalyseXML);
            this.Controls.Add(this.rBtnKOSTVal);
            this.Controls.Add(this.rBtnVera);
            this.Controls.Add(this.rBtnIKAVANoConvFiles);
            this.Controls.Add(this.rBtnIKAVAConvFiles);
            this.Controls.Add(this.rBtnIKAVALog);
            this.Controls.Add(this.rBtnDrdFiles);
            this.Controls.Add(this.rBtnDcmLog);
            this.Controls.Add(this.rBtnDcmBlbRpt);
            this.Controls.Add(this.rBtnInfoXml);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rBtnInfoXml;
        private System.Windows.Forms.RadioButton rBtnDcmBlbRpt;
        private System.Windows.Forms.RadioButton rBtnDcmLog;
        private System.Windows.Forms.RadioButton rBtnDrdFiles;
        private System.Windows.Forms.RadioButton rBtnIKAVALog;
        private System.Windows.Forms.RadioButton rBtnIKAVAConvFiles;
        private System.Windows.Forms.RadioButton rBtnIKAVANoConvFiles;
        private System.Windows.Forms.RadioButton rBtnVera;
        private System.Windows.Forms.RadioButton rBtnKOSTVal;
        private System.Windows.Forms.RadioButton rBtnAnalyseXML;
        private System.Windows.Forms.RadioButton rBtnProd;
        private System.Windows.Forms.RadioButton rBtnArch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBoxInfoText;
        private System.Windows.Forms.TextBox txtBoxOutFolder;
        private System.Windows.Forms.TextBox txtBoxOutFile;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtBoxInRoot;
        private System.Windows.Forms.TextBox txtBoxOutRoot;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnWriteXml;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.CheckBox chkBoxIncXsd;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnSaveLog;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Button btnSaveInput;
        private System.Windows.Forms.Button btnLoadInput;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}

