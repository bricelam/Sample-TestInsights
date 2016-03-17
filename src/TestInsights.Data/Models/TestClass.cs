using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace TestInsights.Models
{
    public class TestClass : ObservableObject
    {
        private TestAssembly _assembly;
        private string _name;

        public TestAssembly Assembly
        {
            get { return _assembly; }
            set { Set(nameof(Assembly), ref _assembly, value); }
        }

        public string Name
        {
            get { return _name; }
            set { Set(nameof(Name), ref _name, value); }
        }

        public ICollection<Test> Tests { get; } = new ObservableCollection<Test>();
    }
}
