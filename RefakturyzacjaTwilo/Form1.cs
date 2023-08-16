using Allegro_Api;
using Allegro_Api.Models.Order.checkoutform;
using OfficeOpenXml;
using OfficeOpenXml.Table;
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

		/// <summary>
		/// Gets the offset in hours for converting UTC to Central European Time (CET) or Central European Summer Time (CEST) based on the DST status.
		/// </summary>
		/// <returns>Returns -1 (winter time) or -2 (summer time) which is the offset in hours</returns>
		private int GetCentralEuropeanTimeOffset()
		{
			var zone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
			var centralTime = DateTime.UtcNow;

			if (zone.IsDaylightSavingTime(centralTime))
			{
				return -2;
			}
			else
			{
				return -1;
			}
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
				
			}
			catch (Exception ex)
			{
				MessageBox.Show("Wyst¹pi³ b³¹d przy pobieraniu zamówieñ z Allegro", "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return null;
			}

			
            //previously there were display glitches
            //this ensures that specific code is executed on the main thread (which solves problem which UI in asynch method since it delegates execution of some code to main thread)

            label2.Invoke(() =>
            {
                label2.Text = Orders?.Count.ToString();
                label2.Font = new Font("Segoe UI", Form1.DefaultFont.Size, FontStyle.Regular);
            });
			
			return Orders;
        }

		[Obsolete("New updates are done only for GenerateXlsx(), it is advised to give up on .txt as a whole in the project and focus on .xslx")]
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

		/// <summary>
		/// Sets column headers in tables of .xlsx. Please, make sure to adjust variable NoOfColumns in correspondence with this function.
		/// </summary>
		/// <param name="listOfSheets"></param>
		private void SetColumnHeadersInXlsx(ref List<OfficeOpenXml.ExcelWorksheet> listOfSheets)
		{
			foreach (var sheet in listOfSheets)
			{
				sheet.Cells[1, 1].Value = "Nazwa";
				sheet.Cells[1, 2].Value = "Cena";
				sheet.Cells[1, 3].Value = "Data transakcji";
				sheet.Cells[1, 4].Value = "ID produktu";
				sheet.Cells[1, 5].Value = "Cena hurtowa brutto";
				sheet.Cells[1, 6].Value = "Cena hurtowa netto";
				sheet.Cells[1, 7].Value = "VAT";
			}
		}

		/// <summary>
		/// Makes a good-looking Excel table in each sheet.
		/// </summary>
		/// <param name="listOfSheets"></param>
		/// <param name="NoOfColumns"></param>
		private void MakeTableInEachSheet(ref List<OfficeOpenXml.ExcelWorksheet> listOfSheets, in int NoOfColumns)
		{
			int tableNumber = 1;
			foreach (var sheet in listOfSheets)
			{
				for (int i = 0; i < NoOfColumns; ++i)
				{
					sheet.Column(i + 1).AutoFit();
				}

				string tableName = "Tabela" + tableNumber.ToString();
				int firstRow = 1;
				int lastRow = sheet.Dimension.End.Row;
				int firstColumn = 1;
				int lastColumn = sheet.Dimension.End.Column;
				ExcelRange tableRange = sheet.Cells[firstRow, firstColumn, lastRow, lastColumn];
				ExcelTable table = sheet.Tables.Add(tableRange, tableName);
				++tableNumber;
			}
		}

		/// <summary>
		/// Generates .xlsx file (Excel), grouping provided data in table
		/// </summary>
		/// <param name="Orders"></param>
		/// <param name="path"></param>
		/// <param name="timestamp"></param>
		private async Task GenerateXlsx(List<CheckOutForm>? Orders, string path, string timestamp)
		{
			// Excel license
			// WARNING: check if the specified license is correct
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			// create excel workbook
			using (ExcelPackage excel = new ExcelPackage()) // used with classes implementing IDisposable
			{
				// name of the sheet
				var workSheet = excel.Workbook.Worksheets.Add("Orders--Liber" + timestamp);
				var workSheet2 = excel.Workbook.Worksheets.Add("Orders--Ateneum" + timestamp);
				var failoverWorkSheet = excel.Workbook.Worksheets.Add("Orders--FAILOVER" + timestamp);

				var listOfSheets = new[] { workSheet, workSheet2, failoverWorkSheet }.ToList();

				// IMPORTANT: const number of columns
				const int NoOfColumns = 7;
                SetColumnHeadersInXlsx(ref listOfSheets);
				int[] row = new int[3] { 2, 2, 2 };
				foreach (var order in Orders)
				{
					foreach (var item in order.lineItems)
					{
						//worksheet that we will do work 
						var currentSheet = failoverWorkSheet;

						// IMPORTANT: when parsing, DateTime converts dates to timezone of the computer running the app
						string tmp = DateTime.Parse(item.boughtAt).ToString("G");

						if (item.offer.external != null)
						{
							if (item.offer.external.id.EndsWith("-1"))
							{
								//liber
								currentSheet = workSheet;
								//System.Diagnostics.Debug.WriteLine(item.offer.external.id);
								var book = liberBooks.Where(bk => bk.ID == item.offer.external.id.Replace("-1", "")).FirstOrDefault();

								currentSheet.Cells[row[0], 1].Value = item.offer.name;
								currentSheet.Cells[row[0], 2].Value = item.originalPrice.amount;
								currentSheet.Cells[row[0], 3].Value = tmp;
								currentSheet.Cells[row[0], 4].Value = item.offer.external?.id;

								if (book == null)
								{
									currentSheet.Cells[row[0], 5].Value = "Brak mozliwosci uzupe³nienia brutto/netto/vat";
								}
								else
								{
									currentSheet.Cells[row[0], 5].Value = book.PriceNettoAferDiscount;
									currentSheet.Cells[row[0], 6].Value = book.PriceBruttoAferDiscount;
									currentSheet.Cells[row[0], 7].Value = book.Vat;
								}
								book = null;
								++row[0];
							}
							else if (item.offer.external.id.EndsWith("-2"))
							{
								//ateneum
								currentSheet = workSheet2;
								//System.Diagnostics.Debug.WriteLine(item.offer.external.id);
								var book = ateneumBooks.Where(bk => bk.ident_ate == item.offer.external.id.Replace("-2", "")).FirstOrDefault();

								currentSheet.Cells[row[1], 1].Value = item.offer.name;
								currentSheet.Cells[row[1], 2].Value = item.originalPrice.amount;
								currentSheet.Cells[row[1], 3].Value = tmp;
								currentSheet.Cells[row[1], 4].Value = item.offer.external?.id;

								if (book == null)
								{
									currentSheet.Cells[row[1], 5].Value = "Brak mozliwosci uzupe³nienia brutto/netto/vat";
								}
								else
								{
									currentSheet.Cells[row[1], 5].Value = book.PriceWholeSaleBrutto;
									currentSheet.Cells[row[1], 6].Value = book.PriceWholeSaleNetto;
									currentSheet.Cells[row[1], 7].Value = book.BookData.stawka_vat;
								}
                                book = null;
                                ++row[1];
							}
						}
						else
						{
							failoverWorkSheet.Cells[row[row.Length - 1], 1].Value = item.offer.name;
							failoverWorkSheet.Cells[row[row.Length - 1], 2].Value = item.originalPrice.amount;
							failoverWorkSheet.Cells[row[row.Length - 1], 3].Value = tmp;
							failoverWorkSheet.Cells[row[row.Length - 1], 4].Value = item.offer.external?.id;
							++row[2];
						}
					}
				}
				MakeTableInEachSheet(ref listOfSheets, NoOfColumns);

				excel.SaveAs(path);
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			// set default value in drop-down list (comboBox) to ".xslx"
			comboBox1.SelectedIndex = 1;
		}

		/// <summary>
		/// download all orders and start processing 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void button1_Click(object sender, EventArgs e)
		{
			button1.Enabled = false;

			// time format must be in UTC for allegro
			DateTime temp = new DateTime(this.dateTimePicker1.Value.Year, this.dateTimePicker1.Value.Month, this.dateTimePicker1.Value.Day);
			int offsetHours = GetCentralEuropeanTimeOffset(); // -2 if summer time and -1 if winter time
			temp = temp.AddHours(offsetHours);
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
				    await GenerateXlsx(Orders, path, timestamp);
					break;
			}

			// displaying everything and visual settings
			string dirPath = Path.Combine(Directory.GetCurrentDirectory(), "Orders");
			label3.Visible = true;
			label4.Text = $"orders{timestamp}{ending}";
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
			progressBar1.Visible = true;
			System.Diagnostics.Debug.WriteLine("pobieram liber");
			liberBooks = await Program.LibreApi.GetAllBooks(0);
			System.Diagnostics.Debug.WriteLine("pobieram ateneum");
			ateneumBooks = await Program.AteneumApi.GetAllBooksWithMagazin(0);
			progressBar1.Visible = false;
			button1.Enabled = true;

			System.Diagnostics.Debug.WriteLine(liberBooks.Count);
			System.Diagnostics.Debug.WriteLine(ateneumBooks.Count);
		}
	}
}