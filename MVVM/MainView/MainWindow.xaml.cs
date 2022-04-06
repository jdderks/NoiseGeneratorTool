using MVVM.General;
using MVVM.Models;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MVVM.MainView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowVM ViewModel => DataContext as MainWindowVM; // MyProperty { get; set; }

        ObservableCollection<LayerVM> observableLayers = new ObservableCollection<LayerVM>();


        public MainWindow()
        {
            InitializeComponent();

            //observableLayers.Add(new Layer() {Name = "Een heel erg lange naam die niet helemaal in de lijst past" });
            //observableLayers.Add(new Layer() {Name = "Nieuwe layer" });
            //lbLayers.ItemsSource = observableLayers;

            //AddNewLayerCommand = new Command(AddnewLayer);
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


        private void RemoveLayer(object sender, RoutedEventArgs e)
        {
            LayerVM selectedLayer = layerListBox.SelectedItem as LayerVM;
            observableLayers.Remove(selectedLayer);
               
        }
    }
}
