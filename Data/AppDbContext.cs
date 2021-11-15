using MeuTodo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeuTodo.Data
{
    public class AppDbContext: DbContext
    {
        public DbSet<ToDo> Todos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite("DataSource=app.db;Cache=Shared");
    }
}
