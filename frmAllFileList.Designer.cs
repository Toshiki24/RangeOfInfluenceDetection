namespace RangeOfInfluenceDetection
{
    partial class frmAllFileList
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
            this.btnSln = new System.Windows.Forms.Button();
            this.btnCodeDisplay = new System.Windows.Forms.Button();
            this.lstResult = new System.Windows.Forms.ListBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSerch = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSln
            // 
            this.btnSln.Location = new System.Drawing.Point(12, 380);
            this.btnSln.Name = "btnSln";
            this.btnSln.Size = new System.Drawing.Size(144, 30);
            this.btnSln.TabIndex = 0;
            this.btnSln.Text = "ソリューション選択に戻る";
            this.btnSln.UseVisualStyleBackColor = true;
            this.btnSln.Click += new System.EventHandler(this.btnSln_Click);
            // 
            // btnCodeDisplay
            // 
            this.btnCodeDisplay.Location = new System.Drawing.Point(191, 382);
            this.btnCodeDisplay.Name = "btnCodeDisplay";
            this.btnCodeDisplay.Size = new System.Drawing.Size(144, 62);
            this.btnCodeDisplay.TabIndex = 0;
            this.btnCodeDisplay.Text = "コードを表示";
            this.btnCodeDisplay.UseVisualStyleBackColor = true;
            this.btnCodeDisplay.Click += new System.EventHandler(this.btnCodeDisplay_Click);
            // 
            // lstResult
            // 
            this.lstResult.FormattingEnabled = true;
            this.lstResult.ItemHeight = 12;
            this.lstResult.Location = new System.Drawing.Point(34, 60);
            this.lstResult.Name = "lstResult";
            this.lstResult.Size = new System.Drawing.Size(263, 316);
            this.lstResult.TabIndex = 1;
            this.lstResult.SelectedIndexChanged += new System.EventHandler(this.lstResult_SelectedIndexChanged);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(73, 25);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(224, 19);
            this.txtSearch.TabIndex = 2;
            // 
            // lblSerch
            // 
            this.lblSerch.AutoSize = true;
            this.lblSerch.Location = new System.Drawing.Point(32, 28);
            this.lblSerch.Name = "lblSerch";
            this.lblSerch.Size = new System.Drawing.Size(35, 12);
            this.lblSerch.TabIndex = 3;
            this.lblSerch.Text = "検索：";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(12, 414);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(144, 30);
            this.btnReset.TabIndex = 0;
            this.btnReset.Text = "変更をリセットする";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // frmAllFileList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 450);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.lblSerch);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.lstResult);
            this.Controls.Add(this.btnCodeDisplay);
            this.Controls.Add(this.btnSln);
            this.Name = "frmAllFileList";
            this.Text = "frmAllFileList";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmAllFileList_FormClosing);
            this.Shown += new System.EventHandler(this.frmAllFileList_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSln;
        private System.Windows.Forms.Button btnCodeDisplay;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblSerch;
        public System.Windows.Forms.ListBox lstResult;
        private System.Windows.Forms.Button btnReset;
    }
}