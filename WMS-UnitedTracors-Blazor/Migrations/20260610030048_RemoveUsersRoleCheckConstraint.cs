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
            migrationBuilder.Sql(@"
                DROP PROCEDURE IF EXISTS DropCheckConstraint;
                CREATE PROCEDURE DropCheckConstraint()
                BEGIN
                    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
                    SET @sql1 = 'ALTER TABLE users DROP CONSTRAINT CK_users_role';
                    PREPARE stmt1 FROM @sql1;
                    EXECUTE stmt1;
                    DEALLOCATE PREPARE stmt1;
                    
                    SET @sql2 = 'ALTER TABLE users DROP CHECK CK_users_role';
                    PREPARE stmt2 FROM @sql2;
                    EXECUTE stmt2;
                    DEALLOCATE PREPARE stmt2;
                END;
                CALL DropCheckConstraint();
                DROP PROCEDURE DropCheckConstraint;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE users ADD CONSTRAINT CK_users_role CHECK (`role` IN ('staff', 'manager', 'admin', 'superadmin'));");
        }
    }
}
