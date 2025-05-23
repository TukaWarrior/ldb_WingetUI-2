using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Text;
using WingetUI2.Domain;

namespace WingetUI2
{
    public sealed partial class MainWindow : Window
    {
        private readonly IWingetService _wingetService;

        public MainWindow()
        {
            InitializeComponent();
            _wingetService = new WingetService();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OutputBox.Text = "Searching...";
                var packages = await _wingetService.SearchPackagesAsync(SearchBox.Text);
                
                var sb = new StringBuilder();
                sb.AppendLine($"Found {packages.Count} packages:");
                foreach (var package in packages)
                {
                    sb.AppendLine($"- {package.Name} ({package.Id})");
                    sb.AppendLine($"  Version: {package.Version}");
                    sb.AppendLine($"  Publisher: {package.Publisher}");
                    sb.AppendLine();
                }
                
                OutputBox.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                OutputBox.Text = $"Error: {ex.Message}";
            }
        }

        private async void GetInstalledButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OutputBox.Text = "Getting installed packages...";
                var packages = await _wingetService.GetInstalledPackagesAsync();
                
                var sb = new StringBuilder();
                sb.AppendLine($"Found {packages.Count} installed packages:");
                foreach (var package in packages)
                {
                    sb.AppendLine($"- {package.Name} ({package.Id})");
                    sb.AppendLine($"  Version: {package.Version}");
                    sb.AppendLine();
                }
                
                OutputBox.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                OutputBox.Text = $"Error: {ex.Message}";
            }
        }

        private async void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var packageId = SearchBox.Text;
                OutputBox.Text = $"Installing {packageId}...";
                await _wingetService.InstallPackageAsync(packageId);
                OutputBox.Text += "\nInstallation completed!";
            }
            catch (Exception ex)
            {
                OutputBox.Text = $"Error: {ex.Message}";
            }
        }

        private async void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var packageId = SearchBox.Text;
                OutputBox.Text = $"Uninstalling {packageId}...";
                await _wingetService.UninstallPackageAsync(packageId);
                OutputBox.Text += "\nUninstallation completed!";
            }
            catch (Exception ex)
            {
                OutputBox.Text = $"Error: {ex.Message}";
            }
        }

        private async void UpgradeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var packageId = SearchBox.Text;
                OutputBox.Text = $"Upgrading {packageId}...";
                await _wingetService.UpgradePackageAsync(packageId);
                OutputBox.Text += "\nUpgrade completed!";
            }
            catch (Exception ex)
            {
                OutputBox.Text = $"Error: {ex.Message}";
            }
        }
    }
}
