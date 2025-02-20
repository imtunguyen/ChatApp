namespace ChatApp.Application.Abstracts.Services
{
    public interface IRedisService
    {
        Task<T?> GetDataObjectByKey<T>(string key);
        Task<List<T>?> GetDataByEndpoint<T>(string endpoint);
        Task SetData(string key, object data, TimeSpan? time);
        Task RemoveDataByKey(string key);
        Task UploadDataByKey(string key, object data);
    }
}
