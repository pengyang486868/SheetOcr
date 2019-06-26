using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace SheetOcr.Map
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
            _imgPath = @"D:\map\main.jpg";
            _points = new List<List<double>>();
        }

        private string _imgPath;
        private double _sourceWidth;
        private double _sourceHeight;
        private double _displayWidth;
        private double _displayHeight;
        private double _scaleRate;
        private List<List<double>> _points;



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
            _points.Add(_points.First());
            Clipboard.SetDataObject(JsonConvert.SerializeObject(_points));
        }

        private void MainImage_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (_points.Count > 8)
            //    return;

            var pos = e.GetPosition(MainImage);
            PointList.Items.Add(pos.ToString());
            _points.Add(new List<double> { pos.X * _scaleRate, pos.Y * _scaleRate });
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
