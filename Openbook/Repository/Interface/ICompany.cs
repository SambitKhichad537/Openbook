using Openbook.Data.Setting;

namespace Openbook.Repository.Interface
{
	public interface ICompany
	{
		Task <Company> GetById();
		Task<int> Save(Company company);
		Task<bool> Update(Company company);
        List<Company> GetCompany(string id);
    }
}
