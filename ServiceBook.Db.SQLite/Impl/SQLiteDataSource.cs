using Dapper;

namespace ServiceBook.Db.SQLite;
public class SQLiteDataSource : IDataSource
{
	private int _sqlCommandTimeout = 600;	// seconds

	private readonly IDbConnectionFactory _db;

	public SQLiteDataSource(IDbConnectionFactory db)
	{
		_db = db;
	}

	public async Task SaveRepairStatus(RepairStatus status)
	{
		if (status == null)
			return;

		const string sql = @"
			insert into repair_status (id, name)
				values (@Id, @Name)
			on conflict(id) do
			update set
				name = excluded.name;";

		await _db.CreateConnection().ExecuteAsync(sql, status, commandTimeout: _sqlCommandTimeout);
	}
	public async Task DeleteRepairStatus(int id)
	{
		await _db.CreateConnection().QueryAsync("DELETE FROM repair_status WHERE id = @id", new { id }, commandTimeout: _sqlCommandTimeout);
	}
	public async Task<RepairStatus[]> ReadRepairStatus(int? id = null)
	{
		const string sql = @"
			select
				id as Id,
				name as Name
			from repair_status
			where @IdRepairStatus is null or id = @IdRepairStatus";

		var data = await _db.CreateConnection().QueryAsync<RepairStatus>(sql, new { IdRepairStatus = id }, commandTimeout: _sqlCommandTimeout);
		return data.ToArray();
	}
}