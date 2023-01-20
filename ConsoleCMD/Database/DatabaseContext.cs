using ConsoleCMD.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsoleCMD.Database;

public partial class DatabaseContext : DbContext
{
    public static DatabaseContext Entities { get; } = new();

    //static DatabaseContext()
    //{
    //    var paths = new string[]
    //    {
    //        "C:\\Users\\Ильназ\\Downloads\\Icons\\letter-d.png",
    //        "C:\\Users\\Ильназ\\Downloads\\Icons\\letter-f.png",
    //        "C:\\Users\\Ильназ\\Downloads\\Icons\\letter-a.png"
    //    };

    //    foreach (var path in paths)
    //    {
    //        Entities.Icons.Add(
    //            new Icon
    //            {
    //                Data = System.IO.File.ReadAllBytes(path)
    //            }    
    //        );
    //    }
    //    Entities.SaveChanges();
    //}

    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Directory> Directories { get; set; }

    public virtual DbSet<Extension> Extensions { get; set; }

    public virtual DbSet<File> Files { get; set; }

    public virtual DbSet<Icon> Icons { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite($"Data Source=Database/filesystem.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Directory>(entity =>
        {
            entity.ToTable("Directory");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(d => d.Icon).WithMany(p => p.Directories)
                .HasForeignKey(d => d.IconId)
                .HasConstraintName("FK_Directory_Icon");

            entity.HasOne(d => d.Parent).WithMany(p => p.SubDirectories)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_Directory_Directory");
        });

        modelBuilder.Entity<Extension>(entity =>
        {
            entity.ToTable("Extension");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.Icon).WithMany(p => p.Extensions)
                .HasForeignKey(d => d.IconId)
                .HasConstraintName("FK_Extension_Icon");
        });

        modelBuilder.Entity<File>(entity =>
        {
            entity.ToTable("File");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(d => d.Extension).WithMany(p => p.Files)
                .HasForeignKey(d => d.ExtensionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_File_Extension");

            entity.HasOne(d => d.Parent).WithMany(p => p.Files)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_File_Directory");
        });

        modelBuilder.Entity<Icon>(entity =>
        {
            entity.ToTable("Icon");

            entity.Property(e => e.Data).IsRequired();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
