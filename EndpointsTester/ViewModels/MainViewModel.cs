using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EndpointsTester.Base;
using EndpointsTester.Models;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;

namespace EndpointsTester.ViewModels
{
    partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<string> _methods =
            [
                "GET",
                "POST",
                "PUT",
                "PATCH",
                "DELETE"
            ];

        [ObservableProperty]
        private string _selectedMethod = "GET";

        [ObservableProperty]
        private ObservableCollection<string> _urlsHistory = [];

        [ObservableProperty]
        private string _url = "http://localhost:8080/api";

        [ObservableProperty]
        private string _body = @"{
    ""username"": ""myCoolUsername"",
    ""password"": ""myVeryStrongPassword""
}";

        [ObservableProperty]
        private ObservableCollection<Header> _headers = [];

        [ObservableProperty]
        private string _response = "";

        [ObservableProperty]
        private System.Windows.Media.Brush _responseBrush = System.Windows.Media.Brushes.Black;

        [ObservableProperty]
        private string _responseBody = "";

        [ObservableProperty]
        private string _responseHeaders = "";

        [RelayCommand]
        private void AddHeader()
        {
            Headers.Add(new());
        }

        [RelayCommand]
        private void RemoveHeader(string name)
        {
            var header = Headers.FirstOrDefault(x => x.Name == name);
            if (header is not null)
                Headers.Remove(header);
        }

        [RelayCommand]
        private async Task SendRequest()
        {
            if (!UrlsHistory.Contains(Url))
            {
                if (UrlsHistory.Count == 7)
                    UrlsHistory.Remove(UrlsHistory.Last());
                UrlsHistory.Insert(0, Url);
            }

            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(5);

            HttpResponseMessage response = new();

            try
            {
                var content = new StringContent(Body, Encoding.UTF8, "application/json");
                response = SelectedMethod switch
                {
                    "GET" => await client.GetAsync(Url),
                    "POST" => await client.PostAsync(Url, content),
                    "PUT" => await client.PutAsync(Url, content),
                    "PATCH" => await client.PatchAsync(Url, content),
                    "DELETE" => await client.DeleteAsync(Url),
                    _ => await client.GetAsync(Url)
                };
            }
            catch (Exception ex)
            {
                Response = ex.Message;
                ResponseBrush = System.Windows.Media.Brushes.Red;
                return;
            }


            ResponseBody = await response.Content.ReadAsStringAsync();

            var allHeaders = response.Headers
                                     .Select(h => $"{h.Key}: {string.Join(", ", h.Value)}")
                                     .Concat(response.Content.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"));

            ResponseHeaders = string.Join(Environment.NewLine, allHeaders);

            Response = $"{(int)response.StatusCode} - {response.ReasonPhrase}";

            var code = ((int)response.StatusCode).ToString();

            if (code.StartsWith('2'))
                ResponseBrush = System.Windows.Media.Brushes.Green;
            else if (code.StartsWith('4'))
                ResponseBrush = System.Windows.Media.Brushes.Orange;
            else if (code.StartsWith('5'))
                ResponseBrush = System.Windows.Media.Brushes.Red;
            else
                ResponseBrush = System.Windows.Media.Brushes.Gray;
        }

        public MainViewModel()
        {
            Headers.Add(new() { Name = "Authorization", Value = "Bearer token..." });
            Headers.Add(new() { Name = "Content-Type", Value = "application/json" });
        }
    }
}
