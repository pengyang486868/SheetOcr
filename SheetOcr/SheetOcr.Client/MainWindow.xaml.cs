using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serialization.Json;

namespace SheetOcr.Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            InitData();
        }

        private void InitData()
        {
            _imgPath = @"D:\pic\firstpic.jpg";
            _points = new List<double>();

            _rowsNumber = 10;
        }

        private string _imgPath;
        private double _sourceWidth;
        private double _sourceHeight;
        private double _displayWidth;
        private double _displayHeight;
        private double _scaleRate;
        private List<double> _points;

        private int _rowsNumber;
        public int RowsNumber
        {
            get => _rowsNumber; set
            {
                _rowsNumber = value;
                OnPropertyChanged();
            }
        }

        private int _colsNumber;
        public int ColsNumber
        {
            get => _colsNumber; set
            {
                _colsNumber = value;
                OnPropertyChanged();
            }
        }

        private int _strokeNumber;
        public int StrokeNumber
        {
            get => _strokeNumber; set
            {
                _strokeNumber = value;
                OnPropertyChanged();
            }
        }

        private void OpenButton_OnClick(object sender, RoutedEventArgs e)
        {
            var img = new BitmapImage(new Uri(_imgPath));
            MainImage.Source = img;

            UpdateLayout();

            _sourceHeight = img.Height;
            _sourceWidth = img.Width;
            _displayHeight = MainImage.ActualHeight;
            _displayWidth = MainImage.ActualWidth;

            _scaleRate = _sourceHeight / _displayHeight;
            //var compare = _sourceWidth / _displayWidth;
        }

        private void ExecuteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var client = new RestClient("http://localhost:32454/");
            // client.Authenticator = new HttpBasicAuthenticator(username, password);

            var request = new RestRequest("ocr", Method.POST);
            request.AddHeader("Content-type", "application/json");

            //var arr = new[] { 700, 326, 1409, 354, 1401, 778, 694, 741 };
            var arr = _points.ToArray();

            //request.AddParameter("w1", arr[0]);
            //request.AddParameter("w2", arr[2]);
            //request.AddParameter("w3", arr[4]);
            //request.AddParameter("w4", arr[6]);
            //request.AddParameter("h1", arr[1]);
            //request.AddParameter("h2", arr[3]);
            //request.AddParameter("h3", arr[5]);
            //request.AddParameter("h4", arr[7]);
            //request.AddParameter("rows", _rowsNumber);
            //request.AddParameter("cols", _colsNumber);
            //request.AddParameter("stroke", _strokeNumber);
            //request.AddParameter("path", _imgPath);

            var splitNumbers = new List<string>();
            //for (var i = 0; i < 14; i++)
            //    splitNumbers.Add(2);
            //splitNumbers[6] = 3;
            //splitNumbers[7] = 3;
            //splitNumbers[12] = 3;
            //splitNumbers[13] = 4;

            FormatConfig fconfig;
            using (var text = System.IO.File.OpenText("formats.json"))
            {
                fconfig = JsonConvert.DeserializeObject<FormatConfig>(text.ReadToEnd());
            }

            var fdic = fconfig.AssignDic;
            for (var i = 0; i < _rowsNumber; i++)
            {
                splitNumbers.Add(fdic.ContainsKey(i) ? fdic[i] : fconfig.Default);
            }

            request.AddJsonBody(new
            {
                w1 = arr[0],
                w2 = arr[2],
                w3 = arr[4],
                w4 = arr[6],
                h1 = arr[1],
                h2 = arr[3],
                h3 = arr[5],
                h4 = arr[7],
                rows = _rowsNumber,
                cols = _colsNumber,
                stroke = _strokeNumber,
                path = _imgPath,
                snumber = splitNumbers,
            });

            //request.AddParameter("snumbers", splitNumbers);

            //request.AddParameter("application/json", splitNumbers, ParameterType.RequestBody);

            //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

            // easily add HTTP Headers
            //request.AddHeader("header", "value");

            // add files to upload (works with compatible verbs)
            //request.AddFile(path);

            // execute the request
            var response = client.Execute(request);
            var content = response.Content;

            MessageBox.Show(content);

            //// or automatically deserialize result
            //// return content type is sniffed but can be explicitly set via RestClient.AddHandler();
            //RestResponse<Person> response2 = client.Execute<Person>(request);
            //var name = response2.Data.Name;

            //// easy async support
            //client.ExecuteAsync(request, response => {
            //    Console.WriteLine(response.Content);
            //});

            //// async with deserialization
            //var asyncHandle = client.ExecuteAsync<Person>(request, response => {
            //    Console.WriteLine(response.Data.Name);
            //});

            //// abort the request on demand
            //asyncHandle.Abort();
        }

        private void MainImage_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_points.Count > 8)
                return;

            var pos = e.GetPosition(MainImage);
            PointList.Items.Add(pos.ToString());
            _points.Add(pos.X * _scaleRate);
            _points.Add(pos.Y * _scaleRate);
        }

        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            _points.Clear();
            PointList.Items.Clear();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
