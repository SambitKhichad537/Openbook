using Openbook.Data.Setting;
using Openbook.Data.ViewModel;

namespace Openbook.Repository.Interface
{
    public interface IProduct
	{
        Task<List<ProductView>> GetAll();
		Task<List<ProductView>> GetAllSales();
		Task<bool> CheckName(string name);
        Task<int> CheckNameId(string name);
        Task<int> Save(Product model);
        Task<bool> Update(Product model);
        Task<Product> GetbyId(int id);
        Task<bool> Delete(int id);
		string GetProductNo();
	}
}
