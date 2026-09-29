using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace ABNCapture
{
    public partial class AreaSelectWindow : Window
    {
        private System.Windows.Point _startPoint;
        private bool _isSelecting;

        public Int32Rect? SelectedRegion { get; private set; }

        public AreaSelectWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(this);
            _isSelecting = true;

            SelectionRect.Visibility = Visibility.Visible;
            Canvas.SetLeft(SelectionRect, _startPoint.X);
            Canvas.SetTop(SelectionRect, _startPoint.Y);
            SelectionRect.Width = 0;
            SelectionRect.Height = 0;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isSelecting) return;

            var current = e.GetPosition(this);

            double x = Math.Min(current.X, _startPoint.X);
            double y = Math.Min(current.Y, _startPoint.Y);
            double width = Math.Abs(current.X - _startPoint.X);
            double height = Math.Abs(current.Y - _startPoint.Y);

            Canvas.SetLeft(SelectionRect, x);
            Canvas.SetTop(SelectionRect, y);
            SelectionRect.Width = width;
            SelectionRect.Height = height;
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isSelecting) return;
            _isSelecting = false;

            var screenX = (int)Canvas.GetLeft(SelectionRect);
            var screenY = (int)Canvas.GetTop(SelectionRect);
            var width = (int)SelectionRect.Width;
            var height = (int)SelectionRect.Height;

            if (width > 2 && height > 2)
            {
                SelectedRegion = new Int32Rect(screenX, screenY, width, height);
                DialogResult = true;
            }
            else
            {
                DialogResult = false;
            }

            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape) return;

            bool hasSelection = SelectionRect.Visibility == Visibility.Visible
                                 && SelectionRect.Width > 2
                                 && SelectionRect.Height > 2;

            if (!hasSelection)
            {
                DialogResult = false;
                Close();
                return;
            }

            this.Topmost = false;
            var result = MessageBox.Show(
                this,
                "Искаш ли да отмениш избора на зона?",
                "ABN Capture",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            this.Topmost = true;

            if (result == MessageBoxResult.Yes)
            {
                DialogResult = false;
                Close();
            }
        }
    }
}
