using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WhatIsXAML
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void ClickMeButton_Click(object sender, RoutedEventArgs e)
        {
            MessageTextBlock.Text = "What is XAML?";
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            Button myButton = new Button();
            myButton.Name = "ClickMeButton";
            myButton.Content = "Click Me";
            myButton.Width = 200;
            myButton.Height = 100;
            myButton.Margin = new Thickness(20, 20, 0, 0);
            myButton.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
            myButton.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;

            myButton.Background = new SolidColorBrush(Windows.UI.Colors.Red);
            myButton.Click += ClickMeButton_Click;

            LayoutGrid.Children.Add(myButton);
            */
        }

    }
}
