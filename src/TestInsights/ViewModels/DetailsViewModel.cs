using System.Collections.Generic;
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
            set { Set(() => Results, ref _results, value); }
        }
    }
}
