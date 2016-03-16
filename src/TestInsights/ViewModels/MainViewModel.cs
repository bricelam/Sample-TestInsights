using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.EntityFrameworkCore;
using TestInsights.Data;
using TestInsights.Models;

namespace TestInsights.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _search;
        private DateTime _start = DateTime.Today.AddMonths(-1);
        private DateTime _end = DateTime.Today;
        private Test _selectedItem;
        private ICollection<Test> _tests = new ObservableCollection<Test>();

        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                _tests.Add(new Test { Name = "Test1" });
                _tests.Add(new Test { Name = "Test2" });
                _tests.Add(new Test { Name = "Test3" });
            }

            RefreshCommand = new RelayCommand(Refresh);
        }

        public string Search
        {
            get { return _search; }
            set { Set(() => Search, ref _search, value); }
        }

        public DateTime Start
        {
            get { return _start; }
            set { Set(() => Start, ref _start, value); }
        }

        public DateTime End
        {
            get { return _end; }
            set { Set(() => End, ref _end, value); }
        }

        public IEnumerable<Test> Tests
        {
            get { return _tests; }
        }

        public Test SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                Set(() => SelectedItem, ref _selectedItem, value);
            }
        }

        public ICommand RefreshCommand { get; }

        public void Refresh()
        {
            using (var db = new InsightContext())
            {
                var testResults =
                    from r in db.TestResults.AsNoTracking()
                    where r.StartTime >= Start
                        && r.StartTime < End
                    group r by new { r.Assembly, r.Class, r.Name } into g
                    select new Test
                    {
                        Assembly = g.Key.Assembly,
                        Class = g.Key.Class,
                        Name = g.Key.Name,
                        TestResults = g.ToArray()
                    };

                _tests.Clear();
                foreach (var testResult in testResults)
                {
                    _tests.Add(testResult);
                }
            }
        }
    }
}
