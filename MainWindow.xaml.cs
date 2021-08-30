using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace dotnet_calculator
{
    class ReturnPackage
    {
        public string Calculation { get; set; } 
    }

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        void ButtonHandle(object sender, RoutedEventArgs e) => LabelDisplay = (string)(sender as Button).Content;
        bool CanInvokeLambda() => new Regex(@"([-+]?[0-9]*\.?[0-9]+[\/\+\-\*])+([-+]?[0-9]*\.?[0-9]+)").Match(LabelDisplay).Success;
        AmazonLambdaClient client = new AmazonLambdaClient("AKIATZFPJHG4R7DYRIX6", "MxGVeU1aHdZcaZ9fE7BxoYo9dTf0kDHFla7hrMdb", RegionEndpoint.USWest2);

        string _content;
        public string LabelDisplay
        {
            get { return _content; }
            set
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(_content);
                sb.Append(value);
                _content = sb.ToString();
                OnPropertyChanged("LabelDisplay");
            }
        }

        void ClearDisplay(object sender = null, RoutedEventArgs e = null)
        {
            _content = String.Empty;
            OnPropertyChanged("LabelDisplay");
        }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
            char[] specialCharacters ={
                '.',
                '-',
                '+',
                '/',
                '*'
            };

            //To handle WPF button content we need to use ascii, to convert int to character byte, 48 = 0 and ending at 58 = 9
            for (int i = 48; i < 58; i++)
            {
                CreateButton((char)(i), new RoutedEventHandler(ButtonHandle));
            }

            foreach (var item in specialCharacters)
            {
                CreateButton(item, new RoutedEventHandler(ButtonHandle));
            }

            CreateButton('=', new RoutedEventHandler(InvokeLambda));
            CreateButton('C', new RoutedEventHandler(ClearDisplay));
        }

        //Creating buttons at runtime to keep it dynamic.
        void CreateButton(char c, RoutedEventHandler eventHandler)
        {
            var button = new Button
            {
                Content     = c.ToString(),
                Height      = 100,
                FontSize    = 30
            };
       
            button.Click += eventHandler;
            grid.Children.Add(button);
        }

        void InvokeLambda(object sender, RoutedEventArgs e)
        {
            if (!CanInvokeLambda())
            {
                ClearDisplay();
                LabelDisplay = "err";
                return;
            }

            InvokeRequest invokeRequest = new InvokeRequest
            {
                FunctionName = "calculator",
                InvocationType = InvocationType.RequestResponse,
                Payload = $"\"{LabelDisplay}\""
            };

            var response = client.Invoke(invokeRequest);
            var streamr = new StreamReader(response.Payload);
            var jsonTextReader = new JsonTextReader(streamr);
            var jsonSerializer = new JsonSerializer().Deserialize(jsonTextReader);

            ClearDisplay();
            LabelDisplay = JsonConvert.DeserializeObject<ReturnPackage>(jsonSerializer.ToString()).Calculation;
        }
    }
}
