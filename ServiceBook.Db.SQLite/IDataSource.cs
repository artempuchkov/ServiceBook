namespace ServiceBook.Db.SQLite;
public interface IDataSource
{
	Task SaveRepairStatus(RepairStatus status);
	Task DeleteRepairStatus(int id);
	Task<RepairStatus[]> ReadRepairStatus(int? id = null);
}