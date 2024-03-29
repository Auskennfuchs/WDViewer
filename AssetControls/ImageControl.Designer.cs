﻿
using System.Windows.Forms;

namespace WDViewer.Controls
{
    partial class ImageControl : UserControl
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.curColPanel = new System.Windows.Forms.Panel();
            this.lblCurColor = new System.Windows.Forms.Label();
            this.paletteControl = new WDViewer.Controls.PaletteControl();
            this.btnExport = new System.Windows.Forms.Button();
            this.exportFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.assetImageControl = new WDViewer.Controls.AssetImageControl();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.assetImageControl)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.curColPanel);
            this.panel1.Controls.Add(this.lblCurColor);
            this.panel1.Controls.Add(this.paletteControl);
            this.panel1.Controls.Add(this.btnExport);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 757);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1053, 83);
            this.panel1.TabIndex = 1;
            // 
            // curColPanel
            // 
            this.curColPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.curColPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.curColPanel.Location = new System.Drawing.Point(831, 45);
            this.curColPanel.Name = "curColPanel";
            this.curColPanel.Size = new System.Drawing.Size(27, 30);
            this.curColPanel.TabIndex = 3;
            // 
            // lblCurColor
            // 
            this.lblCurColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurColor.AutoSize = true;
            this.lblCurColor.Location = new System.Drawing.Point(864, 45);
            this.lblCurColor.Name = "lblCurColor";
            this.lblCurColor.Size = new System.Drawing.Size(79, 25);
            this.lblCurColor.TabIndex = 2;
            this.lblCurColor.Text = "curColor";
            // 
            // paletteControl
            // 
            this.paletteControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.paletteControl.Location = new System.Drawing.Point(0, 3);
            this.paletteControl.Name = "paletteControl";
            this.paletteControl.Palette = null;
            this.paletteControl.Size = new System.Drawing.Size(858, 77);
            this.paletteControl.TabIndex = 1;
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.Location = new System.Drawing.Point(941, 9);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(112, 34);
            this.btnExport.TabIndex = 0;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // exportFileDialog
            // 
            this.exportFileDialog.Filter = "PNG Files (*.png)|*.png| All Files|*.*";
            // 
            // assetImageControl
            // 
            this.assetImageControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.assetImageControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.assetImageControl.Location = new System.Drawing.Point(3, 3);
            this.assetImageControl.Name = "assetImageControl";
            this.assetImageControl.Size = new System.Drawing.Size(1047, 748);
            this.assetImageControl.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.assetImageControl.TabIndex = 2;
            this.assetImageControl.TabStop = false;
            this.assetImageControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.assetImageControl_MouseMove);
            // 
            // ImageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.assetImageControl);
            this.Controls.Add(this.panel1);
            this.Name = "ImageControl";
            this.Size = new System.Drawing.Size(1053, 840);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.assetImageControl)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnExport;
        private PaletteControl paletteControl;
        private System.Windows.Forms.SaveFileDialog exportFileDialog;
        private Label lblCurColor;
        private Panel curColPanel;
        private AssetImageControl assetImageControl;
    }
}
