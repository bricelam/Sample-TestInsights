using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using TestInsights.Data;

namespace TestInsights.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _search;
        private DateTime _start = DateTime.Today.AddMonths(-1);
        private DateTime _end = DateTime.Today;
        private object _selectedItem = "Hello, World!";
        private IEnumerable<Test> _tests = new ObservableCollection<Test>();

        public MainViewModel()
        {
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
            set { Set(() => Tests, ref _tests, value); }
        }

        public object SelectedItem
        {
            get { return _selectedItem; }
            set { Set(() => SelectedItem, ref _selectedItem, value); }
        }

        public ICommand RefreshCommand { get; }

        public void Refresh()
        {
            // TODO
        }
    }
}
