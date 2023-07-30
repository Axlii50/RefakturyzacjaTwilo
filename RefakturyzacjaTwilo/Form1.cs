using Allegro_Api;
using Allegro_Api.Models.Order;
using Allegro_Api.Models.Order.checkoutform;
using System.Diagnostics;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using static System.Windows.Forms.LinkLabel;

namespace RefakturyzacjaTwilo
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}


		private void Form1_Load(object sender, EventArgs e)
		{
			// set default value in drop-down list (comboBox) to ".xslx"
			comboBox1.SelectedIndex = 1;
		}
		private async void button1_Click(object sender, EventArgs e)
		{
			DateTime input = this.dateTimePicker1.Value;
			//DateTime input2 = new DateTime(this.dateTimePicker1.Value.Ticks, DateTimeKind.Utc);

			//download orders since given date until now
			// specify that Orders may be null, in case there have been literally no Orders for some time
			List<CheckOutForm>? Orders = null;
			try
			{
				Orders = await Program._allegroApi.GetOrders(input, OrderStatusType.PICKED_UP);
				//List<CheckOutForm> Orders = await Program._allegroApi.GetOrders(input, OrderStatusType.SENT);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Wyst¹pi³ b³¹d: " + ex.Message, "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			string length = Orders.Count.ToString();
			label2.Text = length;
			label2.Font = new Font("Segoe UI", Form1.DefaultFont.Size, FontStyle.Regular);

			//set date format
			string timestamp = DateTime.Now.ToString("--yyyy-MM-dd--HH-mm-ss");
			string ending = comboBox1.SelectedItem.ToString();
			string path = @"Orders\orders" + timestamp + ending;

			string content = string.Empty;
			foreach (var order in Orders)
			{
				foreach (var item in order.lineItems)
				{
					content += item.offer.name;
					content += ";";
					content += item.originalPrice;
					content += ";";
					content += item.boughtAt;
					content += ";";
					content += item.offer.external;
					content += '\n';
				}
			}
			System.Diagnostics.Debug.WriteLine(content); // just for check if authorization works

			// checking file extension and acting on it
			if (ending == ".txt")
				File.WriteAllText(path, content);
			else if (ending == ".xlsx")
			{
				// IMPORTANT: excel license
				// WARNING: check if the specified license is correct
				ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
				// create excel workbook
				ExcelPackage excel = new ExcelPackage();
				// name of the sheet
				var workSheet = excel.Workbook.Worksheets.Add("Orders" + timestamp);

				string[] lines = content.Split('\n');

				for (int i = 0; i < lines.Length - 1; ++i)
				{
					string[] cols = lines[i].Split(';');
					for (int j = 0; j < 4; ++j)
					{
						workSheet.Cells[i + 1, j + 1].Value = cols[j];
					}
				}

				// create excel file on physical disk
				FileStream objFileStrm = File.Create(path);
				objFileStrm.Close();

				// IMPORTANT: write content to excel file
				File.WriteAllBytes(path, excel.GetAsByteArray());

				// close Excel package
				excel.Dispose();
			}

			string dirPath = Path.Combine(Directory.GetCurrentDirectory(), @"Orders");
			label3.Visible = true;
			label4.Text = "orders" + timestamp + ending;
			label4.Visible = true;
			label5.Visible = true;
			linkLabel1.Text = dirPath;
			linkLabel1.Visible = true;
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			// open folder Orders (in folder with .exe project file) in Windows Explorer
			Process.Start("explorer.exe", @"Orders");
		}
	}
}