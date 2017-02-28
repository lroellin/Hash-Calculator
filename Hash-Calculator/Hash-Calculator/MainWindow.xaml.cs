using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Shell;
using Gat.Controls;
using System.Media;

namespace Dreami.Hash_Calculator
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
		String file;
		List<Task> tasks = new List<Task>();
		List<GUIRow> rows = new List<GUIRow>();
		Boolean tasksCompleted = true;
		Boolean firstRunCompleted = false;
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
			rows.Add(new GUIRow(SupportedHashAlgorithm.MD5, chkMD5, txtMD5, cpyMD5, prgMD5, imgHCMD5));
			rows.Add(new GUIRow(SupportedHashAlgorithm.SHA1, chkSHA1, txtSHA1, cpySHA1, prgSHA1, imgHCSHA1));
			rows.Add(new GUIRow(SupportedHashAlgorithm.SHA256, chkSHA256, txtSHA256, cpySHA256, prgSHA256, imgHCSHA256));
			rows.Add(new GUIRow(SupportedHashAlgorithm.SHA384, chkSHA384, txtSHA384, cpySHA384, prgSHA384, imgHCSHA384));
			rows.Add(new GUIRow(SupportedHashAlgorithm.SHA512, chkSHA512, txtSHA512, cpySHA512, prgSHA512, imgHCSHA512));
			rows.Add(new GUIRow(SupportedHashAlgorithm.RIPEMD160, chkRIPEMD160, txtRIPEMD160, cpyRIPEMD160, prgRIPEMD160, imgHCRIPEMD160));
			chkHashCheck.ToolTip += Messages.HASHFILE_FILENAMES;
			// imgCompareTip.ToolTip += Messages.USERINPUT_NORMALIZATION;
			SetInitialState();
		}


		private void SetInitialState()
		{
			lblStatus.Content = "Ready";
			foreach (GUIRow row in rows)
			{
				row.CopyButton.Tag = row.TextBox;
				row.TextBox.Text = "";
				row.CopyButton.Click += new RoutedEventHandler(this.CopyToClipboard);
				row.ProgressBar.IsIndeterminate = false;
				row.ProgressBar.Maximum = 1;
				if(row.HashCheckImage != null)
				{
					row.HashCheckImage.Source = new BitmapImage(new System.Uri("pack://application:,,,/Resources/Lock.png"));
					row.HashCheckImage.ClearValue(Image.ToolTipProperty);
				}
			}
			SetNormalBorder();
		}

		private void SetNormalBorder()
		{
			foreach (GUIRow row in rows)
			{
				row.TextBox.ClearValue(TextBox.BorderThicknessProperty);
				row.TextBox.ClearValue(TextBox.BorderBrushProperty);
			}
		}

		private void CopyToClipboard(object sender, RoutedEventArgs e)
		{
			TextBox textBox = (TextBox)((Button)sender).Tag;
			Clipboard.SetText(textBox.Text);
		}

		private void btnFileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.CheckFileExists = true;
			openFileDialog.CheckPathExists = true;
			openFileDialog.ValidateNames = true;

            if(openFileDialog.ShowDialog() == true)
            {
				SetInitialState();
                txtFileOpen.Text = openFileDialog.FileName;
				btnCalculate.IsEnabled = true;
			}
        }

		private async void btnCalculate_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckTasksCompleted())
			{
				Messages.CalculationOngoing();
				return;
			}
			// Initialization
			Stream stream = null;
			TasksCompleted = false;
			file = txtFileOpen.Text;
			prgTaskbar.ProgressState = TaskbarItemProgressState.Indeterminate;
			tasks.Clear();
			// GUI Tasks
			btnSave.IsEnabled = true;
			lblStatus.Content = "Starting threads...";
			try
			{
				// Check if stream can be opened
				stream = File.OpenRead(txtFileOpen.Text);
				stream.Close();
				// Start task for each row
				foreach (GUIRow row in rows)
				{
					if (row.IsChecked())
					{
						row.TextBox.Text = "";
						StartTask(row);
					}
				}
				lblStatus.Content = "Working...";

				// Await all tasks
				await TaskEx.WhenAll(tasks);
				// CheckCompare();
				if (Properties.Settings.Default.PlaySoundWhenDone)
				{
					SystemSounds.Asterisk.Play();
				}
			}
			catch (Exception error)
			{
				Messages.ReadException(error);
			}
			finally
			{
				lblStatus.Content = "Calculation done";
				prgTaskbar.ProgressState = TaskbarItemProgressState.None;
				TasksCompleted = true;
				firstRunCompleted = true;
				if (stream != null)
				{
					stream.Close();
				}
				
			}
		}

		private void StartTask(GUIRow row)
		{
			DateTime start = DateTime.UtcNow;
			row.ProgressBar.IsIndeterminate = true;
			Hash hash = new Hash(row.HashAlgorithm, false);
			Task<Hash> task = Task.Factory.StartNew(() => HashCalculation.CalculateHash(hash, file));
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
					 DateTime end = DateTime.UtcNow;
					 row.TextBox.Text = t.Result.HashString;
					 row.Hash = t.Result;
					 row.ProgressBar.IsIndeterminate = false;
					 row.ProgressBar.Maximum = 0;
					 TimeSpan duration = end - start;

					 row.ProgressBar.ToolTip = Messages.RuntimeDisplay(duration);
					 if(chkHashCheck.IsChecked == true && row.HashCheckImage != null)
					 {
						 try
						 {
							 if(HashCalculation.CheckHash(t.Result, file))
							 {
								 row.HashCheckImage.Source = new BitmapImage(new System.Uri("pack://application:,,,/Resources/LockCorrect.png"));
								 row.HashCheckImage.ToolTip = "Correct";
							 } else
							 {
								 row.HashCheckImage.Source = new BitmapImage(new System.Uri("pack://application:,,,/Resources/LockIncorrect.png"));
								 row.HashCheckImage.ToolTip = "Incorrect";
							 }
						 }
						 catch (HashCheckException e)
						 {
							 row.HashCheckImage.Source = new BitmapImage(new System.Uri("pack://application:,,,/Resources/LockUnknown.png"));
							 row.HashCheckImage.ToolTip = e.Message;
						 }
						 finally
						 {

						 }
					 }
				 }
			 }, TaskScheduler.FromCurrentSynchronizationContext());
			tasks.Add(task);			
		}

		private bool CheckTasksCompleted()
		{
			return TasksCompleted;
		}

		private void txtCompare_TextChanged(object sender, TextChangedEventArgs e)
		{
			if(firstRunCompleted)
			{
				SetNormalBorder();
				CheckCompare();
			}			
		}

		private void CheckCompare()
		{
			// Search through results
			foreach (GUIRow row in rows)
			{
				// If found
				if (row.TextBox.Text.Equals(UserInput.Normalize(txtCompare.Text)))
				{
					row.TextBox.BorderThickness = thkThick;
					Console.WriteLine(row.TextBox.Text + "=" + UserInput.Normalize(txtCompare.Text));
					row.TextBox.BorderBrush = Brushes.Green;
				}
			}
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckTasksCompleted())
			{
				Messages.CalculationOngoing();
				return;
			}
			MessageBoxResult result = Messages.SaveConfirmation();
			if (result == MessageBoxResult.OK)
			{
				try
				{
					foreach (GUIRow row in rows)
					{
						if (row.IsChecked())
						{
							HashFile.WriteHashFile(row.Hash, file);
						}
					}
					lblStatus.Content = "Save done";
				}
				catch (Exception error)
				{
					Messages.SaveException(error);
					lblStatus.Content = "Save aborted";
				}

			} else
			{
				lblStatus.Content = "Save canceled";
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
			about.AdditionalNotes = "Feedback:\nhash-calculator@dreami.ch\nUsing:\n- (Modified) WPF About Box\nCopyright (c) 2014 Christoph Gattnar\n- (Modified) VPN Icon\nby AWS Simple Icons";
			about.Show();

		}

		private void btnClose_Click(object sender, RoutedEventArgs e)
		{
			Shutdown();
		}
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Shutdown();
		}

		private void btnSettings_Click(object sender, RoutedEventArgs e)
		{
			Window settings = new Settings();
			settings.Owner = this;
			settings.ShowDialog();
		}

		private void Shutdown()
		{
			Application.Current.Shutdown();
		}
	}
}
