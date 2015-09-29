using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace InAppPurchaseExample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public LicenseInformation AppLicenseInformation { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void PurchaseFeatureButton_Click(object sender, RoutedEventArgs e)
        {
            if (!AppLicenseInformation.ProductLicenses["MyInAppOfferToken"].IsActive)
            {
                try
                {
                    // The customer doesn't own this feature, so 
                    // show the purchase dialog.

                    PurchaseResults results = await CurrentAppSimulator.RequestProductPurchaseAsync("MyInAppOfferToken");

                    //Check the license state to determine if the in-app purchase was successful.

                    if (results.Status == ProductPurchaseStatus.Succeeded)
                    {
                        PurchaseFeatureButton.Visibility = Visibility.Collapsed;
                        FeatureRectangle.Visibility = Visibility.Visible;
                    }
                }
                catch (Exception ex)
                {
                    // The in-app purchase was not completed because 
                    // an error occurred.
                    throw ex;
                }
            }
            else
            {
                // The customer already owns this feature.
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // Remove these lines of code before publishing!
            // The actual CurrentApp will create a WindowsStoreProxy.xml
            // in the package's \LocalState\Microsoft\Windows Store\ApiData
            // folder where it stores the actual purchases.
            // Here we're just giving it a fake version of that file
            // for testing.
            StorageFolder proxyDataFolder = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
            StorageFile proxyFile = await proxyDataFolder.GetFileAsync("test.xml");
            await CurrentAppSimulator.ReloadSimulatorAsync(proxyFile);
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            // You may want to put this at the App level
            AppLicenseInformation = CurrentAppSimulator.LicenseInformation;

            if (AppLicenseInformation.ProductLicenses["MyInAppOfferToken"].IsActive)
            {
                // Customer can access this feature.
                FeatureRectangle.Visibility = Visibility.Visible;
                PurchaseFeatureButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Customer can NOT access this feature.
                FeatureRectangle.Visibility = Visibility.Collapsed;
                PurchaseFeatureButton.Visibility = Visibility.Visible;
            }
        }
    }
}
