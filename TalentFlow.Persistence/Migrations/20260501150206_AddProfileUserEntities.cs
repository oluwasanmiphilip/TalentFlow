using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentFlow.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileUserEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseProgresses_userss_UserId",
                table: "CourseProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_userss_UserId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonProgresses_userss_UserId",
                table: "LessonProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_notification_userss_UserId",
                table: "notification");

            migrationBuilder.DropForeignKey(
                name: "FK_profile_users_userss_UserId",
                table: "profile_users");

            migrationBuilder.DropForeignKey(
                name: "FK_userss_role_RoleId",
                table: "userss");

            migrationBuilder.DropPrimaryKey(
                name: "PK_userss",
                table: "userss");

            migrationBuilder.RenameTable(
                name: "userss",
                newName: "users");

            migrationBuilder.RenameIndex(
                name: "IX_userss_RoleId",
                table: "users",
                newName: "IX_users_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_userss_LearnerId",
                table: "users",
                newName: "IX_users_LearnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseProgresses_users_UserId",
                table: "CourseProgresses",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_users_UserId",
                table: "Enrollments",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonProgresses_users_UserId",
                table: "LessonProgresses",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_notification_users_UserId",
                table: "notification",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_profile_users_users_UserId",
                table: "profile_users",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_role_RoleId",
                table: "users",
                column: "RoleId",
                principalTable: "role",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseProgresses_users_UserId",
                table: "CourseProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_users_UserId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonProgresses_users_UserId",
                table: "LessonProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_notification_users_UserId",
                table: "notification");

            migrationBuilder.DropForeignKey(
                name: "FK_profile_users_users_UserId",
                table: "profile_users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_role_RoleId",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "userss");

            migrationBuilder.RenameIndex(
                name: "IX_users_RoleId",
                table: "userss",
                newName: "IX_userss_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_users_LearnerId",
                table: "userss",
                newName: "IX_userss_LearnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_userss",
                table: "userss",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseProgresses_userss_UserId",
                table: "CourseProgresses",
                column: "UserId",
                principalTable: "userss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_userss_UserId",
                table: "Enrollments",
                column: "UserId",
                principalTable: "userss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonProgresses_userss_UserId",
                table: "LessonProgresses",
                column: "UserId",
                principalTable: "userss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_notification_userss_UserId",
                table: "notification",
                column: "UserId",
                principalTable: "userss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_profile_users_userss_UserId",
                table: "profile_users",
                column: "UserId",
                principalTable: "userss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_userss_role_RoleId",
                table: "userss",
                column: "RoleId",
                principalTable: "role",
                principalColumn: "Id");
        }
    }
}
