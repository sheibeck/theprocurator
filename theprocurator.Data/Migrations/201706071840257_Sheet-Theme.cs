namespace theprocurator.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SheetTheme : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CharacterSheet", "CharacterSheetTheme", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CharacterSheet", "CharacterSheetTheme");
        }
    }
}
