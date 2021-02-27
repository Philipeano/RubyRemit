using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using RubyRemit.Domain.Entities;

namespace RubyRemit.Infrastructure
{
    public class RubyRemitContext : DbContext
    {
        private readonly IConfiguration configuration;

        public RubyRemitContext(DbContextOptions<RubyRemitContext> options, IConfiguration config)
            : base(options)
        {
            configuration = config;
        }


        public virtual DbSet<Payment> Payments { get; set; }

        public virtual DbSet<PaymentState> PaymentStates { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnStr"));
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("PaymentId");

                entity.Property(e => e.CreditCardNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.CardHolder)
                    .IsRequired()
                    .HasMaxLength(25);

                entity.Property(e => e.ExpirationDate)
                    .IsRequired();

                entity.Property(e => e.SecurityCode)
                    .IsRequired(false)
                    .HasMaxLength(3);

                entity.Property(e => e.Amount)
                    .IsRequired()
                    .HasColumnType("decimal(18, 2)");
            });


            modelBuilder.Entity<PaymentState>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("PaymentStateId");

                entity.Property(e => e.PaymentId)
                    .IsRequired();

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasColumnType("tinyint");

                entity.Property(e => e.DateAttempted)
                    .IsRequired()
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Gateway)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Remark)
                    .IsRequired(false)
                    .HasMaxLength(200);

                // A PaymentState has only one Payment, while a Payment has one or more PaymentStates (ProcessingAttempts)
                entity.HasOne(attempt => attempt.Payment)
                    .WithMany(payment => payment.ProcessingAttempts)
                    .HasForeignKey(attempt => attempt.PaymentId)
                    .IsRequired(true)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}





