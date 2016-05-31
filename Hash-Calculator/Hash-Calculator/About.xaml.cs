using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Hash_Calculator
{
	/// <summary>
	/// Interaction logic for About.xaml
	/// </summary>
	public partial class About : Window
	{
		public About()
		{
			InitializeComponent();
			this.Loaded += new RoutedEventHandler(About_Loaded);
		}

		private void About_Loaded(object sender, RoutedEventArgs e)
		{
			Assembly app = Assembly.GetExecutingAssembly();
			String version = app.GetName().Version.ToString();
			AssemblyTitleAttribute title = (AssemblyTitleAttribute)app.GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0];
			AssemblyProductAttribute product = (AssemblyProductAttribute)app.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0];
			AssemblyCopyrightAttribute copyright = (AssemblyCopyrightAttribute)app.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0];
			AssemblyCompanyAttribute company = (AssemblyCompanyAttribute)app.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false)[0];
			AssemblyDescriptionAttribute description = (AssemblyDescriptionAttribute)app.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0];

			txtTitle.Content = title.Title.ToString();
			txtVersion.Content = version;
		}
	}
}
