﻿namespace RefakturyzacjaTwilo
{
    partial class Form1
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
            dateTimePicker1=new DateTimePicker();
            button1=new Button();
            label1=new Label();
            label2=new Label();
            linkLabel1=new LinkLabel();
            label3=new Label();
            label4=new Label();
            label5=new Label();
            comboBox1=new ComboBox();
            button2=new Button();
            SuspendLayout();
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Checked=false;
            dateTimePicker1.CustomFormat="yyyy-MM-dd-HH:mm:ss";
            dateTimePicker1.Format=DateTimePickerFormat.Custom;
            dateTimePicker1.Location=new Point(22, 26);
            dateTimePicker1.Margin=new Padding(6, 6, 6, 6);
            dateTimePicker1.MinDate=new DateTime(1960, 1, 1, 0, 0, 0, 0);
            dateTimePicker1.Name="dateTimePicker1";
            dateTimePicker1.Size=new Size(368, 39);
            dateTimePicker1.TabIndex=0;
            dateTimePicker1.Value=new DateTime(2023, 7, 31, 0, 0, 0, 0);
            dateTimePicker1.ValueChanged+=dateTimePicker1_ValueChanged;
            // 
            // button1
            // 
            button1.Location=new Point(607, 87);
            button1.Margin=new Padding(6, 6, 6, 6);
            button1.Name="button1";
            button1.Size=new Size(225, 49);
            button1.TabIndex=1;
            button1.Text="Pobierz";
            button1.UseVisualStyleBackColor=true;
            button1.Click+=button1_Click;
            // 
            // label1
            // 
            label1.AutoSize=true;
            label1.Location=new Point(607, 164);
            label1.Margin=new Padding(6, 0, 6, 0);
            label1.Name="label1";
            label1.Size=new Size(199, 32);
            label1.TabIndex=2;
            label1.Text="Liczba zamówień:";
            // 
            // label2
            // 
            label2.AutoSize=true;
            label2.Font=new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point);
            label2.Location=new Point(804, 164);
            label2.Margin=new Padding(6, 0, 6, 0);
            label2.Name="label2";
            label2.Size=new Size(166, 32);
            label2.TabIndex=3;
            label2.Text="<placeholder>";
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize=true;
            linkLabel1.Location=new Point(22, 226);
            linkLabel1.Margin=new Padding(6, 0, 6, 0);
            linkLabel1.Name="linkLabel1";
            linkLabel1.Size=new Size(121, 32);
            linkLabel1.TabIndex=4;
            linkLabel1.TabStop=true;
            linkLabel1.Text="linkLabel1";
            linkLabel1.Visible=false;
            linkLabel1.LinkClicked+=linkLabel1_LinkClicked;
            // 
            // label3
            // 
            label3.AutoSize=true;
            label3.Location=new Point(22, 105);
            label3.Margin=new Padding(6, 0, 6, 0);
            label3.Name="label3";
            label3.Size=new Size(229, 32);
            label3.TabIndex=5;
            label3.Text="Wygenerowany plik:";
            label3.Visible=false;
            // 
            // label4
            // 
            label4.AutoSize=true;
            label4.Location=new Point(245, 105);
            label4.Margin=new Padding(6, 0, 6, 0);
            label4.Name="label4";
            label4.Size=new Size(78, 32);
            label4.TabIndex=6;
            label4.Text="label4";
            label4.Visible=false;
            // 
            // label5
            // 
            label5.AutoSize=true;
            label5.Location=new Point(22, 164);
            label5.Margin=new Padding(6, 0, 6, 0);
            label5.Name="label5";
            label5.Size=new Size(319, 32);
            label5.TabIndex=7;
            label5.Text="Ścieżka do folderu z plikiem:";
            label5.Visible=false;
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle=ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled=true;
            comboBox1.Items.AddRange(new object[] { ".txt", ".xlsx" });
            comboBox1.Location=new Point(607, 30);
            comboBox1.Margin=new Padding(6, 6, 6, 6);
            comboBox1.Name="comboBox1";
            comboBox1.Size=new Size(221, 40);
            comboBox1.TabIndex=8;
            // 
            // button2
            // 
            button2.Location=new Point(852, 30);
            button2.Name="button2";
            button2.Size=new Size(288, 46);
            button2.TabIndex=9;
            button2.Text="Pobierz HURT";
            button2.UseVisualStyleBackColor=true;
            button2.Click+=button2_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions=new SizeF(13F, 32F);
            AutoScaleMode=AutoScaleMode.Font;
            AutoSize=true;
            ClientSize=new Size(1152, 277);
            Controls.Add(button2);
            Controls.Add(comboBox1);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(linkLabel1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(dateTimePicker1);
            Margin=new Padding(6, 6, 6, 6);
            Name="Form1";
            Text="Form1";
            Load+=Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DateTimePicker dateTimePicker1;
        private Button button1;
        private Label label1;
        private Label label2;
        private LinkLabel linkLabel1;
        private Label label3;
        private Label label4;
        private Label label5;
        private ComboBox comboBox1;
        private Button button2;
    }
}