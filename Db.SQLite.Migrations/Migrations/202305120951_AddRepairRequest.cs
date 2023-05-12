using FluentMigrator;

namespace Db.SQLite.Migrations;

[Migration(202305120951)]
public class AddRepairRequest : Migration
{
	public override void Up()
	{
		const string tableRepairStatus = "repair_status";
		AddRepairStatusTable(tableRepairStatus);

		const string tableRepairRequest = "repair_request";

		Create.Table(tableRepairRequest).
			WithColumn("id").AsInt32().PrimaryKey().Identity().
			WithColumn("status").AsInt32().NotNullable().
			WithColumn("date_request").AsDateTime().NotNullable().
			WithColumn("address").AsString().Nullable().
			WithColumn("date_desired").AsDateTime().Nullable().
			WithColumn("description").AsString().Nullable().
			WithColumn("user_id").AsInt64().Nullable();

		Create.ForeignKey().
			FromTable(tableRepairRequest).ForeignColumn("status").
			ToTable(tableRepairStatus).PrimaryColumn("id");
	}

	public override void Down()
	{
		
	}

	private void AddRepairStatusTable(string table)
	{
		Create.Table(table).
			WithColumn("id").AsInt32().PrimaryKey().Identity().
			WithColumn("name").AsString().NotNullable();

		Insert.IntoTable(table).
			Row(new { name = "В обработке" }).
			Row(new { name = "Ожидание" }).
			Row(new { name = "В работе" }).
			Row(new { name = "Выполнена" });
	}
}
