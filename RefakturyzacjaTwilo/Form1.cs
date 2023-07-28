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
			List<CheckOutForm> Orders = await Program.AllegroApi.GetOrders(input);

			string length = Orders.Count.ToString();
			label2.Text = length;


			string createText = "test";
			string path = @"Orders\orders.txt";

			string content = string.Empty;
            foreach (var order in Orders)
            {
                foreach (var item in order.lineItems)
                {
					content += item.offer.name;
                }
            }
			System.Diagnostics.Debug.WriteLine(content);

			File.WriteAllText(path, content);
		}
	}
}