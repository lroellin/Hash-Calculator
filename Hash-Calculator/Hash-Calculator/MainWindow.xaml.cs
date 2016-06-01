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
using Gat.Controls;

namespace Dreami.Hash_Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

		List<Task> tasks = new List<Task>();
		List<GUIRow> rows = new List<GUIRow>();
		Boolean tasksCompleted = true;
		Thickness thkNormal = new Thickness(1);
		Thickness thkThick = new Thickness(5, 1, 5, 1);

		public bool TasksCompleted
		{
			get
			{
				return tasksCompleted;
			}

			set
			{
				tasksCompleted = value;
			}
		}

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
				row.ProgressBar.IsIndeterminate = false;
			}
			setNormalBorder();
			prgMain.Maximum = 1;
			prgMain.Value = 0;
		}

		private void setNormalBorder()
		{
			foreach (GUIRow row in rows)
			{
				row.TextBox.ClearValue(TextBox.BorderThicknessProperty);
				row.TextBox.ClearValue(TextBox.BorderBrushProperty);
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
				AlertManager.calculationOngoing();
				return;
			}
			Stream stream = null;
			try
			{
				TasksCompleted = false;
				prgTaskbar.ProgressState = TaskbarItemProgressState.Indeterminate;
				tasks.Clear();
				btnSave.IsEnabled = true;
				lblStatus.Content = "Starting threads...";
				prgMain.Maximum = 0;
				prgMain.Value = 0;
				stream = File.OpenRead(txtFileOpen.Text);
				stream.Close();
				foreach (GUIRow row in rows)
				{
					if (row.isChecked())
					{
						row.ProgressBar.IsIndeterminate = true;
						row.TextBox.Text = "";
						prgMain.Maximum++;
						startTask(row);
					}
				}
				lblStatus.Content = "Working...";

				await Task.WhenAll(tasks);
			}
			catch (IOException error)
			{
				AlertManager.readException(error, txtFileOpen.Text);
			}
			finally
			{
				lblStatus.Content = "Done";
				prgTaskbar.ProgressState = TaskbarItemProgressState.None;
				TasksCompleted = true;
				prgMain.Maximum = 1;
				prgMain.Value = 0;
				if(stream != null)
				{
					stream.Close();
				}
				
			}
		}

		private void startTask(GUIRow row)
		{
			Hash hash = new Hash(row.HashAlgorithm, false);
			Task<Hash> task = Task.Run(() => HashCalculation.calculateHash(hash, txtFileOpen.Text));
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

		

		private bool checkTasksCompleted()
		{
			return TasksCompleted;
		}

		private void txtCompare_TextChanged(object sender, TextChangedEventArgs e)
		{
			setNormalBorder();
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

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			if (!checkTasksCompleted())
			{
				AlertManager.calculationOngoing();
				return;
			}
			String pthFileNoPath = Path.GetFileName(txtFileOpen.Text);
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.FileName = "Hash-" + pthFileNoPath + ".txt";
			saveFileDialog.Filter = "Text files (*.txt) | *.txt | All files(*.*) | *.*";
			saveFileDialog.ShowDialog();
			if (saveFileDialog.FileName != "")
			{
				HashFile hashFile = new HashFile(saveFileDialog.FileName);
				hashFile.addHeader(txtFileOpen.Text);
				foreach (GUIRow row in rows)
				{
					if (row.isChecked())
					{
						hashFile.addHashline(row.HashAlgorithm, row.TextBox.Text);
					}
				}
				hashFile.closeFile();
			}
		}


		private void btnAbout_Click(object sender, RoutedEventArgs e)
		{
			BitmapImage appBi = new BitmapImage(new System.Uri("pack://application:,,,/Resources/Icon.ico"));
			BitmapImage cBi = new BitmapImage(new System.Uri("pack://application:,,,/Resources/Dreami.png"));

			About about = new About();
			about.IsSemanticVersioning = true;
			about.ApplicationLogo = appBi;
			about.PublisherLogo = cBi;
			about.HyperlinkText = "http://www.dreami.ch/";
			about.AdditionalNotes = "Using:\n(Modified) WPF About Box\nCopyright (c) 2014 Christoph Gattnar";
			about.Show();

		}

		private void btnClose_Click(object sender, RoutedEventArgs e)
		{
			shutdown();
		}
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			shutdown();
		}

		private void shutdown()
		{
			Application.Current.Shutdown();
		}
	}
}
