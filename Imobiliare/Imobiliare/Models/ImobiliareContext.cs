using Microsoft.EntityFrameworkCore;
using Imobiliare.Models;
using System.Reflection.Emit;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Imobiliare.Data
{
    public class ImobiliareContext : IdentityDbContext<Utilizator, IdentityRole<int>, int>
    {
        public ImobiliareContext(DbContextOptions<ImobiliareContext> options)
            : base(options)
        {
        }

        public DbSet<Utilizator>? Utilizatori { get; set; }
        public DbSet<Anunturi>? Anunturi { get; set; }
        public DbSet<Conversatie>? Conversatii { get; set; }
        public DbSet<Mesaje>? Mesaje { get; set; }
        public DbSet<Favorite>? Favorite { get; set; }
        public DbSet<Formular>? Formulare { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Formular>()
                .HasOne(f => f.Utilizator)
                .WithMany(u => u.Formular)
                .HasForeignKey(f => f.IdUtilizator)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Anunturi>()
                .HasOne(a => a.Utilizator)
                .WithMany(u => u.Anunturi)
                .HasForeignKey(a => a.ID_Utilizator)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Conversatie>()
                .HasOne(c => c.Anunturi)
                .WithMany()
                .HasForeignKey(c => c.ID_Anunt)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Conversatie>()
                .HasOne(c => c.Utilizator_proprietar)
                .WithMany()
                .HasForeignKey(c => c.ID_Utilizator_proprietar)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversatie>()
                .HasOne(c => c.Utilizator_client)
                .WithMany()
                .HasForeignKey(c => c.ID_Utilizator_client)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mesaje>()
                .HasOne(m => m.Utilizator_expeditor)
                .WithMany(u => u.Mesaje)
                .HasForeignKey(m => m.ID_Utilizator_expeditor)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Utilizator)
                .WithMany(u => u.Favorite)
                .HasForeignKey(f => f.ID_Utilizator)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Anunturi)
                .WithMany()
                .HasForeignKey(f => f.ID_Anunt)
                .OnDelete(DeleteBehavior.Cascade);


        }


    }
}