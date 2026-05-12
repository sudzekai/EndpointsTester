using CommunityToolkit.Mvvm.ComponentModel;

namespace EndpointsTester.Models
{
    partial class Header : ObservableObject
    {
        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _value = string.Empty;
    }
}
