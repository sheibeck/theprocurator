namespace theprocurator.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AspNetUserIdentity : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.AspNetUserLogins", new[] { "IdentityUser_Id" });
            DropIndex("dbo.UserClaim", new[] { "IdentityUser_Id" });
            DropIndex("dbo.UserLogin", new[] { "IdentityUser_Id" });

            DropForeignKey("dbo.UserLogin", "IdentityUser_Id", "dbo.User");
            DropForeignKey("dbo.AspNetUserLogins", "IdentityUser_Id", "dbo.User");
            DropForeignKey("dbo.UserClaim", "IdentityUser_Id", "dbo.User");

            DropColumn("dbo.AspNetUserLogins", "IdentityUser_Id");

         
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUserLogins", "IdentityUser_Id", c => c.String(nullable: true, maxLength: 128));
            CreateIndex("dbo.AspNetUserLogins", "IdentityUser_Id");

            CreateIndex("dbo.AspNetUserLogins", new[] { "IdentityUser_Id" });
            CreateIndex("dbo.UserClaim", new[] { "IdentityUser_Id" });
            CreateIndex("dbo.UserLogin", new[] { "IdentityUser_Id" });

            AddForeignKey("dbo.UserLogin", "IdentityUser_Id", "dbo.User");
            AddForeignKey("dbo.AspNetUserLogins", "IdentityUser_Id", "dbo.User");
            AddForeignKey("dbo.UserClaim", "IdentityUser_Id", "dbo.User");

        }
    }
}
