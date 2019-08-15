using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OxyPlot.Wpf;
using VMS.TPS.Common.Model.API;

namespace BackupCalcReport
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        // Create dummy PlotView to force OxyPlot.Wpf to be loaded
        private static readonly PlotView PlotView = new PlotView();

        private readonly MainViewModel _vm;

        public MainView(MainViewModel viewModel)
        {
            _vm = viewModel;

            InitializeComponent();
            DataContext = viewModel;
        }

        private void Structure_OnChecked(object checkBoxObject, RoutedEventArgs e)
        {
            _vm.AddDvhCurve(GetStructure(checkBoxObject));
        }

        private void Structure_OnUnchecked(object checkBoxObject, RoutedEventArgs e)
        {
            _vm.RemoveDvhCurve(GetStructure(checkBoxObject));
        }

        private Structure GetStructure(object checkBoxObject)
        {
            var checkbox = (CheckBox)checkBoxObject;
            var structure = (Structure)checkbox.DataContext;
            return structure;
        }

        private void ExportPlotAsPdf(object sender, RoutedEventArgs e)
        {
            var filePath = GetPdfSavePath();
            if (filePath != null)
                _vm.ExportPlotAsPdf(filePath);
        }

        private string GetPdfSavePath()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Export to PDF",
                Filter = "PDF Files (*.pdf)|*.pdf"
            };

            var dialogResult = saveFileDialog.ShowDialog();

            if (dialogResult == true)
                return saveFileDialog.FileName;
            else
                return null;
        }
    }
}
