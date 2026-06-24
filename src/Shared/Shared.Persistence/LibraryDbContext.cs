using Microsoft.EntityFrameworkCore;
using Shared.Persistence.Entities;

namespace Shared.Persistence;

public sealed class LibraryDbContext(DbContextOptions<LibraryDbContext> options) : DbContext(options)
{
    public DbSet<AuthorRecord> Authors => Set<AuthorRecord>();
    public DbSet<BookRecord> Books => Set<BookRecord>();
    public DbSet<MemberRecord> Members => Set<MemberRecord>();
    public DbSet<LoanRecord> Loans => Set<LoanRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthorRecord>(entity =>
        {
            entity.ToTable("Authors");
            entity.HasKey(author => author.Id);
            entity.Property(author => author.Id).HasMaxLength(50);
            entity.Property(author => author.Name).HasMaxLength(200).IsRequired();
            entity.Property(author => author.Bio);
        });

        modelBuilder.Entity<BookRecord>(entity =>
        {
            entity.ToTable("Books");
            entity.HasKey(book => book.Id);
            entity.Property(book => book.Id).HasMaxLength(50);
            entity.Property(book => book.Title).HasMaxLength(300).IsRequired();
            entity.Property(book => book.AuthorId).HasMaxLength(50).IsRequired();
            entity.Property(book => book.Isbn).HasMaxLength(20).IsRequired();
            entity.HasOne<AuthorRecord>()
                .WithMany()
                .HasForeignKey(book => book.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(book => book.AuthorId);
        });

        modelBuilder.Entity<MemberRecord>(entity =>
        {
            entity.ToTable("Members");
            entity.HasKey(member => member.Id);
            entity.Property(member => member.Id).HasMaxLength(50);
            entity.Property(member => member.Name).HasMaxLength(200).IsRequired();
            entity.Property(member => member.Email).HasMaxLength(200);
        });

        modelBuilder.Entity<LoanRecord>(entity =>
        {
            entity.ToTable("Loans");
            entity.HasKey(loan => loan.Id);
            entity.Property(loan => loan.Id).HasMaxLength(50);
            entity.Property(loan => loan.BookId).HasMaxLength(50).IsRequired();
            entity.Property(loan => loan.MemberId).HasMaxLength(50).IsRequired();
            entity.HasOne<BookRecord>()
                .WithMany()
                .HasForeignKey(loan => loan.BookId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<MemberRecord>()
                .WithMany()
                .HasForeignKey(loan => loan.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(loan => loan.BookId);
            entity.HasIndex(loan => loan.MemberId);
        });
    }
}
