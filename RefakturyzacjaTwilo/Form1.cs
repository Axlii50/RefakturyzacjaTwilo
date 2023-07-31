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
            //List<CheckOutForm> Orders = new List<CheckOutForm>;

			try
			{
				Orders = await Program._allegroApi.GetOrders(input, OrderStatusType.PICKED_UP);
				Orders.AddRange(await Program._allegroApi.GetOrders(input, OrderStatusType.SENT));
                return Orders;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Wyst�pi� b��d: " + ex.Message, "B��d", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
			}
		}

        private void GenerateTxt(ref List<CheckOutForm>? Orders, string path)
        {
			string content = string.Empty;
			foreach (var order in Orders)
			{
				foreach (var item in order.lineItems)
				{
					DateTime intermediary = DateTime.Parse(item.boughtAt);

					content += item.offer.name;
					content += "\t";
					content += item.originalPrice.amount;
					content += "\t";
					content += intermediary;
					content += "\t";
					content += item.offer.external?.id;
					content += '\n';
					System.Diagnostics.Debug.WriteLine(item.offer.external);
				}
			}

            File.WriteAllText(path, content);
		}

        private void GenerateXlsx(ref List<CheckOutForm>? Orders, string path, string timestamp)
        {
			// Excel license
			// WARNING: check if the specified license is correct
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			// create excel workbook
			using (ExcelPackage excel = new ExcelPackage())
			{

				// name of the sheet
				var workSheet = excel.Workbook.Worksheets.Add("Orders" + timestamp);

                // IMPORTANT: const number of columns
				const int NoOfColumns = 4;
				int row = 1;
				foreach (var order in Orders)
				{
					foreach (var item in order.lineItems)
					{
                        // IMPORTANT: when parsing, DateTime converts dates to timezone of the computer running the app
                        DateTime intermediary = DateTime.Parse(item.boughtAt);

                        // System.Diagnostics.Debug.WriteLine(intermediary);

						workSheet.Cells[row, 1].Value = item.offer.name;
						workSheet.Cells[row, 2].Value = item.originalPrice.amount;
						workSheet.Cells[row, 3].Value = item.boughtAt;
						workSheet.Cells[row, 4].Value = item.offer.external?.id;
					}
					++row;
				}

				for (int i = 0; i < NoOfColumns; ++i)
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
                MessageBox.Show("Od dnia " + input + " nie zosta�o z�o�one �adne zam�wienie.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            label2.Text = length;
            label2.Font = new Font("Segoe UI", Form1.DefaultFont.Size, FontStyle.Regular);
            #endregion

            //set date format
            string timestamp = DateTime.Now.ToString("--yyyy-MM-dd--HH-mm-ss");
            string ending = comboBox1.SelectedItem.ToString()!;
            string path = @"Orders\orders" + timestamp + ending;

            // checking file extension and generating corresponding file
            switch (ending)
            {
                case ".txt":
					GenerateTxt(ref Orders, path);
					break;
                case ".xlsx":
                    GenerateXlsx(ref Orders, path, timestamp);
				    break;
			}

            // displaying everything and visual settings
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