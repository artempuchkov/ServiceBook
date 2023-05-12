using System.Text.RegularExpressions;

namespace Db.SQLite.Migrations;

public static class OtherExtensions
{
	private static readonly Regex _sqLiteConnStringRegex = new Regex(@"Data Source=(?<dbpath>[^;]+)(;.*)?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	public static void EnsureSQLiteDbPath(this string connectionString)
	{
		var match = _sqLiteConnStringRegex.Match(connectionString);

		if (match.Success)
		{
			var dbPath = match.Groups["dbpath"]?.Value.Trim();
			if (string.IsNullOrEmpty(dbPath))
				return;

			var directory = Path.GetDirectoryName(dbPath);
			if (!string.IsNullOrEmpty(directory))
			{
				Directory.CreateDirectory(directory);
			}
		}
		else
		{
			throw new Exception($"Error on parse connectionString. '{connectionString}'.");
		}
	}
}
