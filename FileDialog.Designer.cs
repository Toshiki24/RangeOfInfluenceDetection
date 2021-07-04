namespace RangeOfInfluenceDetection
{
    partial class frmFileDialog
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
            this.txtComparisonPath = new System.Windows.Forms.TextBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnReturn = new System.Windows.Forms.Button();
            this.lbnFileName1 = new System.Windows.Forms.Label();
            this.btnDelete1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtComparisonPath
            // 
            this.txtComparisonPath.Location = new System.Drawing.Point(12, 40);
            this.txtComparisonPath.Name = "txtComparisonPath";
            this.txtComparisonPath.Size = new System.Drawing.Size(317, 19);
            this.txtComparisonPath.TabIndex = 0;
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(335, 38);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 1;
            this.btnOpen.Text = "開く";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(188, 110);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(222, 33);
            this.btnRun.TabIndex = 1;
            this.btnRun.Text = "実行";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnReturn
            // 
            this.btnReturn.Location = new System.Drawing.Point(12, 110);
            this.btnReturn.Name = "btnReturn";
            this.btnReturn.Size = new System.Drawing.Size(136, 33);
            this.btnReturn.TabIndex = 1;
            this.btnReturn.Text = "戻る";
            this.btnReturn.UseVisualStyleBackColor = true;
            this.btnReturn.Click += new System.EventHandler(this.btnReturn_Click);
            // 
            // lbnFileName1
            // 
            this.lbnFileName1.AutoSize = true;
            this.lbnFileName1.Location = new System.Drawing.Point(168, 79);
            this.lbnFileName1.Name = "lbnFileName1";
            this.lbnFileName1.Size = new System.Drawing.Size(103, 12);
            this.lbnFileName1.TabIndex = 2;
            this.lbnFileName1.Text = "選択されたファイル名";
            this.lbnFileName1.Visible = false;
            // 
            // btnDelete1
            // 
            this.btnDelete1.Location = new System.Drawing.Point(298, 74);
            this.btnDelete1.Name = "btnDelete1";
            this.btnDelete1.Size = new System.Drawing.Size(75, 23);
            this.btnDelete1.TabIndex = 1;
            this.btnDelete1.Text = "開く";
            this.btnDelete1.UseVisualStyleBackColor = true;
            this.btnDelete1.Visible = false;
            this.btnDelete1.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // frmFileDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 155);
            this.Controls.Add(this.lbnFileName1);
            this.Controls.Add(this.btnReturn);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.btnDelete1);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.txtComparisonPath);
            this.Name = "frmFileDialog";
            this.Text = "比較ファイル指定";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtComparisonPath;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnReturn;
        private System.Windows.Forms.Label lbnFileName1;
        private System.Windows.Forms.Button btnDelete1;
    }
}