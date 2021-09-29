
namespace WDViewer.Controls
{
    partial class MixControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.contentListBox = new System.Windows.Forms.ListBox();
            this.previewPanel = new System.Windows.Forms.Panel();
            this.bntExportAll = new System.Windows.Forms.Button();
            this.exportAllDialog = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // contentListBox
            // 
            this.contentListBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.contentListBox.FormattingEnabled = true;
            this.contentListBox.IntegralHeight = false;
            this.contentListBox.ItemHeight = 25;
            this.contentListBox.Location = new System.Drawing.Point(0, 0);
            this.contentListBox.Name = "contentListBox";
            this.contentListBox.Size = new System.Drawing.Size(327, 636);
            this.contentListBox.TabIndex = 0;
            this.contentListBox.SelectedIndexChanged += new System.EventHandler(this.contentListBox_SelectedIndexChanged);
            // 
            // previewPanel
            // 
            this.previewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.previewPanel.Location = new System.Drawing.Point(327, 0);
            this.previewPanel.Name = "previewPanel";
            this.previewPanel.Size = new System.Drawing.Size(639, 569);
            this.previewPanel.TabIndex = 1;
            // 
            // bntExportAll
            // 
            this.bntExportAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bntExportAll.Location = new System.Drawing.Point(333, 588);
            this.bntExportAll.Name = "bntExportAll";
            this.bntExportAll.Size = new System.Drawing.Size(112, 34);
            this.bntExportAll.TabIndex = 2;
            this.bntExportAll.Text = "Export All";
            this.bntExportAll.UseVisualStyleBackColor = true;
            this.bntExportAll.Click += new System.EventHandler(this.bntExportAll_Click);
            // 
            // exportAllDialog
            // 
            this.exportAllDialog.DefaultExt = "zip";
            this.exportAllDialog.Filter = "Zip Files (*.zip)|*.zip| All Files|*.*";
            // 
            // MixControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bntExportAll);
            this.Controls.Add(this.previewPanel);
            this.Controls.Add(this.contentListBox);
            this.Name = "MixControl";
            this.Size = new System.Drawing.Size(966, 636);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox contentListBox;
        private System.Windows.Forms.Panel previewPanel;
        private System.Windows.Forms.Button bntExportAll;
        private System.Windows.Forms.SaveFileDialog exportAllDialog;
    }
}
