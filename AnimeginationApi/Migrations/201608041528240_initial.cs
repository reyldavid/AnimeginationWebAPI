namespace AnimeginationApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryID = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 1000),
                        ImageFile = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.CategoryID);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductID = c.Int(nullable: false, identity: true),
                        ProductCode = c.String(nullable: false, maxLength: 10),
                        ProductTitle = c.String(nullable: false, maxLength: 100),
                        ProductDescription = c.String(nullable: false, maxLength: 4000),
                        UnitPrice = c.Double(nullable: false),
                        YourPrice = c.Double(nullable: false),
                        CategoryID = c.Int(nullable: false),
                        ProductAgeRating = c.String(maxLength: 20),
                        ProductLength = c.Int(),
                        ProductYearCreated = c.Int(),
                        MediumID = c.Int(),
                        PublisherID = c.Int(nullable: false),
                        ProductImageURL = c.String(),
                        OnSale = c.Boolean(nullable: false),
                        RatingID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductID)
                .ForeignKey("dbo.Categories", t => t.CategoryID, cascadeDelete: true)
                .ForeignKey("dbo.Media", t => t.MediumID)
                .ForeignKey("dbo.Publishers", t => t.PublisherID, cascadeDelete: true)
                .Index(t => t.CategoryID)
                .Index(t => t.MediumID)
                .Index(t => t.PublisherID);
            
            CreateTable(
                "dbo.Listings",
                c => new
                    {
                        ListingID = c.Int(nullable: false, identity: true),
                        ListTypeID = c.Int(nullable: false),
                        Rank = c.Int(nullable: false),
                        ProductID = c.Int(nullable: false),
                        Effective = c.DateTime(nullable: false),
                        Expiration = c.DateTime(nullable: false),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ListingID)
                .ForeignKey("dbo.ListTypes", t => t.ListTypeID, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.ListTypeID)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.ListTypes",
                c => new
                    {
                        ListTypeID = c.Int(nullable: false, identity: true),
                        ListTypeName = c.String(maxLength: 100),
                        Description = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.ListTypeID);
            
            CreateTable(
                "dbo.Media",
                c => new
                    {
                        MediumID = c.Int(nullable: false, identity: true),
                        MediumName = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.MediumID);
            
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        OrderItemID = c.Int(nullable: false, identity: true),
                        OrderID = c.Int(nullable: false),
                        ProductID = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        FinalUnitPrice = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.OrderItemID)
                .ForeignKey("dbo.Orders", t => t.OrderID, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.OrderID)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderID = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ShippingHandling = c.Double(nullable: false),
                        Taxes = c.Double(nullable: false),
                        Discounts = c.Double(nullable: false),
                        OrderDate = c.DateTime(nullable: false),
                        IsPurchased = c.Boolean(nullable: false),
                        TrackingNumber = c.String(),
                    })
                .PrimaryKey(t => t.OrderID)
                .ForeignKey("dbo.UserProfile", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.UserFeedbacks",
                c => new
                    {
                        FeedbackId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        FeedbackType = c.String(),
                        ProductId = c.Int(nullable: false),
                        RatingID = c.Int(nullable: false),
                        Title = c.String(),
                        Feedback = c.String(),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.FeedbackId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfile", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.UserInfoes",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        FirstName = c.String(nullable: false, maxLength: 100),
                        LastName = c.String(nullable: false, maxLength: 100),
                        StreetAddress = c.String(maxLength: 200),
                        City = c.String(nullable: false, maxLength: 100),
                        StateId = c.Int(nullable: false),
                        ZipCode = c.String(maxLength: 10),
                        CellPhoneNumber = c.String(),
                        HomePhoneNumber = c.String(),
                        EmailAddress = c.String(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreditCardType = c.String(),
                        CreditCardNumber = c.String(maxLength: 20),
                        CreditCardExpiration = c.String(maxLength: 7),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.States", t => t.StateId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfile", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.StateId);
            
            CreateTable(
                "dbo.States",
                c => new
                    {
                        StateID = c.Int(nullable: false, identity: true),
                        StateCode = c.String(nullable: false, maxLength: 3),
                        StateName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.StateID);
            
            CreateTable(
                "dbo.UserNotes",
                c => new
                    {
                        UserNoteId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        CorrespondenceType = c.String(),
                        Title = c.String(),
                        Note = c.String(),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserNoteId)
                .ForeignKey("dbo.UserProfile", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Publishers",
                c => new
                    {
                        PublisherID = c.Int(nullable: false, identity: true),
                        PublisherName = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.PublisherID);
            
            CreateTable(
                "dbo.Ratings",
                c => new
                    {
                        RatingID = c.Int(nullable: false, identity: true),
                        RatingName = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.RatingID);

            // rld. User Membership Tables in Identity Framework
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    Name = c.String(nullable: false, maxLength: 256),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");

            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                {
                    UserId = c.String(nullable: false, maxLength: 128),
                    RoleId = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

            CreateTable(
                "dbo.AspNetUsers",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    Email = c.String(maxLength: 256),
                    EmailConfirmed = c.Boolean(nullable: false),
                    PasswordHash = c.String(),
                    SecurityStamp = c.String(),
                    PhoneNumber = c.String(),
                    PhoneNumberConfirmed = c.Boolean(nullable: false),
                    TwoFactorEnabled = c.Boolean(nullable: false),
                    LockoutEndDateUtc = c.DateTime(),
                    LockoutEnabled = c.Boolean(nullable: false),
                    AccessFailedCount = c.Int(nullable: false),
                    UserName = c.String(nullable: false, maxLength: 256),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");

            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.String(nullable: false, maxLength: 128),
                    ClaimType = c.String(),
                    ClaimValue = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                {
                    LoginProvider = c.String(nullable: false, maxLength: 128),
                    ProviderKey = c.String(nullable: false, maxLength: 128),
                    UserId = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

        }

        //public override void Down()
        //{
        //    DropForeignKey("dbo.Products", "PublisherID", "dbo.Publishers");
        //    DropForeignKey("dbo.OrderItems", "ProductID", "dbo.Products");
        //    DropForeignKey("dbo.UserNotes", "UserId", "dbo.UserProfile");
        //    DropForeignKey("dbo.UserInfoes", "UserId", "dbo.UserProfile");
        //    DropForeignKey("dbo.UserInfoes", "StateId", "dbo.States");
        //    DropForeignKey("dbo.UserFeedbacks", "UserId", "dbo.UserProfile");
        //    DropForeignKey("dbo.UserFeedbacks", "ProductId", "dbo.Products");
        //    DropForeignKey("dbo.Orders", "UserId", "dbo.UserProfile");
        //    DropForeignKey("dbo.OrderItems", "OrderID", "dbo.Orders");
        //    DropForeignKey("dbo.Products", "MediumID", "dbo.Media");
        //    DropForeignKey("dbo.Listings", "ProductID", "dbo.Products");
        //    DropForeignKey("dbo.Listings", "ListTypeID", "dbo.ListTypes");
        //    DropForeignKey("dbo.Products", "CategoryID", "dbo.Categories");
        //    DropIndex("dbo.UserNotes", new[] { "UserId" });
        //    DropIndex("dbo.UserInfoes", new[] { "StateId" });
        //    DropIndex("dbo.UserInfoes", new[] { "UserId" });
        //    DropIndex("dbo.UserFeedbacks", new[] { "ProductId" });
        //    DropIndex("dbo.UserFeedbacks", new[] { "UserId" });
        //    DropIndex("dbo.Orders", new[] { "UserId" });
        //    DropIndex("dbo.OrderItems", new[] { "ProductID" });
        //    DropIndex("dbo.OrderItems", new[] { "OrderID" });
        //    DropIndex("dbo.Listings", new[] { "ProductID" });
        //    DropIndex("dbo.Listings", new[] { "ListTypeID" });
        //    DropIndex("dbo.Products", new[] { "PublisherID" });
        //    DropIndex("dbo.Products", new[] { "MediumID" });
        //    DropIndex("dbo.Products", new[] { "CategoryID" });
        //    DropTable("dbo.Ratings");
        //    DropTable("dbo.Publishers");
        //    DropTable("dbo.UserNotes");
        //    DropTable("dbo.States");
        //    DropTable("dbo.UserInfoes");
        //    DropTable("dbo.UserFeedbacks");
        //    DropTable("dbo.UserProfile");
        //    DropTable("dbo.Orders");
        //    DropTable("dbo.OrderItems");
        //    DropTable("dbo.Media");
        //    DropTable("dbo.ListTypes");
        //    DropTable("dbo.Listings");
        //    DropTable("dbo.Products");
        //    DropTable("dbo.Categories");

            //DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            //DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            //DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            //DropIndex("dbo.AspNetUsers", "UserNameIndex");
            //DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            //DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            //DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            //DropTable("dbo.AspNetUserLogins");
            //DropTable("dbo.AspNetUserClaims");
            //DropTable("dbo.AspNetUsers");
            //DropTable("dbo.AspNetUserRoles");
            //DropTable("dbo.AspNetRoles");
        //}
    }
}
