using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using Series = OxyPlot.Series.Series;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using BackupCalcReport.Helpers;
using BackupCalcReport.Data;

namespace BackupCalcReport.ViewModels
{
    public class MainViewModel : ViewModelBase
    {

        private readonly Patient _patient;
        //private readonly IEnumerable<PlanSetup> _planSetupsInScope;
        private readonly PlanSetup _plan;



        private IEnumerable<PlanningItemViewModel> _planningItems;
        public IEnumerable<PlanningItemViewModel> PlanningItems
        {
            get { return _planningItems; }
            private set { Set(ref _planningItems, value); }
        }

        private ObservableCollection<PlanningItemViewModel> _planSetupsInScope;
        public ObservableCollection<PlanningItemViewModel> PlanSetupsInScope
        {


            get { return _planSetupsInScope; }
            private set { Set(ref _planSetupsInScope, value); }
        }

        private PlanningItemViewModel _selectedPlanSetup;
        public PlanningItemViewModel SelectedPlanSetup
        {
            get { return _selectedPlanSetup; }
            set { Set(ref _selectedPlanSetup, value); }
        }






        public MainViewModel(Patient patient, IEnumerable<PlanSetup> planSetupsInScope, PlanSetup plan)  //later remove planSetup
        {

            _patient = patient;
            //_planSetupsInScope = planSetupsInScope;
            _plan = plan;

            

            Structures = GetPlanStructures();
            PlotModel = CreatePlotModel();

        }











        //DVH creation
        public IEnumerable<Structure> Structures { get; private set; }

        public PlotModel PlotModel { get; private set; }

        public void AddDvhCurve(Structure structure)
        {
            var dvh = CalculateDvh(structure);
            PlotModel.Series.Add(CreateDvhSeries(structure.Id, dvh));
            UpdatePlot();
        }

        public void RemoveDvhCurve(Structure structure)
        {
            var series = FindSeries(structure.Id);
            PlotModel.Series.Remove(series);
            UpdatePlot();
        }

        public void ExportPlotAsPdf(string filePath)
        {
            using (var stream = File.Create(filePath))
            {
                PdfExporter.Export(PlotModel, stream, 600, 400);
            }
        }

        private IEnumerable<Structure> GetPlanStructures()
        {
            return _plan.StructureSet != null
                ? _plan.StructureSet.Structures.Where(Structure => !Structure.IsEmpty)
                : null;
        }

        private PlotModel CreatePlotModel()
        {
            var plotModel = new PlotModel();
            AddAxes(plotModel);
            return plotModel;
        }

        private static void AddAxes(PlotModel plotModel)
        {
            plotModel.Axes.Add(new LinearAxis
            {
                Title = "Dose [cGy]",
                Position = AxisPosition.Bottom
            });

            plotModel.Axes.Add(new LinearAxis
            {
                Title = "Volume [%]",
                Position = AxisPosition.Left
            });
        }

        private DVHData CalculateDvh(Structure structure)
        {
            return _plan.GetDVHCumulativeData(structure,
                DoseValuePresentation.Absolute,
                VolumePresentation.Relative, 0.1);
        }

        private Series CreateDvhSeries(string structureId, DVHData dvh)
        {
            var series = new LineSeries { Tag = structureId };
            var points = dvh.CurveData.Select(CreateDataPoint);
            series.Points.AddRange(points);
            return series;
        }

        private DataPoint CreateDataPoint(DVHPoint p)
        {
            return new DataPoint(p.DoseValue.Dose, p.Volume);
        }

        private Series FindSeries(string structureId)
        {
            return PlotModel.Series.FirstOrDefault(x =>
                (string)x.Tag == structureId);
        }

        private void UpdatePlot()
        {
            PlotModel.InvalidatePlot(true);
        }
    }
}
