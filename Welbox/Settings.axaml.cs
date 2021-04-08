using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaColorPicker;
using Welbox.Classes;

namespace Welbox
{
    public class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            var config = Config.GetConfig();
            AvaloniaXamlLoader.Load(this);
            LoadThemeSettings(config.SelectedTheme);
        }

        private void LoadThemeSettings(string themeName)
        {
            var themesBox = this.FindControl<ComboBox>("Themes");
            List<Theme> availableThemes = Theme.GetAvailableThemes();
            
            // Set current theme as chosen in combobox
            int index = 0;
            foreach (var theme in availableThemes)
            {
                if (theme.FileName == themeName) break;
                index += 1;
            }
            themesBox.Items = availableThemes;
            themesBox.SelectedIndex = index;

            Theme CurrentTheme = Theme.GetTheme(themeName);
            // Set source
            var typeBox = this.FindControl<ComboBox>("TypeComboBox");
            if (CurrentTheme.Source == "Image") typeBox.SelectedIndex = 0;

            // Set display name
            var displayBox = this.FindControl<TextBox>("DisplayNameBox");
            displayBox.Text = CurrentTheme.DisplayName;
            
            // Set font size
            var fontnum = this.FindControl<NumericUpDown>("FontSize");
            fontnum.Value = CurrentTheme.FontSize;
            
            // Set current font color
            var color = this.FindControl<ColorButton>("FontColor");
            color.Color = Color.Parse(CurrentTheme.HexTextColor);
        }

    }
}