using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentFlow.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TableUserchangUserss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseProgresses_user_UserId",
                table: "CourseProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_enrollment_user_UserId",
                table: "enrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonProgresses_user_UserId",
                table: "LessonProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_notification_user_UserId",
                table: "notification");

            migrationBuilder.DropForeignKey(
                name: "FK_user_role_RoleId",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user",
                table: "user");

            migrationBuilder.RenameTable(
                name: "user",
                newName: "userss");

            migrationBuilder.RenameIndex(
                name: "IX_user_RoleId",
                table: "userss",
                newName: "IX_userss_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_user_LearnerId",
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
                name: "FK_enrollment_userss_UserId",
                table: "enrollment",
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
                name: "FK_userss_role_RoleId",
                table: "userss",
                column: "RoleId",
                principalTable: "role",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseProgresses_userss_UserId",
                table: "CourseProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_enrollment_userss_UserId",
                table: "enrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonProgresses_userss_UserId",
                table: "LessonProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_notification_userss_UserId",
                table: "notification");

            migrationBuilder.DropForeignKey(
                name: "FK_userss_role_RoleId",
                table: "userss");

            migrationBuilder.DropPrimaryKey(
                name: "PK_userss",
                table: "userss");

            migrationBuilder.RenameTable(
                name: "userss",
                newName: "user");

            migrationBuilder.RenameIndex(
                name: "IX_userss_RoleId",
                table: "user",
                newName: "IX_user_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_userss_LearnerId",
                table: "user",
                newName: "IX_user_LearnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user",
                table: "user",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseProgresses_user_UserId",
                table: "CourseProgresses",
                column: "UserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_enrollment_user_UserId",
                table: "enrollment",
                column: "UserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonProgresses_user_UserId",
                table: "LessonProgresses",
                column: "UserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_notification_user_UserId",
                table: "notification",
                column: "UserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_role_RoleId",
                table: "user",
                column: "RoleId",
                principalTable: "role",
                principalColumn: "Id");
        }
    }
}
