using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.App.EF.Migrations
{
    public partial class CascadeDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Questions_QuestionId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUserAnswerables_Answerables_AnswerableId",
                table: "AppUserAnswerables");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionInAnswerables_Answerables_AnswerableId",
                table: "QuestionInAnswerables");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionInAnswerables_Questions_QuestionId",
                table: "QuestionInAnswerables");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Questions_QuestionId",
                table: "Answers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserAnswerables_Answerables_AnswerableId",
                table: "AppUserAnswerables",
                column: "AnswerableId",
                principalTable: "Answerables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionInAnswerables_Answerables_AnswerableId",
                table: "QuestionInAnswerables",
                column: "AnswerableId",
                principalTable: "Answerables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionInAnswerables_Questions_QuestionId",
                table: "QuestionInAnswerables",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Questions_QuestionId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUserAnswerables_Answerables_AnswerableId",
                table: "AppUserAnswerables");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionInAnswerables_Answerables_AnswerableId",
                table: "QuestionInAnswerables");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionInAnswerables_Questions_QuestionId",
                table: "QuestionInAnswerables");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Questions_QuestionId",
                table: "Answers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserAnswerables_Answerables_AnswerableId",
                table: "AppUserAnswerables",
                column: "AnswerableId",
                principalTable: "Answerables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionInAnswerables_Answerables_AnswerableId",
                table: "QuestionInAnswerables",
                column: "AnswerableId",
                principalTable: "Answerables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionInAnswerables_Questions_QuestionId",
                table: "QuestionInAnswerables",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
