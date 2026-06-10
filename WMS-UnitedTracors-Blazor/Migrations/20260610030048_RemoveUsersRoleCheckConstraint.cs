using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ut_wms_asp.net.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUsersRoleCheckConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE users DROP CHECK CK_users_role;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE users ADD CONSTRAINT CK_users_role CHECK (`role` IN ('staff', 'manager', 'admin', 'superadmin'));");
        }
    }
}
