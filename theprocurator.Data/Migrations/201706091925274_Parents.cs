namespace theprocurator.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Parents : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "ParentId", c => c.Guid(nullable: false));
            AddColumn("dbo.CharacterSheet", "ParentId", c => c.Guid(nullable: false));
            DropColumn("dbo.CharacterSheet", "CharacterSheetParentId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CharacterSheet", "CharacterSheetParentId", c => c.String());
            DropColumn("dbo.CharacterSheet", "ParentId");
            DropColumn("dbo.Character", "ParentId");
        }
    }
}
