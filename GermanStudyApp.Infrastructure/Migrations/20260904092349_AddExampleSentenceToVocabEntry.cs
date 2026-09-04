using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GermanStudyApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExampleSentenceToVocabEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExampleSentence",
                table: "VocabEntries",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExampleSentence",
                table: "VocabEntries");
        }
    }
}
