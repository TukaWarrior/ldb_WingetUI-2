using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Threading.Tasks;
using System.Linq;

namespace WingetUI2.Domain
{
    public class WinGetPackage
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public bool IsInstalled { get; set; }
        public string Publisher { get; set; } = string.Empty;
    }

    public interface IWingetService
    {
        Task<List<WinGetPackage>> SearchPackagesAsync(string searchTerm);
        Task<List<WinGetPackage>> GetInstalledPackagesAsync();
        Task InstallPackageAsync(string packageId);
        Task UninstallPackageAsync(string packageId);
        Task UpgradePackageAsync(string packageId);
    }

    public class WingetService : IWingetService
    {
        private async Task<IList<PSObject>> InvokePowerShellAsync(string command)
        {
            return await Task.Run(() =>
            {
                using var ps = PowerShell.Create();
                ps.AddScript("Import-Module Microsoft.WinGet.Client");
                ps.AddScript(command);
                
                var results = ps.Invoke();
                
                if (ps.HadErrors)
                {
                    var error = ps.Streams.Error.FirstOrDefault();
                    throw new Exception($"WinGet error: {error?.Exception.Message ?? "Unknown error"}");
                }
                
                return results;
            });
        }

        public async Task<List<WinGetPackage>> SearchPackagesAsync(string searchTerm)
        {
            var results = await InvokePowerShellAsync($"Find-WinGetPackage -Query '{searchTerm}'");
            return results.Select(MapToPackage).ToList();
        }

        public async Task<List<WinGetPackage>> GetInstalledPackagesAsync()
        {
            var results = await InvokePowerShellAsync("Get-WinGetPackage");
            return results.Select(MapToPackage).ToList();
        }

        public async Task InstallPackageAsync(string packageId)
        {
            await InvokePowerShellAsync($"Install-WinGetPackage -Id '{packageId}' -AcceptSourceAgreements");
        }

        public async Task UninstallPackageAsync(string packageId)
        {
            await InvokePowerShellAsync($"Uninstall-WinGetPackage -Id '{packageId}'");
        }

        public async Task UpgradePackageAsync(string packageId)
        {
            await InvokePowerShellAsync($"Update-WinGetPackage -Id '{packageId}'");
        }

        private WinGetPackage MapToPackage(PSObject psObject)
        {
            return new WinGetPackage
            {
                Id = psObject.Properties["Id"]?.Value?.ToString() ?? string.Empty,
                Name = psObject.Properties["Name"]?.Value?.ToString() ?? string.Empty,
                Version = psObject.Properties["Version"]?.Value?.ToString() ?? string.Empty,
                Source = psObject.Properties["Source"]?.Value?.ToString() ?? string.Empty,
                Publisher = psObject.Properties["Publisher"]?.Value?.ToString() ?? string.Empty,
                IsInstalled = psObject.Properties["IsInstalled"]?.Value as bool? ?? false
            };
        }
    }
}