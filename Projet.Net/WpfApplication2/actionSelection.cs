using Microsoft.Practices.Prism.Mvvm;

namespace actionselection
{
    public class actionSelection : BindableBase
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}