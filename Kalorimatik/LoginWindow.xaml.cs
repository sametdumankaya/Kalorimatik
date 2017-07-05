using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;
using Renci.SshNet;
using System.Net.NetworkInformation;
using System.Linq;

namespace Kalorimatik
{
	/// <summary>
	/// Interaction logic for LoginWindow.xaml
	/// </summary>
	public partial class LoginWindow : Window
	{
		MySqlConnection connection;
		bool holder = false;

		string currentUserEmail = "";
		string id = "";

		int totalInstalledMacs = 0;

		System.DateTime now;
		System.DateTime userEndDate;

		System.Collections.Generic.List<string> macs = new System.Collections.Generic.List<string>();

		public LoginWindow()
		{
			InitializeComponent();
		}

		public static System.DateTime GetNistTime()
		{
			System.DateTime dateTime = System.DateTime.MinValue;

			System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://nist.time.gov/actualtime.cgi?lzbc=siqm9b");
			request.Method = "GET";
			request.Accept = "text/html, application/xhtml+xml, */*";
			request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
			request.ContentType = "application/x-www-form-urlencoded";
			request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore); //No caching
			System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				System.IO.StreamReader stream = new System.IO.StreamReader(response.GetResponseStream());
				string html = stream.ReadToEnd();//<timestamp time=\"1395772696469995\" delay=\"1395772696469995\"/>
				string time = System.Text.RegularExpressions.Regex.Match(html, @"(?<=\btime="")[^""]*").Value;
				double milliseconds = System.Convert.ToInt64(time) / 1000.0;
				dateTime = new System.DateTime(1970, 1, 1).AddMilliseconds(milliseconds).ToLocalTime();
			}

