﻿using Microsoft.Win32;
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
			chkHashCheck.ToolTip += Messages.infHashFileFormats;
			setInitialState();
		}


		private void setInitialState()
		{
			lblStatus.Content = "Ready";
			foreach (GUIRow row in rows)
			{
				row.CopyButton.Tag = row.TextBox;
				row.TextBox.Text = "";
				row.CopyButton.Click += new RoutedEventHandler(this.copyToClipboard);
				row.ProgressBar.IsIndeterminate = false;
				row.ProgressBar.Maximum = 1;
				if(row.HashCheckImage != null)
				{
					row.HashCheckImage.Source = new BitmapImage(new System.Uri("pack://application:,,,/Resources/Lock.png"));
					row.HashCheckImage.ClearValue(Image.ToolTipProperty);
				}
			}
			setNormalBorder();
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
			openFileDialog.ValidateNames = true;

            if(openFileDialog.ShowDialog() == true)
            {
				setInitialState();
                txtFileOpen.Text = openFileDialog.FileName;
				btnCalculate.IsEnabled = true;
			}
        }

		private async void btnCalculate_Click(object sender, RoutedEventArgs e)
		{
			if (!checkTasksCompleted())
			{
				Messages.calculationOngoing();
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
					if (row.isChecked())
					{
						row.TextBox.Text = "";
						startTask(row);
					}
				}
				lblStatus.Content = "Working...";

				// Await all tasks
				await TaskEx.WhenAll(tasks);
			}
			catch (Exception error)
			{
				Messages.readException(error);
			}
			finally
			{
				lblStatus.Content = "Calculation done";
				prgTaskbar.ProgressState = TaskbarItemProgressState.None;
				TasksCompleted = true;
				if(stream != null)
				{
					stream.Close();
				}
				
			}
		}

		private void startTask(GUIRow row)
		{
			DateTime start = DateTime.UtcNow;
			row.ProgressBar.IsIndeterminate = true;
			Hash hash = new Hash(row.HashAlgorithm, false);
			Task<Hash> task = Task.Factory.StartNew(() => HashCalculation.calculateHash(hash, file));
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
					 
					 row.ProgressBar.ToolTip = Math.Round(duration.TotalSeconds, 2) + "s (" + duration.ToString("g") + ")";
					 if(chkHashCheck.IsChecked == true && row.HashCheckImage != null)
					 {
						 try
						 {
							 if(HashCalculation.checkHash(t.Result, file))
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
				if (row.TextBox.Text.Equals(UserInput.normalize(txtCompare.Text))) 
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
				Messages.calculationOngoing();
				return;
			}
			MessageBoxResult result = Messages.saveConfirmation();
			if (result == MessageBoxResult.OK)
			{
				try
				{
					foreach (GUIRow row in rows)
					{
						if (row.isChecked())
						{
							HashFile.writeHashFile(row.Hash, file);
						}
					}
				}
				catch (Exception error)
				{
					Messages.saveException(error);
					lblStatus.Content = "Save aborted";
				}

				lblStatus.Content = "Save done";
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
