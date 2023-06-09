using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.SQLite.Migrations.Migrations
{
    [Migration(202306091923)]
    public class AddForeignKeyToReception : Migration
    {
        public override void Down()
        {
        }

        public override void Up()
        {
            Create.Column("master").OnTable("reception").AsInt64().NotNullable();

            Create.ForeignKey().
                FromTable("reception").ForeignColumn("master").
                ToTable("master").PrimaryColumn("id");
        }
    }
}
