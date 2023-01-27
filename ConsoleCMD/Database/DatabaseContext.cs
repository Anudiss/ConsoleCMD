using ConsoleCMD.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using SysIO = System.IO;

namespace ConsoleCMD.Database;

public partial class DatabaseContext : DbContext
{
    public static DatabaseContext Entities { get; set; }

    public static void InitializeEntities()
        => Entities = new();

    public static void LoadEntities()
    {
        DatabaseContext.Entities.Icons.Load();
        DatabaseContext.Entities.Extensions.Load();
        DatabaseContext.Entities.Directories.Load();
        DatabaseContext.Entities.Files.Load();
    }

    public DatabaseContext() { }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options) { }

    public virtual DbSet<Directory> Directories { get; set; }

    public virtual DbSet<Extension> Extensions { get; set; }

    public virtual DbSet<File> Files { get; set; }

    public virtual DbSet<Icon> Icons { get; set; }

    // TODO: ...
    private static readonly string _workingDirectory = SysIO.Directory.GetCurrentDirectory();
    private static readonly string _projectDirectory = SysIO.Directory.GetParent(_workingDirectory).Parent.Parent.FullName;
    private static readonly string _dbDirectory = "Database";
    private static readonly string _dbFileName = "filesystem.db";
    private static readonly string _dbFilePath = SysIO.Path.Combine(SysIO.Path.Combine(_projectDirectory, _dbDirectory), _dbFileName);

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={_dbFilePath}");

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
                .HasConstraintName("FK_Directory_Directory")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Extension>(entity =>
        {
            entity.ToTable("Extension");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.Icon).WithMany(p => p.Extensions)
                .HasForeignKey(d => d.IconId)
                .HasConstraintName("FK_Extension_Icon")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<File>(entity =>
        {
            entity.ToTable("File");

            entity.Property(e => e.Name)
                .HasMaxLength(100);

            entity.HasOne(d => d.Extension).WithMany(p => p.Files)
                .HasForeignKey(d => d.ExtensionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_File_Extension");

            entity.HasOne(d => d.Parent).WithMany(p => p.Files)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_File_Directory")
                .OnDelete(DeleteBehavior.Cascade);
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
