using FluentMigrator;


namespace Db.SQLite.Migrations.Migrations
{
    [Migration(202305160602)]
    public class AddService : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            const string tableService = "service";

            Create.Table(tableService).
            WithColumn("service_id").AsInt64().PrimaryKey().Identity().
            WithColumn("name").AsString().NotNullable().
            WithColumn("description").AsString().NotNullable().
            WithColumn("price").AsInt64().NotNullable().
            WithColumn("category").AsInt64().NotNullable().
            WithColumn("img").AsString().NotNullable();
        }
    }
}
