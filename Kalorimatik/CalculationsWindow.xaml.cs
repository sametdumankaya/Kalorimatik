using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Kalorimatik
{
	/// <summary>
	/// Interaction logic for CalculationsWindow.xaml
	/// </summary>
	public partial class CalculationsWindow : Window
	{
		private static CalculationsWindow instance;

		private CalculationsWindow()
		{
			InitializeComponent();
		}

		public static CalculationsWindow GetInstance()
		{
			if(instance == null)
			{
				instance = new CalculationsWindow();
			}

			return instance;
		}

		//Calculate Body Mass Index
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			_vkiBoy.Background = Brushes.White;
			_vkiBoy.Foreground = Brushes.Black;
			_vkiKilo.Background = Brushes.White;
			_vkiKilo.Foreground = Brushes.Black;

			if (string.IsNullOrWhiteSpace(_vkiBoy.Text))
			{
				_vkiBoy.Background = Brushes.Red;
				_vkiBoy.Foreground = Brushes.White;
				MessageBox.Show("Lütfen beden kitle indeksi alanındaki bütün boşlukları doldurunuz.","Hata", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			else if(string.IsNullOrWhiteSpace(_vkiKilo.Text))
			{
				_vkiKilo.Background = Brushes.Red;
				_vkiKilo.Foreground = Brushes.White;
				MessageBox.Show("Lütfen beden kitle indeksi alanındaki bütün boşlukları doldurunuz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			else
			{
				int valueVkiBoy = Convert.ToInt32(_vkiBoy.Text);
				int valueVkiKilo = Convert.ToInt32(_vkiKilo.Text);

				float sonuc = (float)(valueVkiKilo * 10000) / (valueVkiBoy * valueVkiBoy);

				_vkiSonuc.Text = string.Format("{0:n2}", sonuc);
			}
		}

		//Calculate Schofield 
		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			SetDefaultTextBoxColors();

			if (string.IsNullOrWhiteSpace(_bmhBoy.Text))
			{
				_bmhBoy.Background = Brushes.Red;
				_bmhBoy.Foreground = Brushes.White;
				MessageBox.Show("Lütfen bazal metabolizma hızı alanındaki bütün boşlukları doldurunuz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			else if(string.IsNullOrWhiteSpace(_bmhKilo.Text))
			{
				_bmhKilo.Background = Brushes.Red;
				_bmhKilo.Foreground = Brushes.White;
				MessageBox.Show("Lütfen bazal metabolizma hızı alanındaki bütün boşlukları doldurunuz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			else if(string.IsNullOrWhiteSpace(_bmhYas.Text))
			{
				_bmhYas.Background = Brushes.Red;
				_bmhYas.Foreground = Brushes.White;
				MessageBox.Show("Lütfen bazal metabolizma hızı alanındaki bütün boşlukları doldurunuz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			else
			{
				double first = 0;
				double second = 0;

				int valueBmhKilo = Convert.ToInt32(_bmhKilo.Text);
				int valueBmhYas = Convert.ToInt32(_bmhYas.Text);

				if(_bmhCinsiyet.SelectedIndex == 0)
				{
					//Women
					if(valueBmhYas < 3)
					{
						first = 58.317;
						second = -31.1;
					}
					else if(valueBmhYas < 10)
					{
						first = 20.315;
						second = 485.9;
					}
					else if (valueBmhYas < 18)
					{
						first = 13.384;
						second = 692.6;
					}
					else if (valueBmhYas < 30)
					{
						first = 14.818;
						second = 486.6;

					}
					else if (valueBmhYas < 60)
					{
						first = 8.126;
						second = 845.6;
					}
					else
					{
						first = 9.082;
						second = 658.6;
					}
				}
				else
				{
					//Men
					if (valueBmhYas < 3)
					{
						first = 59.512;
						second = -30.4;
					}
					else if (valueBmhYas < 10)
					{
						first = 22.706;
						second = 504.3;
					}
					else if (valueBmhYas < 18)
					{
						first = 17.686;
						second = 658.2;
					}
					else if (valueBmhYas < 30)
					{
						first = 15.057;
						second = 692.2;

					}
					else if (valueBmhYas < 60)
					{
						first = 11.472;
						second = 873.1;
					}
					else
					{
						first = 11.711;
						second = 587.7;
					}
				}

				double sonuc = first * valueBmhKilo + second;
				
				_bmhSonuc.Text = string.Format("{0:n2}", sonuc);
			}
		}

		private void SetDefaultTextBoxColors()
		{
			_bmhBoy.Background = Brushes.White;
			_bmhBoy.Foreground = Brushes.Black;
			_bmhKilo.Background = Brushes.White;
			_bmhKilo.Foreground = Brushes.Black;
			_bmhYas.Background = Brushes.White;
			_bmhYas.Foreground = Brushes.Black;
		}

		//Calculate Harris - Benedict 
		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			SetDefaultTextBoxColors();

			if (string.IsNullOrWhiteSpace(_bmhBoy.Text))
			{
				_bmhBoy.Background = Brushes.Red;
				_bmhBoy.Foreground = Brushes.White;
				MessageBox.Show("Lütfen bazal metabolizma hızı alanındaki bütün boşlukları doldurunuz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			else if (string.IsNullOrWhiteSpace(_bmhKilo.Text))
			{
				_bmhKilo.Background = Brushes.Red;
				_bmhKilo.Foreground = Brushes.White;
				MessageBox.Show("Lütfen bazal metabolizma hızı alanındaki bütün boşlukları doldurunuz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			else if (string.IsNullOrWhiteSpace(_bmhYas.Text))
			{
				_bmhYas.Background = Brushes.Red;
				_bmhYas.Foreground = Brushes.White;
				MessageBox.Show("Lütfen bazal metabolizma hızı alanındaki bütün boşlukları doldurunuz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			else
			{
				double sonuc = 0;
				double multiplier = 0;

				int valueBmhBoy = Convert.ToInt32(_bmhBoy.Text);
				int valueBmhKilo = Convert.ToInt32(_bmhKilo.Text);
				int valueBmhYas = Convert.ToInt32(_bmhYas.Text);

				if (_bmhCinsiyet.SelectedIndex == 0)
				{
					//Women
					sonuc = (10 * valueBmhKilo) + (6.25 * valueBmhBoy) - (5 * valueBmhYas) - 161;
				}
				else
				{
					//Men
					sonuc = (10 * valueBmhKilo) + (6.25 * valueBmhBoy) - (5 * valueBmhYas) + 5;
				}

				switch (_bmhAktivite.SelectedIndex)
				{
					case 0:
						multiplier = 1.2;
						break;

					case 1:
						multiplier = 1.375;
						break;

					case 2:
						multiplier = 1.55;
						break;

					case 3:
						multiplier = 1.725;
						break;

					case 4:
						multiplier = 1.9;
						break;	
				}

				sonuc *= multiplier;
				
				_bmhSonuc.Text = string.Format("{0:n2}", sonuc);
			}
		}


		//prevents non-numeric input
		private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			if (!char.IsDigit(e.Text, e.Text.Length - 1))
			{
				e.Handled = true;
			}
		}

		//prevents white space input
		private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space)
			{
				e.Handled = true;
			}
		}

		//prevents pasting data
		private void TextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (e.Command == ApplicationCommands.Paste)
			{
				e.Handled = true;
			}
		}

		//window closing event handler
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			instance = null;
		}

		//Back button event handler
		private void Button_Click_3(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
