using FluentMigrator.Runner.VersionTableInfo;

namespace Db.SQLite.Migrations;

[VersionTableMetaData]
public class VersionTable : IVersionTableMetaData
{
	public string ColumnName => "Version";

	public object? ApplicationContext { get; set; }

	public bool OwnsSchema => true;

	public string SchemaName => "";

	public string TableName => "_VersionInfo";

	public string UniqueIndexName => "UC_VersionInfo_Version";

	public virtual string AppliedOnColumnName => "AppliedOn";

	public virtual string DescriptionColumnName => "Description";
}