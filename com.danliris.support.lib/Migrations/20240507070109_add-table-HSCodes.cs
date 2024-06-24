using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace com.danliris.support.lib.Migrations
{
    public partial class addtableHSCodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {       
            migrationBuilder.CreateTable(
                name: "HSCodes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    HSNo = table.Column<string>(nullable: true),
                    HSRemark = table.Column<string>(nullable: true),
                    _CreatedAgent = table.Column<string>(nullable: true),
                    _CreatedBy = table.Column<string>(nullable: true),
                    _CreatedUtc = table.Column<DateTime>(nullable: false),
                    _DeletedAgent = table.Column<string>(nullable: true),
                    _DeletedBy = table.Column<string>(nullable: true),
                    _DeletedUtc = table.Column<DateTime>(nullable: false),
                    _IsDeleted = table.Column<bool>(nullable: false),
                    _LastModifiedAgent = table.Column<string>(nullable: true),
                    _LastModifiedBy = table.Column<string>(nullable: true),
                    _LastModifiedUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HSCodes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HSCodes");

        }
    }
}
