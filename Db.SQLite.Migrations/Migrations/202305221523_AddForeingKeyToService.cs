using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.SQLite.Migrations.Migrations
{
    [Migration(202305221523)]
    public class AddForeingKeyToService : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Create.Column("category").OnTable("service").AsInt64().NotNullable();

            Create.ForeignKey().
                FromTable("service").ForeignColumn("category").
                ToTable("category").PrimaryColumn("category_id");
        }
    }
}
