using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using TestInsights.Models;

namespace TestInsights.ViewModels
{
    public class DetailsViewModel : ViewModelBase
    {
        private IEnumerable<TestResult> _results;

        public IEnumerable<TestResult> Results
        {
            get { return _results; }
            set
            {
                Set(() => Results, ref _results, value);
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
    }
}
