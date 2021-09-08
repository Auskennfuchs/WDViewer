
namespace WDViewer
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fgfgToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contentFileListView = new System.Windows.Forms.ListBox();
            this.entriesListView = new System.Windows.Forms.ListBox();
            this.previewPanel = new System.Windows.Forms.Panel();
            this.mainMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "WD Files (*.wd)|*.wd| All Files|*.*";
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fgfgToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(8, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(1409, 33);
            this.mainMenuStrip.TabIndex = 0;
            this.mainMenuStrip.Text = "mainMenuStrip";
            // 
            // fgfgToolStripMenuItem
            // 
            this.fgfgToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.fgfgToolStripMenuItem.Name = "fgfgToolStripMenuItem";
            this.fgfgToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
            this.fgfgToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(158, 34);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OnMenuOpenFile);
            // 
            // contentFileListView
            // 
            this.contentFileListView.Dock = System.Windows.Forms.DockStyle.Left;
            this.contentFileListView.FormattingEnabled = true;
            this.contentFileListView.ItemHeight = 25;
            this.contentFileListView.Location = new System.Drawing.Point(8, 33);
            this.contentFileListView.Name = "contentFileListView";
            this.contentFileListView.Size = new System.Drawing.Size(339, 802);
            this.contentFileListView.TabIndex = 2;
            this.contentFileListView.SelectedIndexChanged += new System.EventHandler(this.OnFileListViewSelectedIndexChanged);
            // 
            // entriesListView
            // 
            this.entriesListView.Dock = System.Windows.Forms.DockStyle.Left;
            this.entriesListView.FormattingEnabled = true;
            this.entriesListView.ItemHeight = 25;
            this.entriesListView.Location = new System.Drawing.Point(347, 33);
            this.entriesListView.Name = "entriesListView";
            this.entriesListView.Size = new System.Drawing.Size(339, 802);
            this.entriesListView.TabIndex = 3;
            this.entriesListView.SelectedIndexChanged += new System.EventHandler(this.EntriesListView_SelectedIndexChanged);
            // 
            // previewPanel
            // 
            this.previewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.previewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.previewPanel.Location = new System.Drawing.Point(692, 33);
            this.previewPanel.Name = "previewPanel";
            this.previewPanel.Size = new System.Drawing.Size(725, 802);
            this.previewPanel.TabIndex = 5;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1425, 843);
            this.Controls.Add(this.previewPanel);
            this.Controls.Add(this.entriesListView);
            this.Controls.Add(this.contentFileListView);
            this.Controls.Add(this.mainMenuStrip);
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(8, 0, 8, 8);
            this.Text = "WD Viewer";
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fgfgToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ListBox contentFileListView;
        private System.Windows.Forms.ListBox entriesListView;
        private System.Windows.Forms.Panel previewPanel;
    }
}

