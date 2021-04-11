using System;
using System.Diagnostics;
using System.IO;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Projektanker.Icons.Avalonia;
using Welbox.Classes;

namespace Welbox
{
    public class MainWindow : Window
    {
        private Config? _config;
        private Theme? _theme;
        private Timer? _clockTimer;
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            _config = Config.GetConfig();
            _theme = Theme.GetTheme(_config.SelectedTheme);
            LoadTheme();
            if(_theme.LaunchItems != null) LoadStartIcons();
        }

        private void LoadTheme()
        {
            if (_theme.Source == "File")
            {
                // Background
                //var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                var filestream = new FileStream(_theme.BgPath,FileMode.Open);
                var bitmap = new Bitmap(filestream);
                this.Background = new ImageBrush(bitmap) {Stretch = Stretch.UniformToFill}; //! add this to theme settings
                
                // Text color
                var style = new Style(x => x.OfType<TextBlock>());
                var color = new Setter();
                color.Property = ForegroundProperty;
                color.Value = SolidColorBrush.Parse(_theme.HexTextColor);
                style.Setters.Add(color);
                var menustyle = new Style(x => x.OfType<MenuItem>());
                menustyle.Setters.Add(color);

                // Text size
                var size = new Setter();
                size.Property = TextBlock.FontSizeProperty;
                size.Value = _theme.FontSize;
                style.Setters.Add(size);
                    
                    
                this.Styles.Add(style);

                var clock = this.FindControl<TextBlock>("Clock");
                clock.Text = DateTime.Now.ToString("t");
                
                _clockTimer = new Timer();
                _clockTimer.Interval = 10000;
                _clockTimer.Elapsed += ChangeTime;
                _clockTimer.Start();
            }
        }

        private void LoadStartIcons()
        {
            var panel = this.FindControl<DockPanel>("IconPanel");
            foreach (var item in _theme.LaunchItems)
            {
                var iconButton = new Button();
                Projektanker.Icons.Avalonia.Attached.SetIcon(iconButton,item.IconName);
                iconButton.Click += OnButtonClick;
                iconButton.FontSize = 30;
                iconButton.Width = 80;
                iconButton.Height = 80;
                var bgStyle = new Style();
                var bgSet = new Setter();
                bgSet.Property = BackgroundProperty;
                bgSet.Value = SolidColorBrush.Parse(_theme.HexTextColor); // change this to a unique setting in Theme.cs
                bgStyle.Setters.Add(bgSet);
                iconButton.Styles.Add(bgStyle);
                iconButton.Click += (sender, args) =>
                {
                    ProcessStartInfo startInfo = new() { FileName = item.LaunchCommand }; 
                    Process proc = new() { StartInfo = startInfo, };
                    proc.Start();
                };
                panel.Children.Add(iconButton);
            }
        }

        private void OnButtonClick(object source, RoutedEventArgs e)
        {
            Console.WriteLine(e);
        }
        
        private void ChangeTime(object source, ElapsedEventArgs e)
        {
            var clock = this.FindControl<TextBlock>("Clock");
            clock.Text = DateTime.Now.ToString("t");
        }

        private async void MenuItem_OnClick(object? sender, RoutedEventArgs e)
        {
            var settings = new Settings();
            var set = await settings.ShowDialog<bool?>(this);
            if(set == true)
            {
                LoadTheme();
            }
        }

        private void About_OnClick(object? sender, RoutedEventArgs e)
        {
            var win = new Credits();
            win.ShowDialog(this);
        }
    }
}