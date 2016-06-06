using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Dreami.Hash_Calculator
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Settings : Window
	{
		public Settings()
		{
			InitializeComponent();
			chkPlaySound.IsChecked = Properties.Settings.Default.PlaySoundWhenDone;
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void btnApply_Click(object sender, RoutedEventArgs e)
		{
			if(chkPlaySound.IsChecked == true)
			{
				Properties.Settings.Default.PlaySoundWhenDone = true;
			} else
			{
				Properties.Settings.Default.PlaySoundWhenDone = false;
			}
			Properties.Settings.Default.Save();
		}

		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			btnApply_Click(sender, e);
			Close();
		}

		private void btnDefault_Click(object sender, RoutedEventArgs e)
		{
			chkPlaySound.IsChecked = false;
		}
	}
}
