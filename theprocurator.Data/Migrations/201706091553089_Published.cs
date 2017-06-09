namespace theprocurator.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Published : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.CharacterSheet", "Published", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CharacterSheet", "Published");
            DropColumn("dbo.Character", "Published");
        }
    }
}
