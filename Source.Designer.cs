namespace RangeOfInfluenceDetection
{
    partial class Source
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
            this.ctmSearch = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmsSeach = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPre = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmFileSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.txtSource = new System.Windows.Forms.RichTextBox();
            this.ctmSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // ctmSearch
            // 
            this.ctmSearch.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmsSeach,
            this.cmsPre,
            this.tsmFileSelect});
            this.ctmSearch.Name = "ctmSearch";
            this.ctmSearch.Size = new System.Drawing.Size(204, 70);
            // 
            // cmsSeach
            // 
            this.cmsSeach.Name = "cmsSeach";
            this.cmsSeach.Size = new System.Drawing.Size(203, 22);
            this.cmsSeach.Text = "影響範囲を検索";
            this.cmsSeach.Click += new System.EventHandler(this.cmsSeach_Click);
            // 
            // cmsPre
            // 
            this.cmsPre.Name = "cmsPre";
            this.cmsPre.Size = new System.Drawing.Size(203, 22);
            this.cmsPre.Text = "ファイル一覧に戻る";
            this.cmsPre.Click += new System.EventHandler(this.cmsPre_Click);
            // 
            // tsmFileSelect
            // 
            this.tsmFileSelect.Name = "tsmFileSelect";
            this.tsmFileSelect.Size = new System.Drawing.Size(203, 22);
            this.tsmFileSelect.Text = "比較するファイルを選択する";
            this.tsmFileSelect.Click += new System.EventHandler(this.tsmFileSelect_Click);
            // 
            // txtSource
            // 
            this.txtSource.ContextMenuStrip = this.ctmSearch;
            this.txtSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSource.Location = new System.Drawing.Point(0, 0);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(800, 450);
            this.txtSource.TabIndex = 2;
            this.txtSource.Text = "";
            // 
            // Source
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtSource);
            this.Name = "Source";
            this.Text = "Source";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Source_FormClosing);
            this.ctmSearch.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip ctmSearch;
        private System.Windows.Forms.RichTextBox txtSource;
        private System.Windows.Forms.ToolStripMenuItem cmsSeach;
        private System.Windows.Forms.ToolStripMenuItem cmsPre;
        private System.Windows.Forms.ToolStripMenuItem tsmFileSelect;
    }
}