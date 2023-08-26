using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ChatBotAPI.Models
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<TaskItem> Tasks { get; set; }
        public ApplicationDbContext(DbContextOptions opts): base(opts) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

    }
}
