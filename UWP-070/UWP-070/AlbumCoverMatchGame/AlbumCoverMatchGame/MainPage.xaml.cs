using AlbumCoverMatchGame.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AlbumCoverMatchGame
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public LicenseInformation AppLicenseInformation { get; set; }

        private ObservableCollection<Song> Songs;
        private ObservableCollection<StorageFile> AllSongs;

        bool _playingMusic = false;
        int _round = 0;
        int _totalScore = 0;

        public MainPage()
        {
            this.InitializeComponent();

            Songs = new ObservableCollection<Song>();
        }

        private async Task RetrieveFilesInFolders(
            ObservableCollection<StorageFile> list,
            StorageFolder parent)
        {
            foreach (var item in await parent.GetFilesAsync())
            {
                if (item.FileType == ".mp3")
                    list.Add(item);
            }

            foreach (var item in await parent.GetFoldersAsync())
            {
                await RetrieveFilesInFolders(list, item);
            }
        }

        private async Task<List<StorageFile>> PickRandomSongs(ObservableCollection<StorageFile> allSongs)
        {
            Random random = new Random();
            var songCount = allSongs.Count;

            var randomSongs = new List<StorageFile>();

            while (randomSongs.Count < 10)
            {
                var randomNumber = random.Next(songCount);
                var randomSong = allSongs[randomNumber];

                // Find random songs BUT:
                // 1) Don't pick the same song twice!
                // 2) Don't pick a song from an album that I've already picked.

                MusicProperties randomSongMusicProperties =
                    await randomSong.Properties.GetMusicPropertiesAsync();

                bool isDuplicate = false;
                foreach (var song in randomSongs)
                {
                    MusicProperties songMusicProperties = await song.Properties.GetMusicPropertiesAsync();
                    if (String.IsNullOrEmpty(randomSongMusicProperties.Album)
                        || randomSongMusicProperties.Album == songMusicProperties.Album)
                        isDuplicate = true;

                }

                if (!isDuplicate)
                    randomSongs.Add(randomSong);
            }

            return randomSongs;
        }

        private async Task PopulateSongList(List<StorageFile> files)
        {
            int id = 0;

            foreach (var file in files)
            {
                MusicProperties songProperties = await file.Properties.GetMusicPropertiesAsync();

                StorageItemThumbnail currentThumb = await file.GetThumbnailAsync(
                    ThumbnailMode.MusicView,
                    200,
                    ThumbnailOptions.UseCurrentScale);

                var albumCover = new BitmapImage();
                albumCover.SetSource(currentThumb);

                var song = new Song();
                song.Id = id;
                song.Title = songProperties.Title;
                song.Artist = songProperties.Artist;
                song.Album = songProperties.Album;
                song.AlbumCover = albumCover;
                song.SongFile = file;

                Songs.Add(song);
                id++;
            }

        }

        private async void SongGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Ignore clicks when we are in cooldown
            if (!_playingMusic) return;

            CountDown.Pause();
            MyMediaElement.Stop();

            var clickedSong = (Song)e.ClickedItem;
            var correctSong = Songs.FirstOrDefault(p => p.Selected == true);

            // Evaluate the user's selection

            Uri uri;
            int score;
            if (clickedSong.Selected)
            {
                uri = new Uri("ms-appx:///Assets/correct.png");
                score = (int)MyProgressBar.Value;
            }
            else
            {
                uri = new Uri("ms-appx:///Assets/incorrect.png");
                score = ((int)MyProgressBar.Value) * -1;
            }
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            var fileStream = await file.OpenAsync(FileAccessMode.Read);
            await clickedSong.AlbumCover.SetSourceAsync(fileStream);

            _totalScore += score;
            _round++;

            ResultTextBlock.Text = string.Format("Score: {0} Total Score after {1} Rounds: {2}", score, _round, _totalScore);
            TitleTextBlock.Text = String.Format("Correct Song: {0}", correctSong.Title);
            ArtistTextBlock.Text = string.Format("Performed by: {0}", correctSong.Artist);
            AlbumTextBlock.Text = string.Format("On Album: {0}", correctSong.Album);

            clickedSong.Used = true;

            correctSong.Selected = false;
            correctSong.Used = true;

            if (_round >= 5)
            {
                InstructionTextBlock.Text = string.Format("Game over ... You scored: {0}", _totalScore);
                PlayAgainButton.Visibility = Visibility.Visible;
            } else
            {
                StartCooldown();
            }
        }

        private async void PlayAgainButton_Click(object sender, RoutedEventArgs e)
        {
            await PrepareNewGame();

            PlayAgainButton.Visibility = Visibility.Collapsed;
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

            if (AppLicenseInformation.ProductLicenses["RemoveAdsOffer"].IsActive)
            {
                // Customer can access this feature.
                AdMediator_40F141.Visibility = Visibility.Collapsed;
                PurchaseButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Customer can NOT access this feature.
                AdMediator_40F141.Visibility = Visibility.Visible;
                PurchaseButton.Visibility = Visibility.Visible;
            }
        }

        private async Task<ObservableCollection<StorageFile>> SetupMusicList()
        {
            // 1. Get access to Music library
            StorageFolder folder = KnownFolders.MusicLibrary;
            var allSongs = new ObservableCollection<StorageFile>();
            await RetrieveFilesInFolders(allSongs, folder);
            return allSongs;
        }

        private async Task PrepareNewGame()
        {
            Songs.Clear();

            // Choose random songs from library
            var randomSongs = await PickRandomSongs(AllSongs);

            // Pluck off meta data from selected songs
            await PopulateSongList(randomSongs);

            StartCooldown();

            // State management
            InstructionTextBlock.Text = "Get ready ...";
            ResultTextBlock.Text = "";
            TitleTextBlock.Text = "";
            ArtistTextBlock.Text = "";
            AlbumTextBlock.Text = "";

            _totalScore = 0;
            _round = 0;

        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            StartupProgressRing.IsActive = true;

            AllSongs = await SetupMusicList();
            await PrepareNewGame();

            StartupProgressRing.IsActive = false;

            StartCooldown();
        }

        private void StartCooldown()
        {
            _playingMusic = false;
            SolidColorBrush brush = new SolidColorBrush(Colors.Blue);
            MyProgressBar.Foreground = brush;
            InstructionTextBlock.Text = string.Format("Get ready for round {0} ...", _round + 1);
            InstructionTextBlock.Foreground = brush;
            CountDown.Begin();
        }

        private void StartCountdown()
        {
            _playingMusic = true;
            SolidColorBrush brush = new SolidColorBrush(Colors.Red);
            MyProgressBar.Foreground = brush;
            InstructionTextBlock.Text = "GO!";
            InstructionTextBlock.Foreground = brush;
            CountDown.Begin();
        }

        private async void CountDown_Completed(object sender, object e)
        {
            if (!_playingMusic)
            {
                // Start playing music
                var song = PickSong();

                MyMediaElement.SetSource(
                    await song.SongFile.OpenAsync(FileAccessMode.Read),
                    song.SongFile.ContentType);

                // Start countdown
                StartCountdown();
            }
        }

        private Song PickSong()
        {
            Random random = new Random();
            var unusedSongs = Songs.Where(p => p.Used == false);
            var randomNumber = random.Next(unusedSongs.Count());
            var randomSong = unusedSongs.ElementAt(randomNumber);
            randomSong.Selected = true;
            return randomSong;
        }

        private async void PurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (!AppLicenseInformation.ProductLicenses["MyInAppOfferToken"].IsActive)
            {
                try
                {
                    // The customer doesn't own this feature, so 
                    // show the purchase dialog.

                    PurchaseResults results = await CurrentAppSimulator.RequestProductPurchaseAsync("RemoveAdsOffer");

                    //Check the license state to determine if the in-app purchase was successful.

                    if (results.Status == ProductPurchaseStatus.Succeeded)
                    {
                        AdMediator_40F141.Visibility = Visibility.Collapsed;
                        PurchaseButton.Visibility = Visibility.Collapsed;
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
    }
}
