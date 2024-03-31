using Openbook.Data.SaasModels;

namespace Openbook.Repository.Interface
{
    public interface IFeatures
    {
        Task<List<FeaturesView>> GetAll();
        Task<bool> CheckName(string name);
        Task<int> CheckNameId(string name);
        Task<int> Save(Features model);
        Task<bool> Update(Features model);
        Task<Features> GetbyId(int id);
        Task<bool> Delete(int id);
    }
}
