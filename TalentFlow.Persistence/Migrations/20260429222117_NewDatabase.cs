using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentFlow.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NewDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseProgresses_course_CourseId",
                table: "CourseProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_enrollment_course_CourseId",
                table: "enrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_enrollment_course_CourseId1",
                table: "enrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_enrollment_userss_UserId",
                table: "enrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_course_CourseId",
                table: "Lessons");

            migrationBuilder.DropTable(
                name: "course");

            migrationBuilder.DropPrimaryKey(
                name: "PK_enrollment",
                table: "enrollment");

            migrationBuilder.DropColumn(
                name: "FirstLessonId",
                table: "enrollment");

            migrationBuilder.RenameTable(
                name: "enrollment",
                newName: "Enrollments");

            migrationBuilder.RenameIndex(
                name: "IX_enrollment_UserId",
                table: "Enrollments",
                newName: "IX_Enrollments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_enrollment_CourseId1",
                table: "Enrollments",
                newName: "IX_Enrollments_CourseId1");

            migrationBuilder.RenameIndex(
                name: "IX_enrollment_CourseId",
                table: "Enrollments",
                newName: "IX_Enrollments_CourseId");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Enrollments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Enrollments",
                table: "Enrollments",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    InstructorId = table.Column<Guid>(type: "uuid", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_instructor_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "instructor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_certificate_CourseId",
                table: "certificate",
                column: "CourseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_InstructorId",
                table: "Courses",
                column: "InstructorId");

            migrationBuilder.AddForeignKey(
                name: "FK_certificate_Courses_CourseId",
                table: "certificate",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseProgresses_Courses_CourseId",
                table: "CourseProgresses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Courses_CourseId",
                table: "Enrollments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Courses_CourseId1",
                table: "Enrollments",
                column: "CourseId1",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_userss_UserId",
                table: "Enrollments",
                column: "UserId",
                principalTable: "userss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Courses_CourseId",
                table: "Lessons",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_certificate_Courses_CourseId",
                table: "certificate");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseProgresses_Courses_CourseId",
                table: "CourseProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Courses_CourseId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Courses_CourseId1",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_userss_UserId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Courses_CourseId",
                table: "Lessons");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_certificate_CourseId",
                table: "certificate");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Enrollments",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Enrollments");

            migrationBuilder.RenameTable(
                name: "Enrollments",
                newName: "enrollment");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollments_UserId",
                table: "enrollment",
                newName: "IX_enrollment_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollments_CourseId1",
                table: "enrollment",
                newName: "IX_enrollment_CourseId1");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollments_CourseId",
                table: "enrollment",
                newName: "IX_enrollment_CourseId");

            migrationBuilder.AddColumn<Guid>(
                name: "FirstLessonId",
                table: "enrollment",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_enrollment",
                table: "enrollment",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "course",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CourseProgresses_course_CourseId",
                table: "CourseProgresses",
                column: "CourseId",
                principalTable: "course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_enrollment_course_CourseId",
                table: "enrollment",
                column: "CourseId",
                principalTable: "course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_enrollment_course_CourseId1",
                table: "enrollment",
                column: "CourseId1",
                principalTable: "course",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_enrollment_userss_UserId",
                table: "enrollment",
                column: "UserId",
                principalTable: "userss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_course_CourseId",
                table: "Lessons",
                column: "CourseId",
                principalTable: "course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
