using Microsoft.Practices.Prism.Mvvm;

namespace actionselect
{
    public class actionSelect : BindableBase
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}