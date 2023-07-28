using Allegro_Api;
using System.Diagnostics;

namespace RefakturyzacjaTwilo
{
    internal static class Program
    {
		static string ClientSecret = "PjOcDyDm4ZdjOhrdgOqQQMCY6Row2DWJhnwjjPRAwdQcKLCqpV0fbSjrZ2drQnvf";
		static string ClientID = "31b0bc689e414c608d7098aa3966f8f4";
		// refresh token wazny przez 3 miesiace of 28.07.2023
		static string refreshToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX25hbWUiOiIxMTI4MDE1MjIiLCJzY29wZSI6WyJhbGxlZ3JvOmFwaTpvcmRlcnM6cmVhZCIsImFsbGVncm86YXBpOnNhbGU6b2ZmZXJzOndyaXRlIiwiYWxsZWdybzphcGk6c2FsZTpzZXR0aW5nczp3cml0ZSIsImFsbGVncm86YXBpOnNhbGU6c2V0dGluZ3M6cmVhZCIsImFsbGVncm86YXBpOnNhbGU6b2ZmZXJzOnJlYWQiLCJhbGxlZ3JvOmFwaTpvcmRlcnM6d3JpdGUiXSwiYWxsZWdyb19hcGkiOnRydWUsImF0aSI6IjYyMzM3NTBjLTk3NDUtNGE0OS1iZDY2LWFmOWFlMTc1OTJmZCIsImlzcyI6Imh0dHBzOi8vYWxsZWdyby5wbCIsImV4cCI6MTY5ODMzNjUwMiwianRpIjoiODBhMjhmYTYtNWQ1NC00MDNmLTliY2ItYjc5MjY3YmJmNjE0IiwiY2xpZW50X2lkIjoiMzFiMGJjNjg5ZTQxNGM2MDhkNzA5OGFhMzk2NmY4ZjQifQ.SWAMQxeQSRyC-RtOTeCUrCYurOZRvOWSa7kOFQC-pqgnag70s4DkynXjFpr0F2qxEqK5j_pmA94MR3XFpoStvJA3DmAPvMDahH2TPPCfcIn82mzuL1T7px-v3pdSNyyzwRv1BKHf_I22_No7aSMHjjb-oYe-RdYH4pjy1aLCe_y5JP2m59d3cwNH-Q6o5jCbekXxV8Cyv882KbkQ5Cat6Dm_1d5aNLGtTVqSn-jDm4DhAw5SM4NzWFY3ABo2g05UiPxhsoaztJA0g192JVOCIwdPw2GbZHV478oml92Kfo-2-Xk_bJyAByZP0q6kY9gSbNGaOkbG9cNvSNo65jvZBw";

		public static AllegroApi AllegroApi = new AllegroApi(ClientID, ClientSecret, refreshToken);
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
        static void Main()
        {
			/*
			Allegro_Api.Models.VerificationULRModel t = AllegroApi.Authenticate().Result;

			ProcessStartInfo sInfo = new ProcessStartInfo(t.verification_uri_complete);
			sInfo.UseShellExecute = true;
			Process Verification = Process.Start(sInfo);

			bool access = false;
			while (!access)
			{
				Allegro_Api.AllegroPermissionState Permissions = AllegroPermissionState.allegro_api_sale_offers_read | AllegroPermissionState.allegro_api_sale_offers_write;

				access = AllegroApi.CheckForAccessToken(Permissions).Result;

				Thread.Sleep(5000);
			}
			*/


			if (!Directory.Exists("Orders"))
				Directory.CreateDirectory("Orders");


			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}