using ServiceBook.Db.SQLite.Models;

namespace ServiceBook.Db.SQLite;
public interface IDataSource
{
	Task SaveRepairStatus(RepairStatus status);
	Task DeleteRepairStatus(int id);
	Task<RepairStatus[]> ReadRepairStatus(int? id = null);

	Task UserRegistration(string url, RegisterViewModel model);
	Task ConfirmEmail(string userCode);

	Task<string> SignIn(LoginViewModel model);
    Task<UserInfoModel[]> ReadUserInfo(string userCode);
	Task<ServiceModel[]> ReadService(int? id = null);
	Task<ServiceModel[]> ReadByCategoryService(int category, int? service_id = null);
	Task<CategoryModel[]> GetCategories(int? parent = null);
	Task<ServiceModel[]> ReadServiceSearch(string search);
}