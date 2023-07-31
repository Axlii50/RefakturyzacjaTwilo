using Allegro_Api;
using System.Diagnostics;

namespace RefakturyzacjaTwilo
{
	internal static class Program
	{
		static string ClientSecret = "aKgn8GbxJqghLVvqvYpM3Bdlb5eQmCdx6jm2KBybsmSNEfYZtnuHCemwLa5xOvde";
		static string ClientID = "0292044ee78a47f2a7f315ece84edfe5";

		// refresh token wazny przez 3 miesiace of 28.07.2023
		static string refreshToken = "";

		public static AllegroApi _allegroApi = null;
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			if (File.Exists("RefreshToken.txt"))
			{
				refreshToken = File.ReadAllText("RefreshToken.txt");

				_allegroApi = new AllegroApi(ClientID, ClientSecret, refreshToken, _allegroApi_RefreshTokenEvent);
			}
			else
			{
				Allegro_Api.Models.VerificationULRModel t = _allegroApi.Authenticate().Result;

				ProcessStartInfo sInfo = new ProcessStartInfo(t.verification_uri_complete);
				sInfo.UseShellExecute = true;
				Process Verification = Process.Start(sInfo);

				bool access = false;
				while (!access)
				{
					Allegro_Api.AllegroPermissionState Permissions = AllegroPermissionState.allegro_api_sale_offers_read | AllegroPermissionState.allegro_api_sale_offers_write;

					access = _allegroApi.CheckForAccessToken(Permissions).Result;

					Thread.Sleep(5000);
				}

			}

			if (!Directory.Exists("Orders"))
				Directory.CreateDirectory("Orders");

			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			ApplicationConfiguration.Initialize();
			Application.Run(new Form1());
		}

		private static void _allegroApi_RefreshTokenEvent()
		{
			System.Diagnostics.Debug.WriteLine(_allegroApi.RefreshToken);
			File.WriteAllText("RefreshToken.txt", _allegroApi.RefreshToken);
		}
	}
}