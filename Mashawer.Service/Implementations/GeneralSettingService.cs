namespace Mashawer.Service.Implementations
{
    public class GeneralSettingService(IUnitOfWork unitOfWork) : IGeneralSettingService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<string> CreateGeneralSettingAsync(GeneralSetting generalSetting, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.GeneralSettings.AddAsync(generalSetting, cancellationToken);
            await _unitOfWork.CompeleteAsync();
            return "Created";
        }

        public async Task<GeneralSetting?> GetGeneralSettingsAsync()
        {
            return await _unitOfWork.GeneralSettings.GetTableNoTracking().FirstOrDefaultAsync();
        }


        public async Task<string> UpdateGeneralSettingAsync(GeneralSetting generalSetting)
        {
            _unitOfWork.GeneralSettings.Update(generalSetting);
            await _unitOfWork.CompeleteAsync();
            return "Updated";
        }
    }
}
