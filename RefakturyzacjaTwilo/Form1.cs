using Allegro_Api;
using Allegro_Api.Models.Order.checkoutform;
using OfficeOpenXml;
using System.Diagnostics;
using System.Text;

namespace RefakturyzacjaTwilo
{
    public partial class Form1 : Form
    {
        List<Libre_API.Book> liberBooks = null;
        List<AteneumAPI.Book> ateneumBooks = null;


        public Form1()
        {
            InitializeComponent();
        }

        private void SetDateTimePicker1To00_00_00()
        {
            // Get the selected date from the DateTimePicker
            DateTime selectedDate = dateTimePicker1.Value.Date;

            // Set the default hour you want (e.g., 00:00:00)
            TimeSpan defaultHour = new TimeSpan(0, 0, 0);

            // Combine the selected date with the default hour to create the new DateTime value
            DateTime newDateTime = selectedDate + defaultHour;

            // Update the DateTimePicker's value with the new DateTime value
            dateTimePicker1.Value = newDateTime;
        }

        private async Task<List<CheckOutForm>?> DownloadOrdersAsync(DateTime input)
        {
            List<CheckOutForm>? Orders = null;

            try
            {
                Orders = await Program._allegroApi.GetOrders(input, OrderStatusType.PICKED_UP);
                Orders.AddRange(await Program._allegroApi.GetOrders(input, OrderStatusType.SENT));
                System.Diagnostics.Debug.WriteLine(Orders.Count);
                return Orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wyst¹pi³ b³¹d: " + ex.Message, "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void GenerateTxt(ref List<CheckOutForm>? Orders, string path)
        {
            StringBuilder content = new StringBuilder();
            foreach (var order in Orders ?? new List<CheckOutForm>())
            {
                foreach (var item in order.lineItems)
                {
                    DateTime intermediary = DateTime.Parse(item.boughtAt);

                    string[] fields = {
                        item.offer.name,
                        item.originalPrice.amount.ToString(),
                        intermediary.ToString(),
                        item.offer.external?.id
                    };

                    content.AppendLine(string.Join("\t", fields)); // adds a new line with variables specified in fields (in that order), insert '\t' between them and '\n' at the end
					// System.Diagnostics.Debug.WriteLine(item.offer.external);
				}
			}
            File.WriteAllText(path, content.ToString());
        }

        private void GenerateXlsx(ref List<CheckOutForm>? Orders, string path, string timestamp)
        {
            // Excel license
            // WARNING: check if the specified license is correct
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // create excel workbook
            using (ExcelPackage excel = new ExcelPackage()) // used with classes implementing IDisposable
            {
                // name of the sheet
                var workSheet = excel.Workbook.Worksheets.Add("Orders" + timestamp);

                // IMPORTANT: const number of columns
                const int NoOfColumns = 4;
                int row = 1;
                foreach (var order in Orders ?? new List<CheckOutForm>())
                {
                    foreach (var item in order.lineItems)
                    {
                        if (item.offer.external != null)
                            if ((bool)(item.offer.external.id.EndsWith("-1")))
                            {
                                //liber

                                var book = liberBooks.Where(bk => bk.ID == item.offer.external.id.Replace("-1", "")).FirstOrDefault();

                               if (book == null)
                                {
                                    workSheet.Cells[row, 5].Value = "Brak mozliwosci uzupe³nienia brutto/netto/vat";

                                }
                                else
                                {
                                    workSheet.Cells[row, 5].Value = book.PriceNettoAferDiscount;
                                    workSheet.Cells[row, 6].Value = book.PriceBruttoAferDiscount;
                                    workSheet.Cells[row, 7].Value = book.Vat;
                                }

                            }
                            else if ((bool)(item.offer.external.id.EndsWith("-2")))
                            {
                                //ateneum

                                var book = ateneumBooks.Where(bk => bk.ident_ate == item.offer.external.id.Replace("-2","")).FirstOrDefault();

                                if (book == null)
                                {
                                    workSheet.Cells[row, 5].Value = "Brak mozliwosci uzupe³nienia brutto/netto/vat";

                                }
                                else
                                {
                                    workSheet.Cells[row, 5].Value = book.PriceWholeSaleBrutto;
                                    workSheet.Cells[row, 6].Value = book.PriceWholeSaleNetto;
                                    workSheet.Cells[row, 7].Value = book.VAT;
                                }
                            }

                        // IMPORTANT: when parsing, DateTime converts dates to timezone of the computer running the app
                        DateTime intermediary = DateTime.Parse(item.boughtAt);
                        string tmp = intermediary.ToString("G");

                        // System.Diagnostics.Debug.WriteLine(intermediary);

                        workSheet.Cells[row, 1].Value = item.offer.name;
                        workSheet.Cells[row, 2].Value = item.originalPrice.amount;
                        workSheet.Cells[row, 3].Value = tmp;
                        workSheet.Cells[row, 4].Value = item.offer.external?.id;
                    }
                    ++row;
                }

                for (int i = 0; i < NoOfColumns; ++i)
                {
                    workSheet.Column(i + 1).AutoFit();
                }

                excel.SaveAs(path);
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
            DateTime temp = new DateTime(this.dateTimePicker1.Value.Year, this.dateTimePicker1.Value.Month, this.dateTimePicker1.Value.Day);
            temp = temp.AddHours(-2);
            DateTime input = new DateTime(temp.Ticks, DateTimeKind.Utc);

            System.Diagnostics.Debug.WriteLine(input.ToString());

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

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            SetDateTimePicker1To00_00_00();
        }

        private async void button2_Click(object sender, EventArgs e)
        {

            System.Diagnostics.Debug.WriteLine("pobieram liber");
            liberBooks = await Program.LibreApi.GetAllBooks(0);
            System.Diagnostics.Debug.WriteLine("pobieram ateneum");
           ateneumBooks = await Program.AteneumApi.GetAllBooksWithMagazin(0);

            System.Diagnostics.Debug.WriteLine(liberBooks.Count);
            System.Diagnostics.Debug.WriteLine(ateneumBooks.Count);
        }
    }
}