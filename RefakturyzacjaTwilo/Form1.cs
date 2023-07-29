using Allegro_Api;
using Allegro_Api.Models.Order;
using Allegro_Api.Models.Order.checkoutform;
using System.Diagnostics;
using System.IO;

namespace RefakturyzacjaTwilo
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private async void button1_Click(object sender, EventArgs e)
		{
			DateTime input = this.dateTimePicker1.Value;
			//DateTime input2 = new DateTime(this.dateTimePicker1.Value.Ticks, DateTimeKind.Utc);

			//download orders from given date
			List<CheckOutForm> Orders = await Program._allegroApi.GetOrders(input, OrderStatusType.SENT);

			string length = Orders.Count.ToString();
			label2.Text = length;


			string path = @"Orders\orders" + DateTime.Now.ToString("--yyyy-MM-dd--HH-mm-ss") + ".txt";

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
			System.Diagnostics.Debug.WriteLine(content);

			File.WriteAllText(path, content);

			string dirPath = Path.Combine(Directory.GetCurrentDirectory(), @"Orders");
			linkLabel1.Text = dirPath;
			linkLabel1.Visible = true;
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("explorer.exe", @"Orders");
		}
	}
}