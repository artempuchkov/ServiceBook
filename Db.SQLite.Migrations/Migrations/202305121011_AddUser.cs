using FluentMigrator;

namespace Db.SQLite.Migrations;

[Migration(202305121011)]
public class AddUser : Migration
{
	public override void Up()
	{
		AddBonusCardTable();
		AddCarTable();

		Create.Table("user").
			WithColumn("user_id").AsInt64().PrimaryKey().Identity().
			WithColumn("user_code").AsString().NotNullable().

			WithColumn("email").AsString().NotNullable().
			WithColumn("confirm_email").AsBoolean().NotNullable().WithDefaultValue(false).
			WithColumn("password").AsString().NotNullable().

			WithColumn("number_phone").AsString().NotNullable().
			WithColumn("fio").AsString().NotNullable().
				
			WithColumn("bonus_card").AsString().Nullable();
	}

	public override void Down()
	{
		
	}

	private void AddBonusCardTable()
	{
		Create.Table("bonus_card").
			WithColumn("id").AsString().PrimaryKey().
			WithColumn("score").AsInt64().NotNullable().WithDefaultValue(0);
	}

	private void AddCarTable()
	{
		Create.Table("car").
			WithColumn("vin").AsString().PrimaryKey().
			WithColumn("brand").AsString().NotNullable().
			WithColumn("model").AsString().NotNullable().
			WithColumn("release_year").AsInt64().NotNullable().
			WithColumn("state_number").AsString().Nullable().
			WithColumn("user_id").AsInt64().NotNullable().
			WithColumn("service_book_id").AsInt64().Nullable();
	}
}
