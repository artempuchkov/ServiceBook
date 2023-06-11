using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Db.SQLite.Migrations.Migrations
{
    [Migration(202306102130)]
    public class AddWorkMode : Migration
    {
        public override void Down()
        {
        }

        public override void Up()
        {
            Create.Table("working_mode").
                WithColumn("id").AsInt32().PrimaryKey().Identity().
                WithColumn("day").AsString().NotNullable().
                WithColumn("time_start").AsTime().NotNullable().
                WithColumn("time_end").AsTime().NotNullable();
        }
    }
}
