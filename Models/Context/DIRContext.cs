using Domain.Models.Config;
using Microsoft.EntityFrameworkCore;
#nullable disable

namespace Models.Models.Context
{
    public class DIRContext : DbContext
    {
        private readonly WebServiceSetting _webServiceSetting;

        public DIRContext(DbContextOptions<DIRContext> options, WebServiceSetting webServiceSetting)
            : base(options)
        {
            _webServiceSetting = webServiceSetting;
        }

        public DbSet<DirConfig> DirConfigs { get; set; }
        public DbSet<Protected> Protecteds { get; set; }
        public DbSet<ErrorSendTo> ErrorSendTos { get; set; }
        public DbSet<MailSetting> MailSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure one-to-many relationship between Protected and DirConfig
            modelBuilder.Entity<Protected>()
                .HasMany(p => p.DirConfigs)
                .WithOne(d => d.Protected)
                .HasForeignKey(d => d.ProtectedId);

            // Configure one-to-many relationship between Protected and ErrorSendTo
            modelBuilder.Entity<Protected>()
                .HasMany(p => p.ErrorSendTos)
                .WithOne(e => e.Protected)
                .HasForeignKey(e => e.ProtectedId);

            // Additional configurations can be set here
        }
    }

    #region DIRContext old version 舊版
    /*
    public partial class DIRContext : DbContext
    {
        private readonly WebServiceSetting _webServiceSetting;

        public DIRContext(DbContextOptions<DIRContext> options
                           , WebServiceSetting webServiceSetting)
            : base(options)
        {
            _webServiceSetting = webServiceSetting;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(_webServiceSetting.RelationDB.DIR_ConnectionString);
            }
        }

        //public virtual DbSet<AuthorizationAccount> AuthorizationAccounts { get; set; }
        //public virtual DbSet<AuthorizationCategory> AuthorizationCategories { get; set; }
        //public virtual DbSet<AuthorizationRole> AuthorizationRoles { get; set; }
        //public virtual DbSet<AuthorizationRoleCategory> AuthorizationRoleCategories { get; set; }        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");

            //modelBuilder.Entity<AuthorizationAccount>(entity =>
            //{
            //    entity.ToTable("authorization_account");

            //    entity.HasIndex(e => e.RoleId, "role_id");

            //    entity.Property(e => e.Id).HasColumnName("id");

            //    entity.Property(e => e.Account)
            //        .IsRequired()
            //        .HasMaxLength(20)
            //        .HasColumnName("account")
            //        .HasComment("帳號");

            //    entity.Property(e => e.CreatedAt)
            //        .HasColumnType("datetime")
            //        .HasColumnName("created_at")
            //        .HasDefaultValueSql("now()")
            //        .HasComment("建立時間");

            //    entity.Property(e => e.CreatedId)
            //        .HasColumnName("created_id")
            //        .HasComment("建立者 id");

            //    entity.Property(e => e.Deleted)
            //        .HasColumnType("bit(1)")
            //        .HasColumnName("deleted")
            //        .HasDefaultValueSql("b'0'")
            //        .HasComment("刪除狀態");

            //    entity.Property(e => e.Email)
            //        .IsRequired()
            //        .HasMaxLength(50)
            //        .HasColumnName("email")
            //        .HasComment("信箱");

            //    entity.Property(e => e.Name)
            //        .IsRequired()
            //        .HasMaxLength(30)
            //        .HasColumnName("name")
            //        .HasComment("姓名");

            //    entity.Property(e => e.Password)
            //        .IsRequired()
            //        .HasMaxLength(50)
            //        .HasColumnName("password")
            //        .HasComment("密碼");

            //    entity.Property(e => e.RoleId)
            //        .HasColumnName("role_id")
            //        .HasComment("指派腳色");

            //    entity.Property(e => e.UpdatedAt)
            //        .HasColumnType("datetime")
            //        .ValueGeneratedOnAddOrUpdate()
            //        .HasColumnName("updated_at")
            //        .HasComment("更新時間");

            //    entity.Property(e => e.UpdatedId)
            //        .HasColumnName("updated_id")
            //        .HasComment("更新者 id");

            //    entity.HasOne(d => d.Role)
            //        .WithMany(p => p.AuthorizationAccounts)
            //        .HasForeignKey(d => d.RoleId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("authorization_account_ibfk_1");
            //});

            //modelBuilder.Entity<AuthorizationCategory>(entity =>
            //{
            //    entity.ToTable("authorization_category");

            //    entity.Property(e => e.Id).HasColumnName("id");

            //    entity.Property(e => e.CreatedAt)
            //        .HasColumnType("datetime")
            //        .HasColumnName("created_at")
            //        .HasDefaultValueSql("now()")
            //        .HasComment("建立時間");

            //    entity.Property(e => e.Name)
            //        .IsRequired()
            //        .HasMaxLength(20)
            //        .HasColumnName("name")
            //        .HasComment("功能名稱");

            //    entity.Property(e => e.ParentId)
            //        .HasColumnName("parent_id")
            //        .HasComment("父id");

            //    entity.Property(e => e.UpdatedAt)
            //        .HasColumnType("datetime")
            //        .ValueGeneratedOnAddOrUpdate()
            //        .HasColumnName("updated_at")
            //        .HasComment("更新時間");
            //});

            //modelBuilder.Entity<AuthorizationRole>(entity =>
            //{
            //    entity.ToTable("authorization_role");

            //    entity.Property(e => e.Id).HasColumnName("id");

            //    entity.Property(e => e.CreatedAt)
            //        .HasColumnType("datetime")
            //        .HasColumnName("created_at")
            //        .HasDefaultValueSql("now()")
            //        .HasComment("建立時間");

            //    entity.Property(e => e.CreatedId)
            //        .HasColumnName("created_id")
            //        .HasComment("建立者 id");

            //    entity.Property(e => e.Deleted)
            //        .HasColumnType("bit(1)")
            //        .HasColumnName("deleted")
            //        .HasComment("是否被刪除?");

            //    entity.Property(e => e.Name)
            //        .IsRequired()
            //        .HasMaxLength(50)
            //        .HasColumnName("name")
            //        .HasComment("角色名稱");

            //    entity.Property(e => e.UpdatedAt)
            //        .HasColumnType("datetime")
            //        .ValueGeneratedOnAddOrUpdate()
            //        .HasColumnName("updated_at")
            //        .HasComment("更新時間");

            //    entity.Property(e => e.UpdatedId)
            //        .HasColumnName("updated_id")
            //        .HasComment("更新者 id");
            //});

            //modelBuilder.Entity<AuthorizationRoleCategory>(entity =>
            //{
            //    entity.ToTable("authorization_role_category");

            //    entity.HasIndex(e => e.CategoryId, "category_id");

            //    entity.HasIndex(e => e.RoleId, "role_id");

            //    entity.Property(e => e.Id).HasColumnName("id");

            //    entity.Property(e => e.CategoryId)
            //        .HasColumnName("category_id")
            //        .HasComment("功能 id");

            //    entity.Property(e => e.CreatedAt)
            //        .HasColumnType("datetime")
            //        .HasColumnName("created_at")
            //        .HasDefaultValueSql("now()")
            //        .HasComment("建立時間");

            //    entity.Property(e => e.Readonly)
            //        .HasColumnName("readonly")
            //        .HasComment("是否唯讀");

            //    entity.Property(e => e.RoleId)
            //        .HasColumnName("role_id")
            //        .HasComment("角色 id");

            //    entity.Property(e => e.UpdatedAt)
            //        .HasColumnType("datetime")
            //        .ValueGeneratedOnAddOrUpdate()
            //        .HasColumnName("updated_at")
            //        .HasComment("更新時間");

            //    entity.HasOne(d => d.Category)
            //        .WithMany(p => p.AuthorizationRoleCategories)
            //        .HasForeignKey(d => d.CategoryId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("authorization_role_category_ibfk_2");

            //    entity.HasOne(d => d.Role)
            //        .WithMany(p => p.AuthorizationRoleCategories)
            //        .HasForeignKey(d => d.RoleId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("authorization_role_category_ibfk_1");
            //});           

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
    */
    #endregion 
}
