using FluentMigrator;


namespace Db.SQLite.Migrations.Migrations
{
	[Migration(202305221512)]
	public class AddCategory : Migration
	{
		public override void Down()
		{

		}

		public override void Up()
		{
			const string tableCategory = "category";
			Create.Table(tableCategory).
			WithColumn("category_id").AsInt64().PrimaryKey().Identity().
			WithColumn("category_name").AsString().NotNullable();           
        }
	}
}