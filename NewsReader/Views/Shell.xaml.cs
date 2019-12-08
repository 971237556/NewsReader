using System.ComponentModel;
using System.Linq;
using System;
using Template10.Common;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Template10.Mvvm;
using Windows.Media.Playback;

namespace NewsReader.Views
{
    public sealed partial class Shell : Page
    {
        public static Shell Instance { get; set; }
        public static HamburgerMenu HamburgerMenu => Instance.MyHamburgerMenu;
        Services.SettingsServices.SettingsService _settings;

        public Shell()
        {
            Instance = this;
            InitializeComponent();
            _settings = Services.SettingsServices.SettingsService.Instance;
        }

        public Shell(INavigationService navigationService) : this()
        {
            SetNavigationService(navigationService);
        }

        public void SetNavigationService(INavigationService navigationService)
        {
            MyHamburgerMenu.NavigationService = navigationService;
            //HamburgerMenu.RefreshStyles(_settings.AppTheme, true);
            HamburgerMenu.IsFullScreen = _settings.IsFullScreen;
            HamburgerMenu.HamburgerButtonVisibility = _settings.ShowHamburgerButton ? Visibility.Visible : Visibility.Collapsed;
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            Services.MusicServices.MusicService.PlayerOnline();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (Services.MusicServices.MusicService.MediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            {
                Services.MusicServices.MusicService.MediaPlayer.Pause();
            }
            
            else if(Services.MusicServices.MusicService.MediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Paused)
            {
                Services.MusicServices.MusicService.MediaPlayer.Play();
            }

        }
    }
}
