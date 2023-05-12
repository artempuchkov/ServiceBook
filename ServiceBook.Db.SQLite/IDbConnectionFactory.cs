using System.Data;

namespace ServiceBook.Db.SQLite;

public interface IDbConnectionFactory
{
	IDbConnection CreateConnection();
	IDbConnection OpenConnection();
}