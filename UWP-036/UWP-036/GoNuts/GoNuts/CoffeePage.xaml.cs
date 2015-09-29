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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GoNuts
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CoffeePage : Page
    {
        public CoffeePage()
        {
            this.InitializeComponent();
        }

        private string _roast;
        private string _sweetener;
        private string _cream;

        private void Roast_Click(object sender, RoutedEventArgs e)
        {
            var selected = (MenuFlyoutItem)sender;
            _roast = selected.Text;
            displayResult();
        }

        private void Sweetener_Click(object sender, RoutedEventArgs e)
        {
            var selected = (MenuFlyoutItem)sender;
            _sweetener = selected.Text;
            displayResult();

        }

        private void Cream_Click(object sender, RoutedEventArgs e)
        {
            var selected = (MenuFlyoutItem)sender;
            _cream = selected.Text;
            displayResult();

        }

        private void displayResult()
        {
            if (_roast == "None" || String.IsNullOrEmpty(_roast))
            {
                ResultTextBlock.Text = "None";
                return;
            }

            ResultTextBlock.Text = _roast;

            if (_cream != "None" && !String.IsNullOrEmpty(_cream))
                ResultTextBlock.Text += " + " + _cream;

            if (_sweetener != "None" && !String.IsNullOrEmpty(_sweetener))
                ResultTextBlock.Text += " + " + _sweetener;

        }
    }
}
