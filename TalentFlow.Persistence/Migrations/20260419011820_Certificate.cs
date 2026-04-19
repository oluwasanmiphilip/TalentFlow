using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentFlow.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Certificate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_lesson_course_CourseId",
                table: "lesson");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonProgresses_lesson_LessonId",
                table: "LessonProgresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_lesson",
                table: "lesson");

            migrationBuilder.RenameTable(
                name: "lesson",
                newName: "Lessons");

            migrationBuilder.RenameIndex(
                name: "IX_lesson_CourseId",
                table: "Lessons",
                newName: "IX_Lessons_CourseId");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "VideoPosition",
                table: "LessonProgresses",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertificateUrl",
                table: "certificate",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContentUrl",
                table: "Lessons",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Lessons",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lessons",
                table: "Lessons",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonProgresses_Lessons_LessonId",
                table: "LessonProgresses",
                column: "LessonId",
                principalTable: "Lessons",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonProgresses_Lessons_LessonId",
                table: "LessonProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_course_CourseId",
                table: "Lessons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lessons",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "VideoPosition",
                table: "LessonProgresses");

            migrationBuilder.DropColumn(
                name: "CertificateUrl",
                table: "certificate");

            migrationBuilder.DropColumn(
                name: "ContentUrl",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Lessons");

            migrationBuilder.RenameTable(
                name: "Lessons",
                newName: "lesson");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_CourseId",
                table: "lesson",
                newName: "IX_lesson_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_lesson",
                table: "lesson",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_lesson_course_CourseId",
                table: "lesson",
                column: "CourseId",
                principalTable: "course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonProgresses_lesson_LessonId",
                table: "LessonProgresses",
                column: "LessonId",
                principalTable: "lesson",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
