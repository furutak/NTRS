

namespace NTRS
{
    partial class frmNTRS
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNTRS));
            this.txtSubAssy = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtModuleId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOkCount = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSetZero = new System.Windows.Forms.Button();
            this.txtNgCount = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.pnlResult = new System.Windows.Forms.Panel();
            this.btnTodaysCount = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.txtResultDetail = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtSubAssy
            // 
            this.txtSubAssy.Font = new System.Drawing.Font("MS UI Gothic", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtSubAssy.Location = new System.Drawing.Point(114, 37);
            this.txtSubAssy.Name = "txtSubAssy";
            this.txtSubAssy.ReadOnly = true;
            this.txtSubAssy.Size = new System.Drawing.Size(250, 22);
            this.txtSubAssy.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(35, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 14);
            this.label2.TabIndex = 6;
            this.label2.Text = "Sub Assy: ";
            // 
            // txtModuleId
            // 
            this.txtModuleId.Font = new System.Drawing.Font("MS UI Gothic", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtModuleId.Location = new System.Drawing.Point(114, 80);
            this.txtModuleId.Name = "txtModuleId";
            this.txtModuleId.Size = new System.Drawing.Size(250, 22);
            this.txtModuleId.TabIndex = 1;
            this.txtModuleId.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtModuleId_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(35, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 14);
            this.label3.TabIndex = 6;
            this.label3.Text = "Module: ";
            // 
            // txtOkCount
            // 
            this.txtOkCount.Font = new System.Drawing.Font("MS UI Gothic", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtOkCount.Location = new System.Drawing.Point(474, 37);
            this.txtOkCount.Name = "txtOkCount";
            this.txtOkCount.ReadOnly = true;
            this.txtOkCount.Size = new System.Drawing.Size(99, 27);
            this.txtOkCount.TabIndex = 10;
            this.txtOkCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(388, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 14);
            this.label4.TabIndex = 6;
            this.label4.Text = "OK Count: ";
            // 
            // btnSetZero
            // 
            this.btnSetZero.Location = new System.Drawing.Point(599, 80);
            this.btnSetZero.Name = "btnSetZero";
            this.btnSetZero.Size = new System.Drawing.Size(110, 22);
            this.btnSetZero.TabIndex = 3;
            this.btnSetZero.Text = "Set Zero";
            this.btnSetZero.UseVisualStyleBackColor = true;
            this.btnSetZero.Click += new System.EventHandler(this.btnSetZero_Click);
            // 
            // txtNgCount
            // 
            this.txtNgCount.Font = new System.Drawing.Font("MS UI Gothic", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtNgCount.Location = new System.Drawing.Point(474, 80);
            this.txtNgCount.Name = "txtNgCount";
            this.txtNgCount.ReadOnly = true;
            this.txtNgCount.Size = new System.Drawing.Size(99, 27);
            this.txtNgCount.TabIndex = 10;
            this.txtNgCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label5.Location = new System.Drawing.Point(387, 80);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 14);
            this.label5.TabIndex = 6;
            this.label5.Text = "NG Count: ";
            // 
            // pnlResult
            // 
            this.pnlResult.BackColor = System.Drawing.Color.White;
            this.pnlResult.Location = new System.Drawing.Point(193, 142);
            this.pnlResult.Name = "pnlResult";
            this.pnlResult.Size = new System.Drawing.Size(390, 354);
            this.pnlResult.TabIndex = 10;
            // 
            // btnTodaysCount
            // 
            this.btnTodaysCount.Location = new System.Drawing.Point(599, 37);
            this.btnTodaysCount.Name = "btnTodaysCount";
            this.btnTodaysCount.Size = new System.Drawing.Size(110, 22);
            this.btnTodaysCount.TabIndex = 4;
            this.btnTodaysCount.Text = "Show Today\'s";
            this.btnTodaysCount.UseVisualStyleBackColor = true;
            this.btnTodaysCount.Click += new System.EventHandler(this.btnTodaysCount_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("MS UI Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label6.Location = new System.Drawing.Point(121, 142);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 15);
            this.label6.TabIndex = 6;
            this.label6.Text = "Judge:";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(612, 142);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(64, 22);
            this.btnReset.TabIndex = 2;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // txtResultDetail
            // 
            this.txtResultDetail.Font = new System.Drawing.Font("MS UI Gothic", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtResultDetail.Location = new System.Drawing.Point(39, 523);
            this.txtResultDetail.Multiline = true;
            this.txtResultDetail.Name = "txtResultDetail";
            this.txtResultDetail.ReadOnly = true;
            this.txtResultDetail.Size = new System.Drawing.Size(689, 43);
            this.txtResultDetail.TabIndex = 5;
            // 
            // frmNTRS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.LightPink;
            this.ClientSize = new System.Drawing.Size(768, 593);
            this.Controls.Add(this.pnlResult);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtModuleId);
            this.Controls.Add(this.txtResultDetail);
            this.Controls.Add(this.txtSubAssy);
            this.Controls.Add(this.txtNgCount);
            this.Controls.Add(this.txtOkCount);
            this.Controls.Add(this.btnTodaysCount);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnSetZero);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmNTRS";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NTRS: Product - Data Matching";
            this.Load += new System.EventHandler(this.frmModule_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtSubAssy;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtModuleId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtOkCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSetZero;
        private System.Windows.Forms.TextBox txtNgCount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel pnlResult;
        private System.Windows.Forms.Button btnTodaysCount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.TextBox txtResultDetail;
    }
}

