namespace TreeGUI
{
    partial class TimerForm
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
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.btExpand = new System.Windows.Forms.Button();
            this.chkIsProject = new System.Windows.Forms.CheckBox();
            this.lbStatistics = new System.Windows.Forms.Label();
            this.chkLockNode = new System.Windows.Forms.CheckBox();
            this.lbNodeLabel = new System.Windows.Forms.Label();
            this.btStart = new System.Windows.Forms.Button();
            this.lbTimeSpent = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.lstExtrusion = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lstPeriods = new System.Windows.Forms.ListBox();
            this.btReport = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btReport);
            this.panel1.Controls.Add(this.btExpand);
            this.panel1.Controls.Add(this.chkIsProject);
            this.panel1.Controls.Add(this.lbStatistics);
            this.panel1.Controls.Add(this.chkLockNode);
            this.panel1.Controls.Add(this.lbNodeLabel);
            this.panel1.Controls.Add(this.btStart);
            this.panel1.Controls.Add(this.lbTimeSpent);
            this.panel1.Location = new System.Drawing.Point(1, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(217, 334);
            this.panel1.TabIndex = 3;
            // 
            // btExpand
            // 
            this.btExpand.Location = new System.Drawing.Point(185, 57);
            this.btExpand.Name = "btExpand";
            this.btExpand.Size = new System.Drawing.Size(29, 23);
            this.btExpand.TabIndex = 9;
            this.btExpand.Text = "<<";
            this.btExpand.UseVisualStyleBackColor = true;
            this.btExpand.Click += new System.EventHandler(this.BtExpand_Click);
            // 
            // chkIsProject
            // 
            this.chkIsProject.AutoSize = true;
            this.chkIsProject.Location = new System.Drawing.Point(94, 70);
            this.chkIsProject.Name = "chkIsProject";
            this.chkIsProject.Size = new System.Drawing.Size(85, 17);
            this.chkIsProject.TabIndex = 8;
            this.chkIsProject.Text = "chkIsProject";
            this.chkIsProject.UseVisualStyleBackColor = true;
            this.chkIsProject.CheckedChanged += new System.EventHandler(this.ChkIsProject_CheckedChanged);
            // 
            // lbStatistics
            // 
            this.lbStatistics.AutoSize = true;
            this.lbStatistics.Location = new System.Drawing.Point(4, 90);
            this.lbStatistics.Name = "lbStatistics";
            this.lbStatistics.Size = new System.Drawing.Size(100, 26);
            this.lbStatistics.TabIndex = 7;
            this.lbStatistics.Text = "Total childs: \nNearest parent proj:";
            // 
            // chkLockNode
            // 
            this.chkLockNode.AutoSize = true;
            this.chkLockNode.Location = new System.Drawing.Point(94, 56);
            this.chkLockNode.Name = "chkLockNode";
            this.chkLockNode.Size = new System.Drawing.Size(50, 17);
            this.chkLockNode.TabIndex = 6;
            this.chkLockNode.Text = "Lock";
            this.chkLockNode.UseVisualStyleBackColor = true;
            this.chkLockNode.CheckedChanged += new System.EventHandler(this.ChkLockNode_CheckedChanged);
            // 
            // lbNodeLabel
            // 
            this.lbNodeLabel.AutoSize = true;
            this.lbNodeLabel.Location = new System.Drawing.Point(4, 4);
            this.lbNodeLabel.Name = "lbNodeLabel";
            this.lbNodeLabel.Size = new System.Drawing.Size(67, 13);
            this.lbNodeLabel.TabIndex = 4;
            this.lbNodeLabel.Text = "lbNodeLabel";
            // 
            // btStart
            // 
            this.btStart.Location = new System.Drawing.Point(7, 57);
            this.btStart.Name = "btStart";
            this.btStart.Size = new System.Drawing.Size(75, 23);
            this.btStart.TabIndex = 3;
            this.btStart.Text = "btStart";
            this.btStart.UseVisualStyleBackColor = true;
            this.btStart.Click += new System.EventHandler(this.Button1_Click);
            // 
            // lbTimeSpent
            // 
            this.lbTimeSpent.AutoSize = true;
            this.lbTimeSpent.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbTimeSpent.Location = new System.Drawing.Point(3, 20);
            this.lbTimeSpent.Name = "lbTimeSpent";
            this.lbTimeSpent.Size = new System.Drawing.Size(86, 31);
            this.lbTimeSpent.TabIndex = 2;
            this.lbTimeSpent.Text = "label1";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.lstExtrusion);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.lstPeriods);
            this.panel2.Location = new System.Drawing.Point(224, 4);
            this.panel2.MinimumSize = new System.Drawing.Size(378, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(378, 334);
            this.panel2.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(191, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Projects extrusion:";
            // 
            // lstExtrusion
            // 
            this.lstExtrusion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstExtrusion.FormattingEnabled = true;
            this.lstExtrusion.Location = new System.Drawing.Point(194, 25);
            this.lstExtrusion.Name = "lstExtrusion";
            this.lstExtrusion.Size = new System.Drawing.Size(181, 303);
            this.lstExtrusion.TabIndex = 9;
            this.lstExtrusion.SelectedIndexChanged += new System.EventHandler(this.LstExtrusion_SelectedIndexChanged);
            this.lstExtrusion.DoubleClick += new System.EventHandler(this.LstExtrusion_DoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Periods:";
            // 
            // lstPeriods
            // 
            this.lstPeriods.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lstPeriods.FormattingEnabled = true;
            this.lstPeriods.Location = new System.Drawing.Point(6, 24);
            this.lstPeriods.Name = "lstPeriods";
            this.lstPeriods.Size = new System.Drawing.Size(182, 303);
            this.lstPeriods.TabIndex = 7;
            // 
            // btReport
            // 
            this.btReport.Location = new System.Drawing.Point(139, 160);
            this.btReport.Name = "btReport";
            this.btReport.Size = new System.Drawing.Size(75, 23);
            this.btReport.TabIndex = 10;
            this.btReport.Text = "btReport";
            this.btReport.UseVisualStyleBackColor = true;
            this.btReport.Click += new System.EventHandler(this.BtReport_Click);
            // 
            // TimerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(603, 340);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.Name = "TimerForm";
            this.ShowInTaskbar = false;
            this.Text = "TimerForm";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TimerForm_FormClosing);
            this.Load += new System.EventHandler(this.TimerForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btStart;
        private System.Windows.Forms.Label lbTimeSpent;
        private System.Windows.Forms.Label lbNodeLabel;
        private System.Windows.Forms.CheckBox chkLockNode;
        private System.Windows.Forms.Label lbStatistics;
        private System.Windows.Forms.CheckBox chkIsProject;
        private System.Windows.Forms.Button btExpand;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lstExtrusion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstPeriods;
        private System.Windows.Forms.Button btReport;
    }
}