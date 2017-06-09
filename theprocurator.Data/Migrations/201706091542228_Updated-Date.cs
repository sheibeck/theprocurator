namespace theprocurator.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "UpdatedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.CharacterSheet", "CharacterSheetParentId", c => c.String());
            AddColumn("dbo.CharacterSheet", "UpdatedOn", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CharacterSheet", "UpdatedOn");
            DropColumn("dbo.CharacterSheet", "CharacterSheetParentId");
            DropColumn("dbo.Character", "UpdatedOn");
        }
    }
}
