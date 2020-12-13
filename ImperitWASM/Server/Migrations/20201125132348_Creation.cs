using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImperitWASM.Server.Migrations
{
	public partial class Creation : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.CreateTable(
				name: "Games",
				columns: table => new
				{
					Id = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					Active = table.Column<int>(type: "INTEGER", nullable: false),
					Current = table.Column<int>(type: "INTEGER", nullable: false),
					LastChange = table.Column<DateTime>(type: "TEXT", nullable: false)
				},
				constraints: table => table.PrimaryKey("PK_Games", x => x.Id));

			_ = migrationBuilder.CreateTable(
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
					_ = table.PrimaryKey("PK_EntityPlayerPowers", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_EntityPlayerPowers_Games_GameId",
						column: x => x.GameId,
						principalTable: "Games",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
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
					_ = table.PrimaryKey("PK_EntityPlayers", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_EntityPlayers_Games_GameId",
						column: x => x.GameId,
						principalTable: "Games",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "EntityProvinces",
				columns: table => new
				{
					Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
					GameId = table.Column<int>(type: "INTEGER", nullable: false),
					Index = table.Column<int>(type: "INTEGER", nullable: false)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_EntityProvinces", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_EntityProvinces_Games_GameId",
						column: x => x.GameId,
						principalTable: "Games",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "EntitySessions",
				columns: table => new
				{
					Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
					GameId = table.Column<int>(type: "INTEGER", nullable: false),
					PlayerIndex = table.Column<int>(type: "INTEGER", nullable: false),
					SessionKey = table.Column<string>(type: "TEXT", nullable: false)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_EntitySessions", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_EntitySessions_Games_GameId",
						column: x => x.GameId,
						principalTable: "Games",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "EntityPlayerAction",
				columns: table => new
				{
					Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
					EntityPlayerId = table.Column<int>(type: "INTEGER", nullable: false),
					Type = table.Column<int>(type: "INTEGER", nullable: false),
					Debt = table.Column<int>(type: "INTEGER", nullable: true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_EntityPlayerAction", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_EntityPlayerAction_EntityPlayers_EntityPlayerId",
						column: x => x.EntityPlayerId,
						principalTable: "EntityPlayers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "EntityProvinceAction",
				columns: table => new
				{
					Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
					EntityProvinceId = table.Column<int>(type: "INTEGER", nullable: false),
					Type = table.Column<int>(type: "INTEGER", nullable: false),
					EntityPlayerId = table.Column<int>(type: "INTEGER", nullable: false)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_EntityProvinceAction", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_EntityProvinceAction_EntityPlayers_EntityPlayerId",
						column: x => x.EntityPlayerId,
						principalTable: "EntityPlayers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					_ = table.ForeignKey(
						name: "FK_EntityProvinceAction_EntityProvinces_EntityProvinceId",
						column: x => x.EntityProvinceId,
						principalTable: "EntityProvinces",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "EntitySoldier",
				columns: table => new
				{
					Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
					EntityProvinceActionId = table.Column<int>(type: "INTEGER", nullable: false),
					Type = table.Column<int>(type: "INTEGER", nullable: false),
					Count = table.Column<int>(type: "INTEGER", nullable: false)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_EntitySoldier", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_EntitySoldier_EntityProvinceAction_EntityProvinceActionId",
						column: x => x.EntityProvinceActionId,
						principalTable: "EntityProvinceAction",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateIndex(
				name: "IX_EntityPlayerAction_EntityPlayerId",
				table: "EntityPlayerAction",
				column: "EntityPlayerId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_EntityPlayerPowers_GameId",
				table: "EntityPlayerPowers",
				column: "GameId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_EntityPlayers_GameId",
				table: "EntityPlayers",
				column: "GameId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_EntityProvinceAction_EntityPlayerId",
				table: "EntityProvinceAction",
				column: "EntityPlayerId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_EntityProvinceAction_EntityProvinceId",
				table: "EntityProvinceAction",
				column: "EntityProvinceId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_EntityProvinces_GameId",
				table: "EntityProvinces",
				column: "GameId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_EntitySessions_GameId",
				table: "EntitySessions",
				column: "GameId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_EntitySoldier_EntityProvinceActionId",
				table: "EntitySoldier",
				column: "EntityProvinceActionId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.DropTable(name: "EntityPlayerAction");
			_ = migrationBuilder.DropTable(name: "EntityPlayerPowers");
			_ = migrationBuilder.DropTable(name: "EntitySessions");
			_ = migrationBuilder.DropTable(name: "EntitySoldier");
			_ = migrationBuilder.DropTable(name: "EntityProvinceAction");
			_ = migrationBuilder.DropTable(name: "EntityPlayers");
			_ = migrationBuilder.DropTable(name: "EntityProvinces");
			_ = migrationBuilder.DropTable(name: "Games");
		}
	}
}
