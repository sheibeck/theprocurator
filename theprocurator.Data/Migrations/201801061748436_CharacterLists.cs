namespace theprocurator.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CharacterLists : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Character", new[] { "UserId" });
            CreateTable(
                "dbo.CharacterList",
                c => new
                    {
                        CharacterListId = c.Guid(nullable: false, identity: true),
                        CharacterListName = c.String(nullable: false, maxLength: 250),
                        UpdatedOn = c.DateTime(nullable: false),
                        Published = c.Boolean(nullable: false),
                        UserId = c.String(nullable: false),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CharacterListId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.CharacterListCharacter",
                c => new
                    {
                        CharacterList_CharacterListId = c.Guid(nullable: false),
                        Character_CharacterId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.CharacterList_CharacterListId, t.Character_CharacterId })
                .ForeignKey("dbo.CharacterList", t => t.CharacterList_CharacterListId, cascadeDelete: true)
                .ForeignKey("dbo.Character", t => t.Character_CharacterId, cascadeDelete: true)
                .Index(t => t.CharacterList_CharacterListId)
                .Index(t => t.Character_CharacterId);
            
            AlterColumn("dbo.Character", "UserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Character", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CharacterList", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.CharacterListCharacter", "Character_CharacterId", "dbo.Character");
            DropForeignKey("dbo.CharacterListCharacter", "CharacterList_CharacterListId", "dbo.CharacterList");
            DropIndex("dbo.CharacterListCharacter", new[] { "Character_CharacterId" });
            DropIndex("dbo.CharacterListCharacter", new[] { "CharacterList_CharacterListId" });
            DropIndex("dbo.CharacterList", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Character", new[] { "UserId" });
            AlterColumn("dbo.Character", "UserId", c => c.String(maxLength: 128));
            DropTable("dbo.CharacterListCharacter");
            DropTable("dbo.CharacterList");
            CreateIndex("dbo.Character", "UserId");
        }
    }
}
