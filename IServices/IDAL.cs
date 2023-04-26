using Hangfire_background_jobs.Models;

namespace Hangfire_background_jobs.IServices
{
    public interface IDAL
    {
        List<TransactionModel> GetfromMasterDEtail();
        bool Update_master_detail(TransactionModel mdl);
    }
}
