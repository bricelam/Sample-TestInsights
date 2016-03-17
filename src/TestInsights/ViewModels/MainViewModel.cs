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
        private ICollection<TestAssembly> _assemblies = new List<TestAssembly>();
        private DetailsViewModel _detailsViewModel = new DetailsViewModel();

        public MainViewModel()
        {
            SelectItemCommand = new RelayCommand<object>(SelectItem);

            if (!IsInDesignMode)
            {
                Query();
            }
        }

        public ICommand SelectItemCommand { get; }

        public string Search
        {
            get { return _search; }
            set
            {
                if (Set(() => Search, ref _search, value))
                {
                    Filter();
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
                    Query();
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
                    Query();
                }
            }
        }

        public ICollection<TestAssembly> Assemblies { get; } = new ObservableCollection<TestAssembly>();

        public DetailsViewModel DetailsViewModel
        {
            get { return _detailsViewModel; }
            set { Set(() => DetailsViewModel, ref _detailsViewModel, value); }
        }

        public void SelectItem(object item)
        {
            var results = Enumerable.Empty<TestResult>();

            if (item != null)
            {
                using (var db = new InsightContext())
                {
                    var end = End.AddDays(1);
                    var test = item as Test;
                    if (test != null)
                    {
                        results = Enumerable.ToArray(
                            from r in db.Results
                            where r.Test.Class.Assembly.Name == test.Class.Assembly.Name
                                && r.Test.Class.Name == test.Class.Name
                                && r.Test.Name == test.Name
                                && r.StartTime >= Start
                                && r.StartTime < end
                            select r);
                    }
                    else
                    {
                        var testClass = item as TestClass;
                        if (testClass != null)
                        {
                            results = Enumerable.ToArray(
                                from r in db.Results
                                where r.Test.Class.Assembly.Name == testClass.Assembly.Name
                                    && r.Test.Class.Name == testClass.Name
                                    && r.StartTime >= Start
                                    && r.StartTime < end
                                select r);
                        }
                        else
                        {
                            var assembly = item as TestAssembly;
                            if (assembly != null)
                            {
                                results = Enumerable.ToArray(
                                    from r in db.Results
                                    where r.Test.Class.Assembly.Name == assembly.Name
                                        && r.StartTime >= Start
                                        && r.StartTime < end
                                    select r);
                            }
                        }
                    }
                }
            }

            _detailsViewModel.Results = results;
        }

        private void Query()
        {
            _assemblies.Clear();

            using (var db = new InsightContext())
            {
                db.Database.EnsureCreated();

                var end = _end.AddDays(1);
                var tests = Enumerable.ToList(
                    from t in db.Tests.Include(t => t.Class.Assembly)
                    where t.Results.Any(r => r.StartTime >= Start && r.StartTime < end)
                    select t);
                _assemblies.AddRange(tests.Select(t => t.Class.Assembly).Distinct());
            }

            Filter();
        }

        private void Filter()
        {
            Assemblies.Clear();

            if (string.IsNullOrEmpty(Search))
            {
                Assemblies.AddRange(_assemblies);
                return;
            }

            var search = Search.ToUpper();

            foreach (var assembly in _assemblies)
            {
                if (assembly.Name.ToUpper().Contains(search))
                {
                    Assemblies.Add(assembly);

                    continue;
                }

                TestAssembly filteredAssembly = null;
                foreach (var testClass in assembly.Classes)
                {
                    if (testClass.Name.ToUpper().Contains(search))
                    {
                        if (filteredAssembly == null)
                        {
                            filteredAssembly = new TestAssembly { Name = assembly.Name };
                            Assemblies.Add(filteredAssembly);
                        }

                        filteredAssembly.Classes.Add(testClass);

                        continue;
                    }

                    TestClass filteredClass = null;
                    foreach (var test in testClass.Tests)
                    {
                        if (!test.Name.ToUpper().Contains(search))
                        {
                            continue;
                        }

                        if (filteredAssembly == null)
                        {
                            filteredAssembly = new TestAssembly { Name = assembly.Name };
                            Assemblies.Add(filteredAssembly);
                        }

                        if (filteredClass == null)
                        {
                            filteredClass = new TestClass { Assembly = assembly, Name = testClass.Name };
                            filteredAssembly.Classes.Add(filteredClass);
                        }

                        filteredClass.Tests.Add(test);
                    }
                }
            }
        }
    }
}
