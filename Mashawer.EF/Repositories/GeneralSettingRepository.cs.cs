namespace Mashawer.EF.Repositories
{
    public class GeneralSettingRepository : GenericRepository<GeneralSetting>, IGeneralSettingRepository
    {
        public GeneralSettingRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
