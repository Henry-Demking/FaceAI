
namespace FaceAI
{
    partial class EditUser
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
            this.cboField = new System.Windows.Forms.ComboBox();
            this.Field = new System.Windows.Forms.Label();
            this.lbl1 = new System.Windows.Forms.Label();
            this.lstItems = new System.Windows.Forms.ListBox();
            this.lstSelectedItems = new System.Windows.Forms.ListBox();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cboField
            // 
            this.cboField.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.HistoryList;
            this.cboField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboField.FormattingEnabled = true;
            this.cboField.Items.AddRange(new object[] {
            "Academia",
            "Computer Science",
            "Information Technology",
            "Software Development"});
            this.cboField.Location = new System.Drawing.Point(118, 21);
            this.cboField.Name = "cboField";
            this.cboField.Size = new System.Drawing.Size(159, 23);
            this.cboField.Sorted = true;
            this.cboField.TabIndex = 0;
            // 
            // Field
            // 
            this.Field.AutoSize = true;
            this.Field.Location = new System.Drawing.Point(12, 21);
            this.Field.Name = "Field";
            this.Field.Size = new System.Drawing.Size(32, 15);
            this.Field.TabIndex = 1;
            this.Field.Text = "Field";
            // 
            // lbl1
            // 
            this.lbl1.AutoSize = true;
            this.lbl1.Location = new System.Drawing.Point(70, 59);
            this.lbl1.Name = "lbl1";
            this.lbl1.Size = new System.Drawing.Size(141, 15);
            this.lbl1.TabIndex = 3;
            this.lbl1.Text = "Programming Languages";
            // 
            // lstItems
            // 
            this.lstItems.FormattingEnabled = true;
            this.lstItems.ItemHeight = 15;
            this.lstItems.Items.AddRange(new object[] {
            "Ada",
            "C",
            "C#",
            "C++",
            "Cobol",
            "F#",
            "Java",
            "Java Script",
            "Python"});
            this.lstItems.Location = new System.Drawing.Point(6, 77);
            this.lstItems.Name = "lstItems";
            this.lstItems.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstItems.Size = new System.Drawing.Size(120, 94);
            this.lstItems.Sorted = true;
            this.lstItems.TabIndex = 4;
            // 
            // lstSelectedItems
            // 
            this.lstSelectedItems.FormattingEnabled = true;
            this.lstSelectedItems.ItemHeight = 15;
            this.lstSelectedItems.Location = new System.Drawing.Point(157, 77);
            this.lstSelectedItems.Name = "lstSelectedItems";
            this.lstSelectedItems.Size = new System.Drawing.Size(120, 94);
            this.lstSelectedItems.Sorted = true;
            this.lstSelectedItems.TabIndex = 5;
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(132, 92);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(19, 23);
            this.btnLeft.TabIndex = 6;
            this.btnLeft.Text = ">";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(132, 132);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(19, 23);
            this.btnRemove.TabIndex = 7;
            this.btnRemove.Text = "<";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // EditUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(285, 456);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.lstSelectedItems);
            this.Controls.Add(this.lstItems);
            this.Controls.Add(this.lbl1);
            this.Controls.Add(this.Field);
            this.Controls.Add(this.cboField);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditUser";
            this.Text = "Edit User";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cboField;
        private System.Windows.Forms.Label Field;
        private System.Windows.Forms.Label lbl1;
        private System.Windows.Forms.ListBox lstItems;
        private System.Windows.Forms.ListBox lstSelectedItems;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnRemove;
    }
}

