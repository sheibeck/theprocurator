namespace theprocurator.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserIdUpdate : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.CharacterSheet", new[] { "UserId" });                        
            AlterColumn("dbo.CharacterSheet", "UserId", c => c.String(nullable: false, maxLength: 128));            
            CreateIndex("dbo.CharacterSheet", "UserId");            
        }
        
        public override void Down()
        {            
            DropIndex("dbo.CharacterSheet", new[] { "UserId" });            
            AlterColumn("dbo.CharacterSheet", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.CharacterSheet", "UserId");            
        }
    }
}
