using System;
using System.IO;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Welbox.Classes;

namespace Welbox
{
    public class MainWindow : Window
    {
        private Config _config;
        private Theme theme;
        private Timer ClockTimer;
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
            theme = Theme.GetTheme(_config.SelectedTheme);
            LoadTheme();
            if(theme.LaunchItems != null) LoadStartIcons();
        }

        private void LoadTheme()
        {
            if (theme.Source == "file")
            {
                // Background
                //var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                var filestream = new FileStream(theme.BgPath,FileMode.Open);
                var bitmap = new Bitmap(filestream);
                this.Background = new ImageBrush(bitmap);
                
                // Text color
                var style = new Style(x => x.OfType<TextBlock>());
                var color = new Setter();
                color.Property = ForegroundProperty;
                color.Value = SolidColorBrush.Parse(theme.HexTextColor);
                style.Setters.Add(color);
                var menustyle = new Style(x => x.OfType<MenuItem>());
                menustyle.Setters.Add(color);
                
                
                // Text size
                var size = new Setter();
                size.Property = TextBlock.FontSizeProperty;
                size.Value = theme.FontSize;
                style.Setters.Add(size);
                    
                    
                this.Styles.Add(style);

                var clock = this.FindControl<TextBlock>("Clock");
                clock.Text = DateTime.Now.ToString("t");
                
                ClockTimer = new Timer();
                ClockTimer.Interval = 10000;
                ClockTimer.Elapsed += ChangeTime;
                ClockTimer.Start();
            }
        }

        private void LoadStartIcons()
        {
            var panel = this.FindControl<DockPanel>("IconPanel");
            foreach (var item in theme.LaunchItems)
            {
                var iconButton = new Button();
                Projektanker.Icons.Avalonia.Attached.SetIcon(iconButton,item.IconName);
                iconButton.Click += OnButtonClick;
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

        private void MenuItem_OnClick(object? sender, RoutedEventArgs e)
        {
            var settings = new Settings();
            settings.ShowDialog(this);
        }
    }
}