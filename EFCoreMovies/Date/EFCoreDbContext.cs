using EFCoreMovies.Entities;
using EFCoreMovies.Entities.Seeding;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System.CodeDom.Compiler;
using System.Reflection;

namespace EFCoreMovies.Date
{
    public class EFCoreDbContext : DbContext
    {
        public EFCoreDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Actor>().Property(p => p.Name).HasMaxLength(150).IsRequired();
            modelBuilder.Entity<Actor>().Property(p => p.DateOfBirth).HasColumnType("Date");
            //modelBuilder.Entity<Actor>().Property(p => p.Name).HasField("_name");

            modelBuilder.Entity<Cinema>().Property(p => p.Name).HasMaxLength(150).IsRequired();

            modelBuilder.Entity<CinemaHall>().Property(p => p.Cost).HasPrecision(precision: 9, scale: 2);
            modelBuilder.Entity<CinemaHall>().Property(p => p.CinemaHallType).HasDefaultValue(CinemaHallType.TwoDimensions);

            modelBuilder.Entity<CinemaOffer>().Property(p => p.DiscountPercentage).HasPrecision(5, 2);
            modelBuilder.Entity<CinemaOffer>().Property(p => p.Begin).HasColumnType("Date");
            modelBuilder.Entity<CinemaOffer>().Property(p => p.End).HasColumnType("Date");

            modelBuilder.Entity<Genre>().Property(p => p.Name).HasMaxLength(150).IsRequired();
            modelBuilder.Entity<Genre>().HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<MovieActor>().HasKey(p => new { p.MovieId, p.ActorId });
            modelBuilder.Entity<MovieActor>().Property(p => p.Character).HasMaxLength(150);

            modelBuilder.Entity<Movie>().Property(p => p.Title).HasMaxLength(250).IsRequired();
            modelBuilder.Entity<Movie>().Property(p => p.ReleaseDate).HasColumnType("Date");
            modelBuilder.Entity<Movie>().Property(p => p.PosterURL).HasMaxLength(500).IsUnicode(false).IsRequired(false);

            ClassSeeding.Seed(modelBuilder);
        }

        public DbSet<Genre> Genres { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<CinemaOffer> CinemaOffers { get; set; }
        public DbSet<CinemaHall> CinemaHalls { get; set; }
        public DbSet<MovieActor> MovieActors { get; set; }
    }
}
