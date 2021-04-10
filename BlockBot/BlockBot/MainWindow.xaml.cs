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
using System.Windows.Shapes;

namespace BlockBot {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public List<FunctionsType> functionsList = new List<FunctionsType>();
        public List<FunctionBlock> codeBlocks = new List<FunctionBlock>();
        public MainWindow() {
            InitializeComponent();            


            FunctionsType Basic = new FunctionsType() { Title = "Basic" };
            Basic.functionsList.Add(new FunctionBlock { Name = "Move" });
            Basic.functionsList.Add(new FunctionBlock { Name = "Open" });
            Basic.functionsList.Add(new FunctionBlock { Name = "Close" });
            functionsList.Add(Basic);

            FunctionsType Advanced = new FunctionsType() { Title = "Advanced" };
            Advanced.functionsList.Add(new FunctionBlock { Name = "Continous Move" });
            Advanced.functionsList.Add(new FunctionBlock { Name = "Grid Placing" });
            functionsList.Add(Advanced);

            FunctionsTreeView.ItemsSource = functionsList;

            codeBlocks.Add(new FunctionBlock { Name = "zc" });
        }

        private void Btn_New_Click(object sender, RoutedEventArgs e) {

        }

        private void Btn_Open_Click(object sender, RoutedEventArgs e) {

        }

        private void Btn_Save_Click(object sender, RoutedEventArgs e) {

        }

        private void Btn_SaveAs_Click(object sender, RoutedEventArgs e) {

        }

        private void Btn_Exit_Click(object sender, RoutedEventArgs e) {
            
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e) {

        }
    }

    public class FunctionsType {
        public string Title { get; set; }
        public List<FunctionBlock> functionsList { get; set; }

        public FunctionsType() {
            this.functionsList = new List<FunctionBlock>();
        }
    }

    public class FunctionBlock {
        public string Name { get; set; }
        public List<FunctionBlock> Functions { get; set; }

        public FunctionBlock() {
            Functions = new List<FunctionBlock>();
        }
    }
}
