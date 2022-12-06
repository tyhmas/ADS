namespace ADSCloud.Views
{
    partial class ReplaceRow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReplaceRow));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cancel = new System.Windows.Forms.Button();
            this.newRowNumber = new System.Windows.Forms.TextBox();
            this.oldRowNumber = new System.Windows.Forms.TextBox();
            this.OK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.groupBox1.Controls.Add(this.cancel);
            this.groupBox1.Controls.Add(this.newRowNumber);
            this.groupBox1.Controls.Add(this.oldRowNumber);
            this.groupBox1.Controls.Add(this.OK);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(441, 293);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(71, 229);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(139, 41);
            this.cancel.TabIndex = 7;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // newRowNumber
            // 
            this.newRowNumber.Location = new System.Drawing.Point(298, 138);
            this.newRowNumber.Name = "newRowNumber";
            this.newRowNumber.Size = new System.Drawing.Size(122, 38);
            this.newRowNumber.TabIndex = 5;
            this.newRowNumber.TextChanged += new System.EventHandler(this.passwordtext_TextChanged);
            // 
            // oldRowNumber
            // 
            this.oldRowNumber.Location = new System.Drawing.Point(298, 64);
            this.oldRowNumber.Name = "oldRowNumber";
            this.oldRowNumber.Size = new System.Drawing.Size(122, 38);
            this.oldRowNumber.TabIndex = 4;
            this.oldRowNumber.TextChanged += new System.EventHandler(this.usertext_TextChanged);
            // 
            // OK
            // 
            this.OK.Enabled = false;
            this.OK.Location = new System.Drawing.Point(216, 229);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(139, 41);
            this.OK.TabIndex = 3;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 60);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(199, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Old Row Number";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 144);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(209, 32);
            this.label2.TabIndex = 1;
            this.label2.Text = "New Row Number";
            // 
            // ReplaceRow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(464, 307);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 13.8F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.MaximizeBox = false;
            this.Name = "ReplaceRow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Replace Row";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.TextBox newRowNumber;
        private System.Windows.Forms.TextBox oldRowNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}