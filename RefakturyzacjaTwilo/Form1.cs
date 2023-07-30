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

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 1;
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            DateTime input = new DateTime(this.dateTimePicker1.Value.Ticks, DateTimeKind.Utc);

            //download orders from given date
            List<CheckOutForm> Orders = await Program._allegroApi.GetOrders(input, OrderStatusType.SENT);
            Orders.AddRange(await Program._allegroApi.GetOrders(input, OrderStatusType.PICKED_UP));

            string length = Orders.Count.ToString();
            label2.Text = length;

            string timestamp = DateTime.Now.ToString("--yyyy-MM-dd--HH-mm-ss");
            string path = @"Orders\orders" + timestamp + ".txt";

            string content = string.Empty;
            foreach (var order in Orders)
            {
                foreach (var item in order.lineItems)
                {
                    content += item.offer.name;
                    content += "\t";
                    content += item.originalPrice.amount;
                    content += "\t";
                    content += item.boughtAt;
                    content += "\t";
                    content += item.offer.external?.id;
                    content += '\n';
                }
            }
            System.Diagnostics.Debug.WriteLine(content);

            File.WriteAllText(path, content);

            string dirPath = Path.Combine(Directory.GetCurrentDirectory(), "Orders");
            label3.Visible = true;
            label4.Text = "orders" + timestamp + ".txt";
            label4.Visible = true;
            label5.Visible = true;
            linkLabel1.Text = dirPath;
            linkLabel1.Visible = true;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer.exe", "Orders");
        }
    }
}