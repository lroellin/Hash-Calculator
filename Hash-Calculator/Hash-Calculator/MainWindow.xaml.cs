using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Security.Cryptography;
using System.IO;
using System.Windows.Shell;

namespace Hash_Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

		string pthFile;
		List<Task> tasks = new List<Task>();
		List<GUIRow> rows = new List<GUIRow>();
		Boolean tasksCompleted = true;
		int nrOfTasks = 0;
		Thickness thkNormal = new Thickness(1);
		Thickness thkThick = new Thickness(5, 1, 5, 1);

        public MainWindow()
        {
            InitializeComponent();
			this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
			Title = "Hash Calculator";
        }

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			rows.Add(new GUIRow(SupportedHashAlgorithm.MD5, chkMD5, txtMD5, cpyMD5, prgMD5));
			rows.Add(new GUIRow(SupportedHashAlgorithm.SHA1, chkSHA1, txtSHA1, cpySHA1, prgSHA1));
			rows.Add(new GUIRow(SupportedHashAlgorithm.SHA256, chkSHA256, txtSHA256, cpySHA256, prgSHA256));
			rows.Add(new GUIRow(SupportedHashAlgorithm.SHA384, chkSHA384, txtSHA384, cpySHA384, prgSHA384));
			rows.Add(new GUIRow(SupportedHashAlgorithm.SHA512, chkSHA512, txtSHA512, cpySHA512, prgSHA512));
			setInitialState();
		}


		private void setInitialState()
		{
			lblStatus.Content = "Ready";
			foreach (GUIRow row in rows)
			{
				row.CopyButton.Tag = row.TextBox;
				row.CopyButton.Click += new RoutedEventHandler(this.copyToClipboard);
				row.TextBox.BorderThickness = thkNormal;
				row.ProgressBar.IsIndeterminate = false;
			}
		}

		private void copyToClipboard(object sender, RoutedEventArgs e)
		{
			TextBox textBox = (TextBox)((Button)sender).Tag;
			Clipboard.SetText(textBox.Text);
		}

		private void btnFileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.CheckFileExists = true;
			openFileDialog.CheckPathExists = true; 

            if(openFileDialog.ShowDialog() == true)
            {
                txtFileOpen.Text = openFileDialog.FileName;
				btnCalculate.IsEnabled = true;
			}
        }

		private async void btnCalculate_Click(object sender, RoutedEventArgs e)
		{
			if (!checkTasksCompleted())
			{
				alertCalculationOngoing();
				return;
			}
			tasksCompleted = false;
			prgTaskbar.ProgressState = TaskbarItemProgressState.Indeterminate;
			pthFile = txtFileOpen.Text;
			tasks.Clear();
			btnSave.IsEnabled = true;
			lblStatus.Content = "Starting threads...";
			prgMain.Maximum = 0;
			prgMain.Value = 0;
			foreach (GUIRow row in rows)
			{
				if(row.isChecked())
				{
					row.ProgressBar.IsIndeterminate = true;
					row.TextBox.Text = "";
					prgMain.Maximum++;
					startTask(row);
				}
			}
			lblStatus.Content = "Working...";

			await Task.WhenAll(tasks);
			lblStatus.Content = "Done";
			prgTaskbar.ProgressState = TaskbarItemProgressState.None;
			tasksCompleted = true;
		}

		private void startTask(GUIRow row)
		{
			Hash hash = new Hash(row.HashAlgorithm, false);
			Task<Hash> task = Task.Run(() => HashCalculation.calculateHash(hash, pthFile));
			task.ContinueWith((t) =>
			 {
				 if (t.IsFaulted)
				 {
					 if (t.Exception.InnerException is HMACNotSupportedException)
					 {
						 row.TextBox.Text = "HMAC not supported";
					 }
				 }
				 else
				 {
					 row.TextBox.Text = t.Result.HashString;
					 row.ProgressBar.IsIndeterminate = false;
					 prgMain.Value++;
				 }
			 }, TaskScheduler.FromCurrentSynchronizationContext());
			tasks.Add(task);			
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			if(!checkTasksCompleted())
			{
				alertCalculationOngoing();
				return;
			}
			String pthFileNoPath = Path.GetFileName(pthFile);
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.FileName = "Hash-" + pthFileNoPath + ".txt";
			saveFileDialog.Filter = "Text files (*.txt) | *.txt | All files(*.*) | *.*";
			saveFileDialog.ShowDialog();
			if(saveFileDialog.FileName != "")
			{
				HashFile hashFile = new HashFile(saveFileDialog.FileName);
				hashFile.addHeader(pthFile);
				foreach(GUIRow row in rows) {
					if(row.isChecked())
					{
						hashFile.addHashline(row.HashAlgorithm, row.TextBox.Text);
					}
				}
				hashFile.closeFile();
			}
		}

		private static void alertCalculationOngoing()
		{
			MessageBox.Show("Calculation has not yet completed.", "Calculation ongoing", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private bool checkTasksCompleted()
		{
			return tasksCompleted;
		}

		private void txtCompare_TextChanged(object sender, TextChangedEventArgs e)
		{
			setInitialState();
			// Search through results
			foreach (GUIRow row in rows)
			{
				// If found
				if (row.TextBox.Text.Equals(txtCompare.Text))
				{
					row.TextBox.BorderThickness = thkThick;
					row.TextBox.BorderBrush = Brushes.Green;
				}
			}
		}

		private void btnClose_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}


	}
}
