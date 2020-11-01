namespace ProjectZero3_Translation
{
    partial class MainUI
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
            this.linkLabelVHG = new System.Windows.Forms.LinkLabel();
            this.linkLabelGit = new System.Windows.Forms.LinkLabel();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.listFiles = new System.Windows.Forms.ListBox();
            this.btnReimport = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.txtBoxGameLocation = new System.Windows.Forms.TextBox();
            this.labelGameLocation = new System.Windows.Forms.Label();
            this.btnSelectGameLocation = new System.Windows.Forms.Button();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAllToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.importAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // linkLabelVHG
            // 
            this.linkLabelVHG.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelVHG.AutoSize = true;
            this.linkLabelVHG.Location = new System.Drawing.Point(399, 463);
            this.linkLabelVHG.Name = "linkLabelVHG";
            this.linkLabelVHG.Size = new System.Drawing.Size(73, 13);
            this.linkLabelVHG.TabIndex = 25;
            this.linkLabelVHG.TabStop = true;
            this.linkLabelVHG.Text = "VietHoaGame";
            // 
            // linkLabelGit
            // 
            this.linkLabelGit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabelGit.AutoSize = true;
            this.linkLabelGit.Location = new System.Drawing.Point(15, 463);
            this.linkLabelGit.Name = "linkLabelGit";
            this.linkLabelGit.Size = new System.Drawing.Size(38, 13);
            this.linkLabelGit.TabIndex = 24;
            this.linkLabelGit.TabStop = true;
            this.linkLabelGit.Text = "Github";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(15, 437);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(457, 10);
            this.progressBar.TabIndex = 23;
            // 
            // listFiles
            // 
            this.listFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listFiles.ContextMenuStrip = this.contextMenuStrip;
            this.listFiles.FormattingEnabled = true;
            this.listFiles.Location = new System.Drawing.Point(15, 128);
            this.listFiles.Name = "listFiles";
            this.listFiles.Size = new System.Drawing.Size(457, 303);
            this.listFiles.TabIndex = 22;
            this.listFiles.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listFiles_MouseDoubleClick);
            // 
            // btnReimport
            // 
            this.btnReimport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReimport.Location = new System.Drawing.Point(12, 99);
            this.btnReimport.Name = "btnReimport";
            this.btnReimport.Size = new System.Drawing.Size(460, 23);
            this.btnReimport.TabIndex = 21;
            this.btnReimport.Text = "Save";
            this.btnReimport.UseVisualStyleBackColor = true;
            this.btnReimport.Click += new System.EventHandler(this.btnReimport_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpen.Location = new System.Drawing.Point(12, 70);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(460, 23);
            this.btnOpen.TabIndex = 20;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // txtBoxGameLocation
            // 
            this.txtBoxGameLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBoxGameLocation.Location = new System.Drawing.Point(93, 37);
            this.txtBoxGameLocation.Name = "txtBoxGameLocation";
            this.txtBoxGameLocation.ReadOnly = true;
            this.txtBoxGameLocation.Size = new System.Drawing.Size(379, 20);
            this.txtBoxGameLocation.TabIndex = 19;
            // 
            // labelGameLocation
            // 
            this.labelGameLocation.AutoSize = true;
            this.labelGameLocation.Location = new System.Drawing.Point(12, 14);
            this.labelGameLocation.Name = "labelGameLocation";
            this.labelGameLocation.Size = new System.Drawing.Size(79, 13);
            this.labelGameLocation.TabIndex = 18;
            this.labelGameLocation.Text = "Game Location";
            // 
            // btnSelectGameLocation
            // 
            this.btnSelectGameLocation.Location = new System.Drawing.Point(12, 35);
            this.btnSelectGameLocation.Name = "btnSelectGameLocation";
            this.btnSelectGameLocation.Size = new System.Drawing.Size(75, 23);
            this.btnSelectGameLocation.TabIndex = 17;
            this.btnSelectGameLocation.Text = "Select";
            this.btnSelectGameLocation.UseVisualStyleBackColor = true;
            this.btnSelectGameLocation.Click += new System.EventHandler(this.btnSelectGameLocation_Click);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToolStripMenuItem,
            this.importToolStripMenuItem,
            this.exportAllToolStripMenuItem1,
            this.importAllToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(181, 114);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.importToolStripMenuItem.Text = "Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // exportAllToolStripMenuItem1
            // 
            this.exportAllToolStripMenuItem1.Name = "exportAllToolStripMenuItem1";
            this.exportAllToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.exportAllToolStripMenuItem1.Text = "Export All";
            this.exportAllToolStripMenuItem1.Click += new System.EventHandler(this.exportAllToolStripMenuItem1_Click);
            // 
            // importAllToolStripMenuItem
            // 
            this.importAllToolStripMenuItem.Name = "importAllToolStripMenuItem";
            this.importAllToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.importAllToolStripMenuItem.Text = "Import All";
            this.importAllToolStripMenuItem.Click += new System.EventHandler(this.importAllToolStripMenuItem_Click);
            // 
            // MainUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 491);
            this.Controls.Add(this.linkLabelVHG);
            this.Controls.Add(this.linkLabelGit);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.listFiles);
            this.Controls.Add(this.btnReimport);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.txtBoxGameLocation);
            this.Controls.Add(this.labelGameLocation);
            this.Controls.Add(this.btnSelectGameLocation);
            this.Name = "MainUI";
            this.Text = "Project Zero 3 Translation";
            this.Load += new System.EventHandler(this.MainUI_Load);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLabelVHG;
        private System.Windows.Forms.LinkLabel linkLabelGit;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ListBox listFiles;
        private System.Windows.Forms.Button btnReimport;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.TextBox txtBoxGameLocation;
        private System.Windows.Forms.Label labelGameLocation;
        private System.Windows.Forms.Button btnSelectGameLocation;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAllToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem importAllToolStripMenuItem;
    }
}

