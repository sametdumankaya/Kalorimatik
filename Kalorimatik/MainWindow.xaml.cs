using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Kalorimatik
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private TextBox[] textboxes;

		public MainWindow()
		{
			InitializeComponent();

			this.textboxes = new TextBox[]
			{
				_khYuzde1,
				_khYuzde2,
				_yagYuzde1,
				_yagYuzde2,
				_proteinYuzde1,
				_proteinYuzde2,
				_sutLimitMin,
				_sutLimitMax,
				_eygLimitMin,
				_eygLimitMax,
				_meyveLimitMin,
				_meyveLimitMax,
				_etLimitMin,
				_etLimitMax,
				_sebzeLimitMin,
				_sebzeLimitMax,
				_yagLimitMin,
				_yagLimitMax,
				_sekerGram,
				_proteinGram,
				_minKalori,
				_maxKalori
			};

		}

		private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			if (!char.IsDigit(e.Text, e.Text.Length - 1))
				e.Handled = true;
		}

		private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space)
			{
				e.Handled = true;
			}
		}

		private void _khYuzde1_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (e.Command == ApplicationCommands.Paste)
			{
				e.Handled = true;
			}
		}

		//otomatik doldur click
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			FillAutomatically();
		}

		private void FillAutomatically()
		{
			SetTextBoxesDefaultColor();

			_khYuzde1.Text = "55";
			_khYuzde2.Text = "60";
			_yagYuzde1.Text = "25";
			_yagYuzde2.Text = "30";
			_proteinYuzde1.Text = "12";
			_proteinYuzde2.Text = "15";
			_sutLimitMin.Text = "2";
			_sutLimitMax.Text = "4";
			_eygLimitMin.Text = "4";
			_eygLimitMax.Text = "9";
			_meyveLimitMin.Text = "2";
			_meyveLimitMax.Text = "6";
			_etLimitMin.Text = "2";
			_etLimitMax.Text = "5";
			_sebzeLimitMin.Text = "2";
			_sebzeLimitMax.Text = "4";
			_yagLimitMin.Text = "2";
			_yagLimitMax.Text = "5";
			_sekerGram.Text = "0";
			_proteinGram.Text = "0";
			_minKalori.Text = "1375";
			_maxKalori.Text = "1425";
		}

		//değerleri temizle click
		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			SetTextBoxesDefaultColor();

			foreach (var textbox in this.textboxes)
			{
				textbox.Text = "";
			}
		}

		//tabloyu temizle click
		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			_dataGrid.ItemsSource = null;
		}

		private void Button_Click_3(object sender, RoutedEventArgs e)
		{
			Calculate();
		}

		private void Calculate()
		{
			SetTextBoxesDefaultColor();

			foreach (var textbox in textboxes)
			{
				if (string.IsNullOrWhiteSpace(textbox.Text))
				{
					textbox.Background = Brushes.Yellow;

					MessageBox.Show("Lütfen bütün boşlukları doldurunuz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
			}

			RequiredValues rv = new RequiredValues();

			//degisim limitleri
			rv.valueSutMin = Convert.ToInt32(_sutLimitMin.Text);
			rv.valueSutMax = Convert.ToInt32(_sutLimitMax.Text);
			rv.valueEYGMin = Convert.ToInt32(_eygLimitMin.Text);
			rv.valueEYGMax = Convert.ToInt32(_eygLimitMax.Text);
			rv.valueMeyveMin = Convert.ToInt32(_meyveLimitMin.Text);
			rv.valueMeyveMax = Convert.ToInt32(_meyveLimitMax.Text);
			rv.valueEtMin = Convert.ToInt32(_etLimitMin.Text);
			rv.valueEtMax = Convert.ToInt32(_etLimitMax.Text);
			rv.valueSebzeMin = Convert.ToInt32(_sebzeLimitMin.Text);
			rv.valueSebzeMax = Convert.ToInt32(_sebzeLimitMax.Text);
			rv.valueYagMin = Convert.ToInt32(_yagLimitMin.Text);
			rv.valueYagMax = Convert.ToInt32(_yagLimitMax.Text);

			//yüzdeler
			rv.valueKhYuzdeMin = Convert.ToInt32(_khYuzde1.Text);
			rv.valueKhYuzdeMax = Convert.ToInt32(_khYuzde2.Text);
			rv.valueYagYuzdeMin = Convert.ToInt32(_yagYuzde1.Text);
			rv.valueYagYuzdeMax = Convert.ToInt32(_yagYuzde2.Text);
			rv.valueProteinYuzdeMin = Convert.ToInt32(_proteinYuzde1.Text);
			rv.valueProteinYuzdeMax = Convert.ToInt32(_proteinYuzde2.Text);

			//ekstralar
			rv.valueEkstraSeker = Convert.ToInt32(_sekerGram.Text);
			rv.valueEkstraProtein = Convert.ToInt32(_proteinGram.Text);

			//kalori limitleri
			rv.valueMinKalori = Convert.ToInt32(_minKalori.Text);
			rv.valueMaxKalori = Convert.ToInt32(_maxKalori.Text);

			for (int i = 0; i < textboxes.Length; i += 2)
			{
				if (i == 18)
				{
					continue;
				}

				if (Convert.ToInt32(textboxes[i].Text) > Convert.ToInt32(textboxes[i + 1].Text))
				{
					textboxes[i].Background = Brushes.Red;
					textboxes[i].Foreground = Brushes.White;
					MessageBox.Show("Minimum değerler maksimum değerlerden büyük olamaz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
			}

			FillDataGrid(rv);
		}

		private void FillDataGrid(RequiredValues rv)
		{
			List<DataGridBeanRow> list = new List<DataGridBeanRow>();

			for (int a = rv.valueSutMin; a <= rv.valueSutMax; a++)
			{
				for (int b = rv.valueEYGMin; b <= rv.valueEYGMax; b++)
				{
					for (int c = rv.valueMeyveMin; c <= rv.valueMeyveMax; c++)
					{
						for (int d = rv.valueEtMin; d <= rv.valueEtMax; d++)
						{
							for (int e = rv.valueSebzeMin; e <= rv.valueSebzeMax; e++)
							{
								for (int f = rv.valueYagMin; f <= rv.valueYagMax; f++)
								{
									int khTotalKkal = a * 36 + b * 60 + c * 60 + e * 24 + rv.valueEkstraSeker * 4;
									int proteinTotalKkal = d * 24 + a * 24 + b * 8 + e * 8 + rv.valueEkstraProtein * 4;
									int yagTotalKkal = d * 45 + a * 54 + f * 45;
									int totalKalori = khTotalKkal + proteinTotalKkal + yagTotalKkal;

									if (totalKalori >= rv.valueMinKalori && totalKalori <= rv.valueMaxKalori)
									{
										float khYuzdeSonuc = ((float)khTotalKkal / totalKalori) * 100;
										float proteinYuzdeSonuc = ((float)proteinTotalKkal / totalKalori) * 100;
										float yagYuzdeSonuc = ((float)yagTotalKkal / totalKalori) * 100;

										if (khYuzdeSonuc >= rv.valueKhYuzdeMin && khYuzdeSonuc <= rv.valueKhYuzdeMax &&
											proteinYuzdeSonuc >= rv.valueProteinYuzdeMin && proteinYuzdeSonuc <= rv.valueProteinYuzdeMax &&
											yagYuzdeSonuc >= rv.valueYagYuzdeMin && yagYuzdeSonuc <= rv.valueYagYuzdeMax)
										{
											list.Add(new DataGridBeanRow(a, d, b, e, c, f, (a + b + c + d + e + f), khYuzdeSonuc, proteinYuzdeSonuc, yagYuzdeSonuc, rv.valueEkstraSeker, rv.valueEkstraProtein, totalKalori));
										}
									}
								}
							}
						}
					}
				}
			}

			_dataGrid.ItemsSource = list;

			if(list.Count == 0)
			{
				MessageBox.Show("Verilen parametrelere uygun hiçbir veri bulunamadı", "Bilgi",MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void SetTextBoxesDefaultColor()
		{
			foreach (var textbox in textboxes)
			{
				textbox.Background = Brushes.White;
				textbox.Foreground = Brushes.Black;
			}
		}

		private void Image_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			CalculationsWindow.GetInstance().Show();
			CalculationsWindow.GetInstance().Activate();	
		}
	}

	class RequiredValues
	{
		//degisim limitleri
		public int valueSutMin { get; set; }
		public int valueSutMax { get; set; }
		public int valueEYGMin { get; set; }
		public int valueEYGMax { get; set; }
		public int valueMeyveMin { get; set; }
		public int valueMeyveMax { get; set; }
		public int valueEtMin { get; set; }
		public int valueEtMax { get; set; }
		public int valueSebzeMin { get; set; }
		public int valueSebzeMax { get; set; }
		public int valueYagMin { get; set; }
		public int valueYagMax { get; set; }

		//yüzdeler					
		public int valueKhYuzdeMin { get; set; }
		public int valueKhYuzdeMax { get; set; }
		public int valueYagYuzdeMin { get; set; }
		public int valueYagYuzdeMax { get; set; }
		public int valueProteinYuzdeMin { get; set; }
		public int valueProteinYuzdeMax { get; set; }

		//ekstralar					
		public int valueEkstraSeker { get; set; }
		public int valueEkstraProtein { get; set; }

		//kalori limitleri			
		public int valueMinKalori { get; set; }
		public int valueMaxKalori { get; set; }
	}

	class DataGridBeanRow
	{
		public int sut { get; }
		public int et { get; }
		public int eyg { get; }
		public int sebze { get; }
		public int meyve { get; }
		public int yag { get; }
		public int toplamDegisim { get; }
		public string karbonhidratYuzde { get; }
		public string proteinYuzde { get; }
		public string yagYuzde { get; }
		public string ekstraSeker { get; }
		public string ekstraProtein { get; }
		public int toplamKalori { get; }

		public DataGridBeanRow(int sut, int et, int eyg, int sebze, int meyve, int yag, int toplamDegisim, float karbonhidratYuzde, float proteinYuzde, float yagYuzde, int ekstraSeker, int ekstraProtein, int toplamKalori)
		{
			this.sut = sut;
			this.et = et;
			this.eyg = eyg;
			this.sebze = sebze;
			this.meyve = meyve;
			this.yag = yag;
			this.toplamDegisim = toplamDegisim;

			this.karbonhidratYuzde = string.Format("{0:n2}%", karbonhidratYuzde);
			this.proteinYuzde = string.Format("{0:n2}%", proteinYuzde);
			this.yagYuzde = string.Format("{0:n2}%", yagYuzde);
			this.ekstraSeker = ekstraSeker + " gr";
			this.ekstraProtein = ekstraProtein + " gr";
			this.toplamKalori = toplamKalori;
		}
	}
}