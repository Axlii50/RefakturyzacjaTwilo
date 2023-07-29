namespace RefakturyzacjaTwilo
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
			dateTimePicker1 = new DateTimePicker();
			button1 = new Button();
			label1 = new Label();
			label2 = new Label();
			linkLabel1 = new LinkLabel();
			SuspendLayout();
			// 
			// dateTimePicker1
			// 
			dateTimePicker1.CustomFormat = "";
			dateTimePicker1.Location = new Point(46, 35);
			dateTimePicker1.MinDate = new DateTime(1960, 1, 1, 0, 0, 0, 0);
			dateTimePicker1.Name = "dateTimePicker1";
			dateTimePicker1.Size = new Size(200, 23);
			dateTimePicker1.TabIndex = 0;
			// 
			// button1
			// 
			button1.Location = new Point(377, 35);
			button1.Name = "button1";
			button1.Size = new Size(75, 23);
			button1.TabIndex = 1;
			button1.Text = "Pobierz";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(356, 105);
			label1.Name = "label1";
			label1.Size = new Size(97, 15);
			label1.TabIndex = 2;
			label1.Text = "Liczba zamówień";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point);
			label2.Location = new Point(356, 129);
			label2.Name = "label2";
			label2.Size = new Size(84, 15);
			label2.TabIndex = 3;
			label2.Text = "<placeholder>";
			// 
			// linkLabel1
			// 
			linkLabel1.AutoSize = true;
			linkLabel1.Location = new Point(46, 257);
			linkLabel1.Name = "linkLabel1";
			linkLabel1.Size = new Size(60, 15);
			linkLabel1.TabIndex = 4;
			linkLabel1.TabStop = true;
			linkLabel1.Text = "linkLabel1";
			linkLabel1.Visible = false;
			linkLabel1.LinkClicked += linkLabel1_LinkClicked;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(548, 320);
			Controls.Add(linkLabel1);
			Controls.Add(label2);
			Controls.Add(label1);
			Controls.Add(button1);
			Controls.Add(dateTimePicker1);
			Name = "Form1";
			Text = "Form1";
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private DateTimePicker dateTimePicker1;
		private Button button1;
		private Label label1;
		private Label label2;
		private LinkLabel linkLabel1;
	}
}