			return dateTime;
		}

		private void ResetFields()
		{
			currentUserEmail = "";
			id = "";
			macs = new System.Collections.Generic.List<string>();
			holder = false;
		}

		private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			var startInfo = new System.Diagnostics.ProcessStartInfo("explorer.exe", e.Uri.AbsoluteUri);
			System.Diagnostics.Process.Start(startInfo);
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckConnection("http://www.kalorimatik.net"))
			{
				MessageBox.Show("Sunucuya bağlanılamıyor...", "Bağlantı Hatası", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			ResetFields();

			string email = _email.Text;
			string password = _password.Password;

			if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
			{
				MessageBox.Show("Lütfen email ve şifre alanlarını doldurunuz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			var slowTask3 = Task.Factory.StartNew(() => GetNistTime());
			var slowTask = Task.Factory.StartNew(() => SshConnect(email, password));

			_busyIndicator.IsBusy = true;
			now = await slowTask3;
			await slowTask;
			_busyIndicator.IsBusy = false;

			string currentMacAddress = GetMacAddress();

			if (macs.Count >= 2 && !macs.Contains(currentMacAddress))
			{
				MessageBox.Show("Daha önce bu hesapla iki bilgisayarda oturum açtınız. Bir hesapla ikiden fazla bilgisayarda oturum açamazsınız.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			if (totalInstalledMacs == 2)
			{
				if (!macs.Contains(currentMacAddress))
				{
					MessageBox.Show("Daha önce bu bilgisayarda iki hesap aktifleştirilmiş. Lütfen aktifleştirilmiş hesaplardan birini kullanın.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
				else
				{
					new MainWindow().Show();
					Close();
					return;
				}
			}
			else
			{
				if (holder)
				{
					if (!macs.Contains(currentMacAddress))
					{
						var slowTask2 = Task.Factory.StartNew(() => SshAddMacAddress());
						_busyIndicator.IsBusy = true;
						await slowTask2;
						_busyIndicator.IsBusy = false;
					}

					new MainWindow().Show();
					Close();
					return;
				}
			}
		}

		private bool CheckConnection(string URL)
		{
			try
			{
				System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URL);
				request.Timeout = 5000;
				request.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
				System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();

				return response.StatusCode == System.Net.HttpStatusCode.OK;
			}
			catch
			{
				return false;
			}
		}

		private void SshAddMacAddress()
		{
			string username = "kalobkwr";
			string password = "eA7TfF1$9gDBN";
			string host = "server226.web-hosting.com";
			int port = 21098;

			using (var client = new SshClient(host, port, username, password))
			{
				client.Connect();
				var tunnel = new ForwardedPortLocal("127.0.0.1", 21098, "127.0.0.1", 3306);
				client.AddForwardedPort(tunnel);
				tunnel.Start();

				TryToAddMac(id);

				tunnel.Stop();
				client.Disconnect();
			}
		}

		private void SshConnect(string useremail, string userpassword)
		{
			string username = "kalobkwr";
			string password = "eA7TfF1$9gDBN";
			string host = "server226.web-hosting.com";
			int port = 21098;

			using (var client = new SshClient(host, port, username, password))
			{
				client.Connect();
				var tunnel = new ForwardedPortLocal("127.0.0.1", 21098, "127.0.0.1", 3306);
				client.AddForwardedPort(tunnel);
				tunnel.Start();
				TryToConnectDatabase(useremail, userpassword);
				TryToGetMacs(id);
				tunnel.Stop();
				client.Disconnect();
			}
		}

		private string GetMacAddress()
		{
			var macAddr =
			(
				from nic in NetworkInterface.GetAllNetworkInterfaces()
				where nic.OperationalStatus == OperationalStatus.Up
				select nic.GetPhysicalAddress().ToString()
			).FirstOrDefault();

			return macAddr;
		}

		private void TryToConnectDatabase(string email, string password)
		{
			string connString = "Server=127.0.0.1; Port=21098; Database=kalobkwr_kalorimatik; Uid=kalobkwr_user; Pwd=05326204259;";

			using (connection = new MySqlConnection(connString))
			{
				using (MySqlCommand cmd = connection.CreateCommand())
				{
					connection.Open();
					cmd.CommandText = @"SELECT * FROM users WHERE email = ?Email AND password = ?Password;";
					cmd.Parameters.AddWithValue("Email", email);
					cmd.Parameters.AddWithValue("Password", password);
					MySqlDataReader reader = cmd.ExecuteReader();

					if (!reader.HasRows)
					{
						MessageBox.Show("Email veya şifre hatalı. Lütfen giriş bilgilerinizi kontrol edip tekrar deneyiniz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
						reader.Close();
						connection.Close();
						return;
					}

					string dbEmail = "";
					string dbPassword = "";
					string dbIsLocked = "";

					while (reader.Read())
					{
						id = reader[0].ToString();

						dbEmail = reader[1].ToString();
						dbPassword = reader[2].ToString();
						userEndDate = System.DateTime.Parse(reader[3].ToString());
						dbIsLocked = reader[4].ToString();

						currentUserEmail = dbEmail;
					}

					if (userEndDate < now)
					{
						if (MessageBox.Show("Uygulama planı satın almak için lütfen sametdumankaya@gmail.com adresi ile iletişime geçin.", "Hata", MessageBoxButton.OKCancel, MessageBoxImage.Hand) == MessageBoxResult.OK)
						{
							System.Diagnostics.Process.Start("mailto:sametdumankaya@gmail.com");

							//var startInfo = new System.Diagnostics.ProcessStartInfo("explorer.exe", "http://www.kalorimatik.net/register.html");
							//System.Diagnostics.Process.Start(startInfo);
							reader.Close();
							connection.Close();
							return;
						}
					}
					else
					{
						holder = true;
						reader.Close();
						connection.Close();
						return;
					}

					reader.Close();
					connection.Close();
				}
			}
		}

		

		private void TryToGetMacs(string id)
		{
			if (currentUserEmail.Equals(""))
			{
				return;
			}

			string connString = "Server=127.0.0.1; Port=21098; Database=kalobkwr_kalorimatik; Uid=kalobkwr_user; Pwd=05326204259;";

			using (connection = new MySqlConnection(connString))
			{
				using (MySqlCommand cmd = connection.CreateCommand())
				{
					connection.Open();
					//first query

					cmd.CommandText = @"SELECT mac FROM macs WHERE user_id_fk = ?Id;";
					cmd.Parameters.AddWithValue("Id", id);
					MySqlDataReader reader = cmd.ExecuteReader();
					
					string mac = "";

					macs = new System.Collections.Generic.List<string>();

					while (reader.Read())
					{
						mac = reader[0].ToString();
						macs.Add(mac);
					}

					//second query
					cmd.CommandText = @"SELECT COUNT(mac), user_id_fk FROM macs WHERE mac = ?Mac;";
					cmd.Parameters.AddWithValue("Mac", GetMacAddress());
					reader.Close();
					reader = cmd.ExecuteReader();

					while (reader.Read())
					{
						totalInstalledMacs = System.Convert.ToInt32(reader[0].ToString());
					}

					reader.Close();
					connection.Close();
				}
			}
		}

		private void TryToAddMac(string id)
		{
			string connString = "Server=127.0.0.1; Port=21098; Database=kalobkwr_kalorimatik; Uid=kalobkwr_user; Pwd=05326204259;";

			using (connection = new MySqlConnection(connString))
			{
				using (MySqlCommand cmd = connection.CreateCommand())
				{
					connection.Open();
					cmd.CommandText = @"INSERT INTO macs VALUES (?Mac, ?Id);";
					cmd.Parameters.AddWithValue("Mac", GetMacAddress());
					cmd.Parameters.AddWithValue("Id", id);
					cmd.ExecuteNonQuery();

					connection.Close();
				}
			}
		}


		private static string CreateMD5(string input)
		{
			// Use input string to calculate MD5 hash
			using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
			{
				byte[] inputBytes = Encoding.ASCII.GetBytes(input);
				byte[] hashBytes = md5.ComputeHash(inputBytes);

				// Convert the byte array to hexadecimal string
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < hashBytes.Length; i++)
				{
					sb.Append(hashBytes[i].ToString("X2"));
				}
				return sb.ToString();
			}
		}

		private void _password_GotFocus(object sender, RoutedEventArgs e)
		{
			_password.SelectAll();
		}

		private void _email_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Enter)
			{
				_button.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
			}
		}
	}
}
