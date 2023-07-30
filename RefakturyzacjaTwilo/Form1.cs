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

        private async Task<List<CheckOutForm>?> DownloadOrdersAsync(DateTime input)
        {
			List<CheckOutForm>? Orders = null;

			try
			{
				Orders = await Program._allegroApi.GetOrders(input, OrderStatusType.PICKED_UP);
				Orders.AddRange(await Program._allegroApi.GetOrders(input, OrderStatusType.SENT));
                return Orders;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Wyst¹pi³ b³¹d: " + ex.Message, "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
			}
		}

        private void GenerateXlsx(string path, string content, string timestamp)
        {
			// Excel license
			// WARNING: check if the specified license is correct
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			// create excel workbook
			using (ExcelPackage excel = new ExcelPackage())
			{

				// name of the sheet
				var workSheet = excel.Workbook.Worksheets.Add("Orders" + timestamp);

				string[] lines = content.Split('\n');
				string[]? cols = null;

				for (int i = 0; i < lines.Length - 1; ++i)
				{
					cols = lines[i].Split('\t');
					for (int j = 0; j < cols.Length; ++j)
					{
						workSheet.Cells[i + 1, j + 1].Value = cols[j];
					}
				}

				for (int i = 0; i < cols.Length; ++i)
				{
					workSheet.Column(i + 1).AutoFit();
				}

				excel.SaveAs(path);

				// create excel file on physical disk
				//FileStream objFileStrm = File.Create(path);
				//objFileStrm.Close();
				//// IMPORTANT: write content to excel file
				//File.WriteAllBytes(path, excel.GetAsByteArray());
			}
		}



		private void Form1_Load(object sender, EventArgs e)
        {
            // set default value in drop-down list (comboBox) to ".xslx"
            comboBox1.SelectedIndex = 1;
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            // time format must be in UTC for allegro
            DateTime input = new DateTime(this.dateTimePicker1.Value.Ticks, DateTimeKind.Utc);

			// download orders since given date until now
			// specify that Orders may be null, in case there have been literally no Orders for some time
			List<CheckOutForm>? Orders = await DownloadOrdersAsync(input);

            #region NumberOfOrders
            string length = string.Empty;
            // display number of Orders
            if (Orders is not null)
            {
                length = Orders.Count.ToString();
            }
            else
            {
                MessageBox.Show("Od dnia " + input + " nie zosta³o z³o¿one ¿adne zamówienie.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            label2.Text = length;
            label2.Font = new Font("Segoe UI", Form1.DefaultFont.Size, FontStyle.Regular);
            #endregion

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
                    content += "\t";
                    content += item.originalPrice.amount;
                    content += "\t";
                    // godzina jest o 2 godziny do ty³u, do naprawy albo dodaæ komunikat
                    content += item.boughtAt;
                    content += "\t";
                    content += item.offer.external?.id;
                    content += '\n';
                    System.Diagnostics.Debug.WriteLine(item.offer.external);
                }
            }

            System.Diagnostics.Debug.WriteLine(content); // just for check if authorization works

            // checking file extension and acting on it
            switch (ending)
            {
                case ".txt":
                    File.WriteAllText(path, content);
                    break;
                case ".xlsx":
                    GenerateXlsx(path, content, string timestamp);
				    break;
			}

            string dirPath = Path.Combine(Directory.GetCurrentDirectory(), @"Orders");
            label3.Visible = true;
            label4.Text = "orders" + timestamp + ending;
            label4.Visible = true;
            label5.Visible = true;
            linkLabel1.Text = dirPath;
            linkLabel1.Visible = true;
            button1.Enabled = true;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // open folder Orders (in folder with .exe project file) in Windows Explorer
            Process.Start("explorer.exe", "Orders");
        }
    }
}