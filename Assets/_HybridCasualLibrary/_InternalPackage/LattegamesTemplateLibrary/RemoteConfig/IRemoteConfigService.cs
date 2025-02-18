namespace LatteGames
{
    public interface IRemoteConfigService
    {
        float GetFloat(string key, float defaultValue);
        int GetInt(string key, int defaultValue);
        string GetString(string key, string defaultValue);
        bool GetBool(string key, bool defaultValue);
    }
}