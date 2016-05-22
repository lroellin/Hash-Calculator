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

namespace Hash_Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

		string pthFile;
		bool hasRun = false;
		List<Task> tasks = new List<Task>();
        public MainWindow()
        {
            InitializeComponent();
			this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			setInitialContent();
			setInitialState();
		}

		private void setInitialContent()
		{

		}

		private void setInitialState()
		{
			lblStatus.Content = "Ready";
		}


        private void btnFileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.CheckFileExists = true;
			openFileDialog.CheckPathExists = true; 

            if(openFileDialog.ShowDialog() == true)
            {
                txtFileOpen.Text = openFileDialog.FileName;
            }
			btnCalculate.IsEnabled = true;
        }

		private void btnCalculate_Click(object sender, RoutedEventArgs e)
		{
			pthFile = txtFileOpen.Text;
			tasks.Clear();
			btnSave.IsEnabled = true;
			
			lblStatus.Content = "Starting threads...";
			if(chkMD5.IsChecked == true)
			{
				startTask(txtMD5, SupportedHashAlgorithm.MD5);
			}
			if (chkSHA1.IsChecked == true)
			{
				startTask(txtSHA1, SupportedHashAlgorithm.SHA1);
			}
			if (chkSHA256.IsChecked == true)
			{
				startTask(txtSHA256, SupportedHashAlgorithm.SHA256);
			}
			if (chkSHA384.IsChecked == true)
			{
				startTask(txtSHA384, SupportedHashAlgorithm.SHA384);
			}
			if (chkSHA512.IsChecked == true)
			{
				startTask(txtSHA512, SupportedHashAlgorithm.SHA512);
			}

			setInitialState();
		}

		private void startTask(TextBox textBox, SupportedHashAlgorithm hashAlgorithm)
		{
			setWorkingState(textBox);
			Hash hash = new Hash(hashAlgorithm, false);
			Task<Hash> task = Task.Run(() => HashCalculation.calculateHash(hash, pthFile));
			task.ContinueWith((t) =>
			{
				if(t.IsFaulted)
				{
					if(t.Exception.InnerException is HMACNotSupportedException)
					{
						textBox.Text = "HMAC not supported";
					}
				} else
				{
					textBox.Text = t.Result.HashString;
				}
			}, TaskScheduler.FromCurrentSynchronizationContext());
			tasks.Add(task);
			
		}

		private void setWorkingState(TextBox textBox)
		{
			textBox.Text = "Working...";
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			if(!checkTasksCompleted()) {
				MessageBox.Show("Calculation has not yet completed.", "Calculation ongoing", MessageBoxButton.OK, MessageBoxImage.Information);
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
				if (chkMD5.IsChecked == true)
				{
					hashFile.addHashline(SupportedHashAlgorithm.MD5, txtMD5.Text);
				}
				if (chkSHA1.IsChecked == true)
				{
					hashFile.addHashline(SupportedHashAlgorithm.SHA1, txtSHA1.Text);
				}
				if (chkSHA256.IsChecked == true)
				{
					hashFile.addHashline(SupportedHashAlgorithm.SHA256, txtSHA256.Text);
				}
				if (chkSHA384.IsChecked == true)
				{
					hashFile.addHashline(SupportedHashAlgorithm.SHA384, txtSHA384.Text);
				}
				if (chkSHA512.IsChecked == true)
				{
					hashFile.addHashline(SupportedHashAlgorithm.SHA512, txtSHA512.Text);
				}
				hashFile.closeFile();
			}
		}

		private bool checkTasksCompleted()
		{
			foreach (Task task in tasks)
			{
				if (task.Status != TaskStatus.RanToCompletion && task.Status != TaskStatus.Faulted)
				{
					return false;
				}
			}
			return true;
		}

	}
}
