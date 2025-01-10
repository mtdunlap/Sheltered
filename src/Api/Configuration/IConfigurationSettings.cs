namespace Api.Configuration;

public interface IConfigurationSettings
{
    static abstract string SectionKey { get; }

    static abstract string ErrorMessage { get; }
}
