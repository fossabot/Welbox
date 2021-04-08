using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
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
            var themesBox = this.FindControl<ComboBox>("Themes");
            List<Theme> AvailableThemes = Theme.GetAvailableThemes();
            int index = 0;
            foreach (var theme in AvailableThemes)
            {
                if (theme.FileName == config.SelectedTheme) break;
                index += 1;
            }
            themesBox.Items = AvailableThemes;
            themesBox.SelectedIndex = index;
        }
    }
}