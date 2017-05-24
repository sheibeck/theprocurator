namespace theprocurator.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Base : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Character",
                c => new
                    {
                        CharacterId = c.Guid(nullable: false, identity: true),
                        CharacterName = c.String(nullable: false, maxLength: 250),
                        CharacterUrl = c.String(nullable: false, maxLength: 250),
                        CharacterData = c.String(nullable: false),
                        CharacerSheetId = c.Guid(nullable: false),
                        UserId = c.String(maxLength: 128),
                        CharacterSheet_CharacterSheetId = c.Guid(),
                    })
                .PrimaryKey(t => t.CharacterId)
                .ForeignKey("dbo.CharacterSheet", t => t.CharacterSheet_CharacterSheetId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CharacterSheet_CharacterSheetId);
            
            CreateTable(
                "dbo.CharacterSheet",
                c => new
                    {
                        CharacterSheetId = c.Guid(nullable: false),
                        CharacterSheetName = c.String(nullable: false, maxLength: 250),
                        CharacterSheetUrl = c.String(nullable: false, maxLength: 250),
                        CharacterSheetForm = c.String(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CharacterSheetId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.CharacterHistory",
                c => new
                    {
                        HistoryId = c.Int(nullable: false, identity: true),
                        Version = c.String(nullable: false),
                        Date = c.String(nullable: false),
                        CharacterId = c.Guid(nullable: false),
                        CharacterName = c.String(nullable: false, maxLength: 250),
                        CharacterUrl = c.String(nullable: false, maxLength: 250),
                        CharacterData = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.HistoryId);
            
            CreateTable(
                "dbo.CharacterSheetHistory",
                c => new
                    {
                        HistoryId = c.Int(nullable: false, identity: true),
                        Version = c.String(nullable: false),
                        Date = c.String(nullable: false),
                        CharacterSheetId = c.Guid(nullable: false),
                        CharacterSheetName = c.String(nullable: false, maxLength: 250),
                        CharacterSheetUrl = c.String(nullable: false, maxLength: 250),
                        CharacterSheetForm = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.HistoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CharacterSheet", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Character", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Character", "CharacterSheet_CharacterSheetId", "dbo.CharacterSheet");
            DropIndex("dbo.CharacterSheet", new[] { "UserId" });
            DropIndex("dbo.Character", new[] { "CharacterSheet_CharacterSheetId" });
            DropIndex("dbo.Character", new[] { "UserId" });
            DropTable("dbo.CharacterSheetHistory");
            DropTable("dbo.CharacterHistory");
            DropTable("dbo.CharacterSheet");
            DropTable("dbo.Character");
        }
    }
}
