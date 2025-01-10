using System;
using Microsoft.Extensions.Configuration;

namespace Api.Configuration;

public static class ConfigurationManagerExtensions
{
    public static TConfigurationSettings GetRequiredSettings<TConfigurationSettings>(this ConfigurationManager configurationManager) where TConfigurationSettings : IConfigurationSettings
    {
        var section = configurationManager.GetRequiredSection(TConfigurationSettings.SectionKey);
        return section.Get<TConfigurationSettings>() ?? throw new InvalidOperationException(TConfigurationSettings.ErrorMessage);
    }
}
