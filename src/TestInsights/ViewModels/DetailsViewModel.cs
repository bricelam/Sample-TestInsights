using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using TestInsights.Models;

namespace TestInsights.ViewModels
{
    public class DetailsViewModel : ViewModelBase
    {
        private IEnumerable<TestResult> _results;
        private LineSeries _series;
        private LineSeries _sizeSeries;
        private PlotModel _plotModel;
        private Visibility _showGraph = Visibility.Hidden;

        public DetailsViewModel()
        {
            InitializePlotModel();
        }


        public IEnumerable<TestResult> Results
        {
            get { return _results; }
            set
            {
                Set(() => Results, ref _results, value);
                ShowGraph = Visibility.Visible;
                DrawGraph();
                UpdateErrors();
            }
        }

        public ICollection<TestError> Errors { get; } = new ObservableCollection<TestError>();

        private void UpdateErrors()
        {
            Errors.Clear();

            foreach (var result in _results)
            {
                var failedResult = result as TestFailedResult;
                if (failedResult != null)
                {
                    Errors.Add(
                        new TestError
                        {
                            StartDate = result.StartTime,
                            Error = failedResult.ExceptionType,
                            Message = failedResult.Message.Replace("\r\n", "\n").Replace("\n", ", "),
                            StackTrace = failedResult.StackTrace
                        });
                }
                else
                {
                    var skippedResult = result as TestSkippedResult;
                    if (skippedResult != null)
                    {
                        Errors.Add(
                            new TestError
                            {
                                StartDate = result.StartTime,
                                Error = "(Skipped)",
                                Message = skippedResult.Reason
                            });
                    }
                }
            }
        }

        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set { Set(() => PlotModel, ref _plotModel, value); }
        }


        public Visibility ShowGraph
        {
            get { return _showGraph; }
            set { Set(() => ShowGraph, ref _showGraph, value); }
        }

        private void DrawGraph()
        {
            _series.Points.Clear();
            _sizeSeries.Points.Clear();

            if (Results.Any())
            {
                var groupedResults = Results.GroupBy(r => r.StartTime);
                var index = 1;
                decimal upperLimit = 0;
                decimal lowerLimit = 0;

                foreach (var result in groupedResults)
                {
                    var executionTime = result.Sum(r => r.ExecutionTime);
                    upperLimit = Math.Max(executionTime, upperLimit);
                    lowerLimit = Math.Min(executionTime, upperLimit);
                    _series.Points.Add(new DataPoint(index, decimal.ToDouble(1000 * executionTime)));
                    index++;
                }

                _sizeSeries.Points.Add(new DataPoint(0, decimal.ToDouble(900 * upperLimit)));
                _sizeSeries.Points.Add(new DataPoint(index, decimal.ToDouble(900 * upperLimit)));
                _sizeSeries.Points.Add(new DataPoint(index, decimal.ToDouble(1100 * upperLimit)));
                PlotModel.InvalidatePlot(true);
            }
            else
            {
                ShowGraph = Visibility.Hidden;
            }

        }

        private void InitializePlotModel()
        {
            _plotModel = new PlotModel();

            var dateAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Test run",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                MajorStep = 1,
                MinorStep = 1
            };
            PlotModel.Axes.Add(dateAxis);

            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Title = "Execution time (in ms)"
            };
            PlotModel.Axes.Add(valueAxis);

            _series = new LineSeries
            {
                StrokeThickness = 1,
                MarkerSize = 5,
                MarkerType = MarkerType.Diamond,
                CanTrackerInterpolatePoints = false,
                Smooth = false,
            };

            _sizeSeries = new LineSeries
            {
                StrokeThickness = 0,
                MarkerSize = 0,
                CanTrackerInterpolatePoints = false,
                Smooth = false,
            };

            PlotModel.Series.Add(_series);
            PlotModel.Series.Add(_sizeSeries);
        }
    }
}
