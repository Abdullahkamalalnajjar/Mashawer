namespace Mashawer.Service.Abstracts
{
    public interface IGeneralSettingService
    {
        public Task<string> CreateGeneralSettingAsync(GeneralSetting generalSetting, CancellationToken cancellationToken = default);
        public Task<string> UpdateGeneralSettingAsync(GeneralSetting generalSetting);
        public Task<GeneralSetting?> GetGeneralSettingsAsync();
    }
}
