using System;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace DisJockey.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddAzureKeyVault(this ConfigurationManager configuration)
    {
        var keyVaultUri = configuration.GetValue<string>("AzureKeyValueUri");
        if (!string.IsNullOrWhiteSpace(keyVaultUri))
        {
            configuration.AddAzureKeyVault(new Uri(keyVaultUri), new VisualStudioCredential());
        }
    }
}
