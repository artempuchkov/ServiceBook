using System.Data;
using System.Data.SQLite;
using Microsoft.Extensions.Configuration;

namespace ServiceBook.Db.SQLite;
public class SQLiteDatabase : IDbConnectionFactory
{
	private readonly string _connectionString;

	public SQLiteDatabase(IConfiguration configuration)
	{
		_connectionString = configuration.GetSection("connectionStrings:default").Value;
	}

	public IDbConnection OpenConnection()
	{
		var conn = CreateConnection();
		conn.Open();
		return conn;
	}

	public IDbConnection CreateConnection()
	{
		if (string.IsNullOrEmpty(_connectionString))
			throw new Exception("SQLiteDatabase connection string is empty.");

		return new SQLiteConnection(_connectionString);
	}
}