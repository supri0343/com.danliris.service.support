using com.danliris.support.lib.Models;
using Com.Moonlay.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace com.danliris.support.lib
{
    public class SupportDbContext : BaseDbContext
    {
        public SupportDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<FactBeacukai> FactBeacukai { get; set; }
        public DbSet<BEACUKAI_TEMP> BEACUKAI_TEMP { get; set; }
    }
}
