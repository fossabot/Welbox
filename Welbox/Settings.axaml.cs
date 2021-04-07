using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

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
            AvaloniaXamlLoader.Load(this);
            var themes = this.FindControl<ComboBox>("Themes");
            
        }
    }
}