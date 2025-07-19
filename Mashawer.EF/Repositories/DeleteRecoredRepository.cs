namespace Mashawer.EF.Repositories
{
    public class DeleteRecoredRepository : GenericRepository<DeletedRecord>, IDeleteRecoredRepository
    {
        public DeleteRecoredRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
