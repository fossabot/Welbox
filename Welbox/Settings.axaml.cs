using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using AvaloniaColorPicker;
using MessageBox.Avalonia.Enums;
using Newtonsoft.Json;
using Projektanker.Icons.Avalonia;
using Welbox.Classes;

namespace Welbox
{
    public class Settings : Window
    {
        private int boxIndex = 0;
        private List<TextBox> _scriptBoxes = new();
        private List<TextBox> _iconBoxes = new();
        private string _loadedTheme;
        public Settings()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            boxIndex = 0;
            var config = Config.GetConfig();
            AvaloniaXamlLoader.Load(this);
            LoadThemeSettings(config.SelectedTheme);
        }

        private void LoadThemeSettings(string themeName)
        {
            // clear help variables
            boxIndex = 0;
            _scriptBoxes = new List<TextBox>();
            _iconBoxes = new List<TextBox>();
            
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
            _loadedTheme = CurrentTheme.FileName;
            // Set source
            var typeBox = this.FindControl<ComboBox>("TypeComboBox");
            if (CurrentTheme.Source == "File")
            {
                typeBox.SelectedIndex = 0;
                var PathBox = this.FindControl<TextBox>("ImagePathBox");
                PathBox.Text = CurrentTheme.BgPath;
            }

            // Set display name
            var displayBox = this.FindControl<TextBox>("DisplayNameBox");
            displayBox.Text = CurrentTheme.DisplayName;
            
            // Set font size
            var fontnum = this.FindControl<NumericUpDown>("FontSize");
            fontnum.Value = CurrentTheme.FontSize;
            
            // Set current font color
            var color = this.FindControl<ColorButton>("FontColor");
            color.Color = Color.Parse(CurrentTheme.HexTextColor);
            
            // Add launch icons
            if (CurrentTheme.LaunchItems.Count > 0)
            {
                var panel = this.FindControl<StackPanel>("IconPanel");
                foreach (var launchItem in CurrentTheme.LaunchItems)
                {
                    var IconPanel = new StackPanel {Margin = Thickness.Parse("10,10,0,0")};

                    var IconNameBlock = new TextBlock {Text = "Icon code (Font Awesome 5)"};
                    IconPanel.Children.Add(IconNameBlock);
                    var IconNameBox = new TextBox {Watermark = "fab fa-github", Width = 200};
                    IconNameBox.Text = launchItem.IconName;
                    _iconBoxes.Add(IconNameBox);
                    IconPanel.Children.Add(IconNameBox);

                    var openPanel = new DockPanel();
                    var IconScriptBlock = new TextBlock {Text = "Script to call on click"};
                    IconPanel.Children.Add(IconScriptBlock);
                    var ScriptBox = new TextBox();
                    ScriptBox.Text = launchItem.LaunchCommand;
                    ScriptBox.IsReadOnly = true;
                    ScriptBox.Width = 200;
                    ScriptBox.Name = "PathBox"+boxIndex;
                    _scriptBoxes.Add(ScriptBox);
                    openPanel.Children.Add(ScriptBox);
                    var ScriptOpenButton = new Button();
                    ScriptOpenButton.Click += OpenScriptButton;
                    ScriptOpenButton.Margin = Thickness.Parse("5,0,0,0");
                    ScriptOpenButton.Name = "OpenButton" + boxIndex;
                    Attached.SetIcon(ScriptOpenButton,"fas fa-folder-open");
                    openPanel.Children.Add(ScriptOpenButton);
                    IconPanel.Children.Add(openPanel);
            
                    panel.Children.Add(IconPanel);
                    boxIndex += 1;
                }
            }
        }

