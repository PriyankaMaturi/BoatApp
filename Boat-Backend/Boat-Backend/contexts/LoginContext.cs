﻿using Microsoft.EntityFrameworkCore;
using Boat_Backend.Models;
namespace Boat_Backend.contexts
{
    public class LoginContext :DbContext
    {
        public DbSet<Login> login {  get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // "FoodCS": "server= DESKTOP-IFVHQAF;database=FoodDb; Integrated Security=True ; MultipleActiveResultSets=true; TrustServerCertificate=true"
            optionsBuilder.UseSqlServer("server= DESKTOP-IFVHQAF;database=BoatDb; Integrated Security=True ; MultipleActiveResultSets=true; TrustServerCertificate=true");
        }
    }
}