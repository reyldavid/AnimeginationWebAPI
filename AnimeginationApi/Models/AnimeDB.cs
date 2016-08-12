namespace AnimeginationApi.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AnimeDB : DbContext
    {
        public AnimeDB()
            : base("name=AnimeDB")
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Listing> Listings { get; set; }
        public virtual DbSet<ListType> ListTypes { get; set; }
        public virtual DbSet<Medium> Media { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Publisher> Publishers { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<UserFeedback> UserFeedbacks { get; set; }
        public virtual DbSet<UserInfo> UserInfoes { get; set; }
        public virtual DbSet<UserNote> UserNotes { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserProfile>()
                .HasOptional(e => e.UserInfo)
                .WithRequired(e => e.UserProfile);
        }
    }
}
