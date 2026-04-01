using Microsoft.EntityFrameworkCore;
using TaskManager.Models;

namespace Task_Management.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Adaugă aici tabelele tale
        public DbSet<CalendarNote> CalendarNotes { get; set; }

        // Dacă ai alte modele, adaugă-le aici
        // public DbSet<Task> Tasks { get; set; }
        // public DbSet<User> Users { get; set; }
    }
}