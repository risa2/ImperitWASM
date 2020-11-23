using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImperitWASM.Server.Migrations
{
    public partial class OneIM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EntitySoldier",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntitySoldier", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Active = table.Column<int>(type: "INTEGER", nullable: false),
                    Current = table.Column<int>(type: "INTEGER", nullable: false),
                    LastChange = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntitySoldierPair",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntitySoldierId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Count = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntitySoldierPair", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntitySoldierPair_EntitySoldier_EntitySoldierId",
                        column: x => x.EntitySoldierId,
                        principalTable: "EntitySoldier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityPlayerPowers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    Alive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Soldiers = table.Column<int>(type: "INTEGER", nullable: false),
                    Lands = table.Column<int>(type: "INTEGER", nullable: false),
                    Income = table.Column<int>(type: "INTEGER", nullable: false),
                    Money = table.Column<int>(type: "INTEGER", nullable: false),
                    Final = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityPlayerPowers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityPlayerPowers_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityPlayers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    Index = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Color = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Money = table.Column<int>(type: "INTEGER", nullable: false),
                    Alive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityPlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityPlayers_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntitySessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    SessionKey = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntitySessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntitySessions_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityPlayerAction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntityPlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Debt = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityPlayerAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityPlayerAction_EntityPlayers_EntityPlayerId",
                        column: x => x.EntityPlayerId,
                        principalTable: "EntityPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityProvinces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    Index = table.Column<int>(type: "INTEGER", nullable: false),
                    EntityPlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    EntitySoldierId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityProvinces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityProvinces_EntityPlayers_EntityPlayerId",
                        column: x => x.EntityPlayerId,
                        principalTable: "EntityPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityProvinces_EntitySoldier_EntitySoldierId",
                        column: x => x.EntitySoldierId,
                        principalTable: "EntitySoldier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityProvinces_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityProvinceAction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntityProvinceId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    EntityPlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    EntitySoldierId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityProvinceAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityProvinceAction_EntityPlayers_EntityPlayerId",
                        column: x => x.EntityPlayerId,
                        principalTable: "EntityPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityProvinceAction_EntityProvinces_EntityProvinceId",
                        column: x => x.EntityProvinceId,
                        principalTable: "EntityProvinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityProvinceAction_EntitySoldier_EntitySoldierId",
                        column: x => x.EntitySoldierId,
                        principalTable: "EntitySoldier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityPlayerAction_EntityPlayerId",
                table: "EntityPlayerAction",
                column: "EntityPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityPlayerPowers_GameId",
                table: "EntityPlayerPowers",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityPlayers_GameId",
                table: "EntityPlayers",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityProvinceAction_EntityPlayerId",
                table: "EntityProvinceAction",
                column: "EntityPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityProvinceAction_EntityProvinceId",
                table: "EntityProvinceAction",
                column: "EntityProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityProvinceAction_EntitySoldierId",
                table: "EntityProvinceAction",
                column: "EntitySoldierId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityProvinces_EntityPlayerId",
                table: "EntityProvinces",
                column: "EntityPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityProvinces_EntitySoldierId",
                table: "EntityProvinces",
                column: "EntitySoldierId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityProvinces_GameId",
                table: "EntityProvinces",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySessions_GameId",
                table: "EntitySessions",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySoldierPair_EntitySoldierId",
                table: "EntitySoldierPair",
                column: "EntitySoldierId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityPlayerAction");

            migrationBuilder.DropTable(
                name: "EntityPlayerPowers");

            migrationBuilder.DropTable(
                name: "EntityProvinceAction");

            migrationBuilder.DropTable(
                name: "EntitySessions");

            migrationBuilder.DropTable(
                name: "EntitySoldierPair");

            migrationBuilder.DropTable(
                name: "EntityProvinces");

            migrationBuilder.DropTable(
                name: "EntityPlayers");

            migrationBuilder.DropTable(
                name: "EntitySoldier");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
