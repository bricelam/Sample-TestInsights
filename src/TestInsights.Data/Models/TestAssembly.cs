using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using GalaSoft.MvvmLight;

namespace TestInsights.Models
{
    public class TestAssembly : ObservableObject
    {
        private string _name;

        [Key]
        public string Name
        {
            get { return _name; }
            set { Set(nameof(Name), ref _name, value); }
        }

        public ICollection<TestClass> Classes { get; } = new ObservableCollection<TestClass>();
    }
}
