using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace MVVM.MainView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowVM ViewModel => DataContext as MainWindowVM; // MyProperty { get; set; }
        List<Layer> layers = new List<Layer>();

        public MainWindow()
        {
            InitializeComponent();

            //Wanneer je het viewmodel wilt assignen in de "code behind"; nu heb je geen intellisense:
            //DataContext = new MainWindowVM();

            layers.Add(new Layer() {Name = "Een heel erg lange naam die niet helemaal in de lijst past"});
            layers.Add(new Layer() {Name = "Naam"});
            layers.Add(new Layer() {Name = "Naam"});
            layers.Add(new Layer() {Name = "Naam"});
            layers.Add(new Layer() {Name = "Naam"});
            layers.Add(new Layer() {Name = "Naam"});
            layers.Add(new Layer() {Name = "Naam"});
            lbLayers.ItemsSource = layers;
        }

        private void AddNewLayer()
        {
            layers.Add(new Layer() {Name = "New layer"});
        }


        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ConfigCommand.Execute(null);
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.GenerateCommand.Execute(null);
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }



    }
}
