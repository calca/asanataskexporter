using Asana_Exporter_Lib.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Shapes;

namespace Asana_Exporter_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Query Query { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            
            var apiKey = ConfigurationManager.AppSettings["apiKey"];
            Query = new Query(apiKey);

            listProjects.DataContext = Query.GetProjects().data;
        }

        private void listProjects_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            var item = (Project)listProjects.SelectedItem;
            listTask.DataContext = Query.GetTasks(item).data;
        }

    }
}
