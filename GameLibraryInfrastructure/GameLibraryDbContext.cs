using System;
using System.Collections.Generic;
using GameLibraryDomain.Model;
using Microsoft.EntityFrameworkCore;

//namespace GameLibraryDomain.Model;
namespace GameLibraryInfrastructure;

public partial class GameLibraryDbContext : DbContext
{
    public GameLibraryDbContext()
    {
    }

    public GameLibraryDbContext(DbContextOptions<GameLibraryDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Developer> Developers { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<Gamestatus> Gamestatuses { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Statushistory> Statushistories { get; set; }

    public virtual DbSet<Userlibrary> Userlibraries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=game_library;Username=postgres;Password=1202");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Developer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("developers_pkey");

            entity.ToTable("developers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("games_pkey");

            entity.ToTable("games");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Developerid).HasColumnName("developerid");
            entity.Property(e => e.Genreid).HasColumnName("genreid");
            entity.Property(e => e.Releaseyear).HasColumnName("releaseyear");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Developer).WithMany(p => p.Games)
                .HasForeignKey(d => d.Developerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_games_developer");

            entity.HasOne(d => d.Genre).WithMany(p => p.Games)
                .HasForeignKey(d => d.Genreid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_games_genre");
        });

        modelBuilder.Entity<Gamestatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("gamestatuses_pkey");

            entity.ToTable("gamestatuses");

            entity.HasIndex(e => e.Statusname, "gamestatuses_statusname_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Statusname)
                .HasMaxLength(50)
                .HasColumnName("statusname");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("genres_pkey");

            entity.ToTable("genres");

            entity.HasIndex(e => e.Name, "genres_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Statushistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("statushistories_pkey");

            entity.ToTable("statushistories");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Changedate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("changedate");
            entity.Property(e => e.Newstatusid).HasColumnName("newstatusid");
            entity.Property(e => e.Oldstatusid).HasColumnName("oldstatusid");
            entity.Property(e => e.Userlibraryid).HasColumnName("userlibraryid");

            entity.HasOne(d => d.Newstatus).WithMany(p => p.StatushistoryNewstatuses)
                .HasForeignKey(d => d.Newstatusid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_history_new");

            entity.HasOne(d => d.Oldstatus).WithMany(p => p.StatushistoryOldstatuses)
                .HasForeignKey(d => d.Oldstatusid)
                .HasConstraintName("fk_history_old");

            entity.HasOne(d => d.Userlibrary).WithMany(p => p.Statushistories)
                .HasForeignKey(d => d.Userlibraryid)
                .HasConstraintName("fk_history_library");
        });

        modelBuilder.Entity<Userlibrary>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("userlibraries_pkey");

            entity.ToTable("userlibraries");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Addedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("addedat");
            entity.Property(e => e.Updatedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("Updatedat");
            entity.Property(e => e.Gameid).HasColumnName("gameid");
            entity.Property(e => e.Isfavorite).HasColumnName("isfavorite");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Review).HasColumnName("review");
            entity.Property(e => e.Statusid).HasColumnName("statusid");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Game).WithMany(p => p.Userlibraries)
                .HasForeignKey(d => d.Gameid)
                .HasConstraintName("fk_library_game");

            entity.HasOne(d => d.Status).WithMany(p => p.Userlibraries)
                .HasForeignKey(d => d.Statusid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_library_status");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
