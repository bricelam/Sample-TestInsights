using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using Microsoft.EntityFrameworkCore;
using TestInsights.Data;
using TestInsights.Models;

namespace TestInsights.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _search;
        private DateTime _start = DateTime.Today.AddMonths(-1);
        private DateTime _end = DateTime.Today.AddDays(1);
        private Test _selectedItem;
        private Test[] _tests;
        private ICollection<Test> _filteredTests = new ObservableCollection<Test>();
        private DetailsViewModel _detailsViewModel = new DetailsViewModel();

        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                _filteredTests.Add(new Test { Name = "Test1" });
                _filteredTests.Add(new Test { Name = "Test2" });
                _filteredTests.Add(new Test { Name = "Test3" });
            }
            else
            {
                QueryTests();
            }
        }

        public string Search
        {
            get { return _search; }
            set
            {
                if (Set(() => Search, ref _search, value))
                {
                    FilterTests();
                }
            }
        }

        public DateTime Start
        {
            get { return _start; }
            set
            {
                if (Set(() => Start, ref _start, value))
                {
                    QueryTests();
                }
            }
        }

        public DateTime End
        {
            get { return _end; }
            set
            {
                if (Set(() => End, ref _end, value))
                {
                    QueryTests();
                }
            }
        }

        public IEnumerable<Test> Tests
        {
            get { return _filteredTests; }
        }

        public Test SelectedTest
        {
            get { return _selectedItem; }
            set
            {
                if (Set(() => SelectedTest, ref _selectedItem, value))
                {
                    QueryDetails();
                }
            }
        }

        public DetailsViewModel DetailsViewModel
        {
            get { return _detailsViewModel; }
            set { Set(() => DetailsViewModel, ref _detailsViewModel, value); }
        }

        private void QueryTests()
        {
            using (var db = new InsightContext())
            {
                db.Database.EnsureCreated();

                _tests = Enumerable.ToArray(
                    from t in db.Tests.AsNoTracking()
                    where t.Results.Any(r => r.StartTime >= Start && r.StartTime <= End)
                    select t);
            }

            FilterTests();
        }

        private void FilterTests()
        {
            _filteredTests.Clear();
            _filteredTests.AddRange(
                string.IsNullOrEmpty(Search)
                    ? _tests
                    : _tests.Where(t => t.Name.ToUpper().Contains(Search.ToUpper())));
        }

        private void QueryDetails()
        {
            if (SelectedTest == null)
            {
                _detailsViewModel.Results = Enumerable.Empty<TestResult>();

                return;
            }

            using (var db = new InsightContext())
            {
                _detailsViewModel.Results = Enumerable.ToArray(
                from r in db.Results.AsNoTracking()
                where
                    // UNDONE: r.Test == SelectedTest
                    r.Test.Assembly == SelectedTest.Assembly && r.Test.Class == SelectedTest.Class && r.Test.Name == SelectedTest.Name
                    && r.StartTime >= Start
                    && r.StartTime <= End
                select r);
            }
        }
    }
}
