using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OBPostupyApi.Entities;
using System;
using System.Linq;

namespace OBPostupyApi.Contexts
{
    public class RepositoryContext : IdentityDbContext<User>
    {
        public RepositoryContext()
        {
        }
        public RepositoryContext(DbContextOptions<RepositoryContext> options)
            : base(options)
        {
        }

        public DbSet<Race> Races { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Control> Controls { get; set; }
        public DbSet<CourseControl> CourseControls { get; set; }
        public DbSet<Map> Maps { get; set; }
        public DbSet<Split> Splits { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<PersonResult> PersonResults { get; set; }
        public DbSet<SplitTime> SplitTimes { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<Path> Paths { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<CourseSplit> CourseSplits { get; set; }
        public DbSet<CourseData> CourseData { get; set; }
        public DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));

            modelBuilder.Entity<Split>()
                .HasOne(s => s.FirstControl)
                .WithMany(c => c.SplitsFirstControl)
                .HasForeignKey(s => s.FirstControlId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Split>()
                .HasOne(s => s.SecondControl)
                .WithMany(c => c.SplitsSecondControl)
                .HasForeignKey(s => s.SecondControlId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SplitTime>()
                .HasOne(st => st.Split)
                .WithMany(s => s.SplitTimes)
                .HasForeignKey(st => st.SplitId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.Course)
                .WithMany(c => c.Categories)
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<CourseControl>()
                .HasKey(cc => new { cc.CourseId, cc.ControlId, cc.Order });

            modelBuilder.Entity<CourseSplit>()
                .HasKey(cs => new { cs.CourseId, cs.SplitId });

            modelBuilder.Entity<Race>()
                .HasIndex(r => r.Key)
                .IsUnique();

            modelBuilder.Entity<Person>()
                .Property(p => p.RegNumbers)
                .HasConversion(
                    c => string.Join(';', c),
                    c => c.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
                    );

            modelBuilder.Entity<Path>()
                .HasOne(p => p.PersonResult)
                .WithOne(p => p.Path)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
