namespace DynamicConfig
{
    public interface IConfigurationReader
    {
        T GetValue<T>(string key);
    }
}