        private void SaveButton_OnClick(object? sender, RoutedEventArgs e)
        {
            bool SaveFailSwitch = false;
            Theme SaveTheme = new();
            SaveTheme.FileName = _loadedTheme;
            
            var typeBox = this.FindControl<ComboBox>("TypeComboBox");
            var item = (ComboBoxItem) typeBox.SelectedItem;
            SaveTheme.Source = item.Content.ToString();

            if (SaveTheme.Source == "File")
            {
                var PathBox = this.FindControl<TextBox>("ImagePathBox");
                if (string.IsNullOrEmpty(PathBox.Text))
                {
                    var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
                        .GetMessageBoxStandardWindow("Error", "You need to set an image path.",ButtonEnum.Ok,MessageBox.Avalonia.Enums.Icon.Error);
                    messageBoxStandardWindow.Show();
                    return;
                }

                SaveTheme.BgPath = PathBox.Text;
            }
            
            var displayBox = this.FindControl<TextBox>("DisplayNameBox");
            SaveTheme.DisplayName = displayBox.Text;
            
            var fontnum = this.FindControl<NumericUpDown>("FontSize");
            SaveTheme.FontSize = fontnum.Value;
            
            var color = this.FindControl<ColorButton>("FontColor");
            SaveTheme.HexTextColor = color.Color.ToString().Replace("#ff","#"); //! Might break(?)

            List<LaunchItem> Launch = new();
            if (_iconBoxes.Count > 0)
            {
                for (int i = 0; i < boxIndex; i++)
                {
                    LaunchItem l = new();
                    TextBox Icon = _iconBoxes[i];
                    TextBox Script = _scriptBoxes[i];
                    if (String.IsNullOrEmpty(Icon.Text))
                    {
                        SaveFailSwitch = true;
                        var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
                            .GetMessageBoxStandardWindow("Error", "You need to set a Font Awesome icon name.",ButtonEnum.Ok,MessageBox.Avalonia.Enums.Icon.Error);
                        messageBoxStandardWindow.Show();
                        break;
                    }

                    try
                    {
                        IconProvider.GetIconPath(Icon.Text);
                    }
                    catch (Exception exception)
                    {
                        SaveFailSwitch = true;
                        var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
                            .GetMessageBoxStandardWindow("Error", "Icon not found. Make sure you wrote its name correctly.",ButtonEnum.Ok,MessageBox.Avalonia.Enums.Icon.Error);
                        messageBoxStandardWindow.Show();
                        break;
                    }
                    l.IconName = Icon.Text;

                    if (String.IsNullOrEmpty(Script.Text))
                    {
                        SaveFailSwitch = true;
                        var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
                            .GetMessageBoxStandardWindow("Error", "Launch command path cannot be empty.",ButtonEnum.Ok,MessageBox.Avalonia.Enums.Icon.Error);
                        messageBoxStandardWindow.Show();
                    }
                    l.LaunchCommand = Script.Text;
                    Launch.Add(l);
                }
            }

            SaveTheme.LaunchItems = Launch;
            if (SaveFailSwitch) return;
            var json = JsonConvert.SerializeObject(SaveTheme);
            File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Welbox/Themes/"+SaveTheme.FileName+".json"),json);
            Close(true);
        }

        private void AddIcon_OnClick(object? sender, RoutedEventArgs e)
        {
            var panel = this.FindControl<StackPanel>("IconPanel");
            var IconPanel = new StackPanel {Margin = Thickness.Parse("10,10,0,0")};

            var IconNameBlock = new TextBlock {Text = "Icon code (Font Awesome 5)"};
            IconPanel.Children.Add(IconNameBlock);
            var IconNameBox = new TextBox {Watermark = "fab fa-github", Width = 200};
            _iconBoxes.Add(IconNameBox);
            IconPanel.Children.Add(IconNameBox);

            var openPanel = new DockPanel();
            var IconScriptBlock = new TextBlock {Text = "Script to call on click"};
            IconPanel.Children.Add(IconScriptBlock);
            var ScriptBox = new TextBox();
            ScriptBox.IsReadOnly = true;
            ScriptBox.Width = 200;
            ScriptBox.Name = "PathBox"+boxIndex;
            _scriptBoxes.Add(ScriptBox);
            openPanel.Children.Add(ScriptBox);
            var ScriptOpenButton = new Button();
            ScriptOpenButton.Click += OpenScriptButton;
            ScriptOpenButton.Margin = Thickness.Parse("5,0,0,0");
            ScriptOpenButton.Name = "OpenButton" + boxIndex;
            Attached.SetIcon(ScriptOpenButton,"fas fa-folder-open");
            openPanel.Children.Add(ScriptOpenButton);
            IconPanel.Children.Add(openPanel);
            
            panel.Children.Add(IconPanel);
            boxIndex += 1;
        }
        
        private async Task<string> GetPath(FileDialogFilter? filters=null)
        {
            OpenFileDialog dialog = new();
            if (filters != null)
            {
                dialog.Filters.Add(filters);
            }

            string[] result = await dialog.ShowAsync(this);

            return result == null ? "" : string.Join(" ", result);
        }

        private async void OpenScriptButton(object? sender, RoutedEventArgs e)
        {
            var b = (Button) sender;
            var file = await GetPath();
            var box = _scriptBoxes[int.Parse(b.Name.Replace("OpenButton", ""))];
            box.Text = file;
        }

        private async void OpenImageBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            var PathBox = this.FindControl<TextBox>("ImagePathBox");
            var filter = new FileDialogFilter();
            filter.Name = "Images";
            filter.Extensions.Add("jpg");
            filter.Extensions.Add("png");
            filter.Extensions.Add("jpeg");
            var path = await GetPath(filter);
            PathBox.Text = path;
        }
    }
}