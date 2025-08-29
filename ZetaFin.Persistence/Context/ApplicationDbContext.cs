using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZetaFin.Domain.Entities;

namespace ZetaFin.Persistence.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Goal> Goals { get; set; }
    public DbSet<Deposit> Deposits { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserGoal> UserGoals { get; set; }

}
