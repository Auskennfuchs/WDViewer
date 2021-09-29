
namespace WDViewer.AssetControls
{
    partial class LevelControl
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
            this.mapPanel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkShowFlags = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mapPanel
            // 
            this.mapPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mapPanel.AutoScroll = true;
            this.mapPanel.Location = new System.Drawing.Point(0, 0);
            this.mapPanel.Name = "mapPanel";
            this.mapPanel.Size = new System.Drawing.Size(1364, 994);
            this.mapPanel.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chkShowFlags);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 996);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1364, 74);
            this.panel1.TabIndex = 1;
            // 
            // chkShowFlags
            // 
            this.chkShowFlags.AutoSize = true;
            this.chkShowFlags.Location = new System.Drawing.Point(3, 22);
            this.chkShowFlags.Name = "chkShowFlags";
            this.chkShowFlags.Size = new System.Drawing.Size(123, 29);
            this.chkShowFlags.TabIndex = 0;
            this.chkShowFlags.Text = "show flags";
            this.chkShowFlags.UseVisualStyleBackColor = true;
            this.chkShowFlags.CheckedChanged += new System.EventHandler(this.chkShowFlags_CheckedChanged);
            // 
            // LevelControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.mapPanel);
            this.Name = "LevelControl";
            this.Size = new System.Drawing.Size(1364, 1070);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mapPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chkShowFlags;
    }
}
