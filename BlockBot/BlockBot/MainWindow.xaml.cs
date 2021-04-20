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
using System.Diagnostics;
using System.IO;

namespace BlockBot {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();

            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            string[] basicFunctions = { "move", "open", "close" };
            string[] advancedFunctions = { "grid place", "dance" };
            string[] loopFunctions = { "for", "while" };

            TreeViewItem basicFunctionsItem = new TreeViewItem { Header = "Basic" };
            foreach(string fcnName in basicFunctions) {
                basicFunctionsItem.Items.Add(new TreeViewFunction { Header=fcnName});
            }
            FunctionsTreeView.Items.Add(basicFunctionsItem);

            TreeViewItem advancedFunctionsItem = new TreeViewItem { Header = "Advanced" };
            foreach(string fcnName in advancedFunctions) {
                advancedFunctionsItem.Items.Add(new TreeViewFunction { Header = fcnName });
            }
            FunctionsTreeView.Items.Add(advancedFunctionsItem);

            TreeViewItem loopFunctionsItem = new TreeViewItem { Header = "Loop" };
            foreach(string fcnName in loopFunctions) {
                loopFunctionsItem.Items.Add(new TreeViewFunction { Header = fcnName });
            }
            FunctionsTreeView.Items.Add(loopFunctionsItem);


            ///////////CODE
            CodeTreeView.Items.Add(new TreeViewFunction { Header = "Test1" });
            CodeTreeView.Items.Add(new TreeViewFunction { Header = "Test2" });

            TreeViewFunction loopTest = new TreeViewFunction { Header = "Loop test" };
            loopTest.Items.Add(new TreeViewFunction { Header = "Loop1" });
            loopTest.Items.Add(new TreeViewFunction { Header = "Loop2" });
            loopTest.Items.Add(new TreeViewFunction { Header = "Loop3" });

            CodeTreeView.Items.Add(loopTest);
        }
        private void LoadProperties() {

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
    public class TreeViewFunction : TreeViewItem {
        public TreeViewFunction InsideFunctions { get; set; }
    }
}
