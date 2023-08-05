using Allegro_Api;
using AteneumAPI;
using Libre_API;
using System.Diagnostics;

namespace RefakturyzacjaTwilo
{
	internal static class Program
	{
		static string ClientSecret = "PjOcDyDm4ZdjOhrdgOqQQMCY6Row2DWJhnwjjPRAwdQcKLCqpV0fbSjrZ2drQnvf";
		static string ClientID = "31b0bc689e414c608d7098aa3966f8f4";

		// refresh token wazny przez 3 miesiace of 28.07.2023
		static string refreshToken = "";

		public static AllegroApi _allegroApi = null;

        public static LibreApi LibreApi { get; private set; }
        public static AteneumApi AteneumApi { get; private set; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
		static void Main()
		{
            string LibreLogin = "38103_2345";
            string LibrePassword = "38103";

            LibreApi = new LibreApi(LibrePassword, LibreLogin);

            string AteneumLogin = "kempo_warszawa";
            string AteneumPassword = "6KsSGWT6dhD9r8Xvvr";

            AteneumApi = new AteneumApi(AteneumLogin, AteneumPassword);


            if (File.Exists("RefreshToken.txt") && true)
			{
				refreshToken = File.ReadAllText("RefreshToken.txt");

				_allegroApi = new AllegroApi(ClientID, ClientSecret, refreshToken, _allegroApi_RefreshTokenEvent);
			}
			else
			{
                _allegroApi = new AllegroApi(ClientID, ClientSecret, _allegroApi_RefreshTokenEvent);
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