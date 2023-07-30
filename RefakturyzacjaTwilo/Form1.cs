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
			comboBox1.SelectedIndex = 1;
		}
		private async void button1_Click(object sender, EventArgs e)
		{
			DateTime input = this.dateTimePicker1.Value;
			//DateTime input2 = new DateTime(this.dateTimePicker1.Value.Ticks, DateTimeKind.Utc);

			//download orders from given date
			List<CheckOutForm> Orders = await Program._allegroApi.GetOrders(input, OrderStatusType.PICKED_UP);
			//List<CheckOutForm> Orders = await Program._allegroApi.GetOrders(input, OrderStatusType.SENT);

			string length = Orders.Count.ToString();
			label2.Text = length;

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

			string selectedFromDropDownList = comboBox1.SelectedItem.ToString();
			if (selectedFromDropDownList == ".txt")
				File.WriteAllText(path, content);
			else if(selectedFromDropDownList == ".xlsx")
			{
				ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
				ExcelPackage excel = new ExcelPackage();
				var workSheet = excel.Workbook.Worksheets.Add("Orders" + timestamp);

				string[] lines = content.Split('\n');

				for (int i = 0; i < lines.Length - 1; ++i)
				{
					string[] cols = lines[i].Split(';');
					for (int j = 0; j < 4; ++j)
					{
						workSheet.Cells[i+1, j+1].Value = cols[j];
					}
				}

				FileStream objFileStrm = File.Create(path);
				objFileStrm.Close();

				File.WriteAllBytes(path, excel.GetAsByteArray());

				excel.Dispose();
			}

			string dirPath = Path.Combine(Directory.GetCurrentDirectory(), @"Orders");
			label3.Visible = true;
			label4.Text = "orders" + timestamp + comboBox1.SelectedItem.ToString();
			label4.Visible = true;
			label5.Visible = true;
			linkLabel1.Text = dirPath;
			linkLabel1.Visible = true;
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("explorer.exe", @"Orders");
		}
	}
}