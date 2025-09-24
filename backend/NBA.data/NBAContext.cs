using Microsoft.EntityFrameworkCore;

namespace NBA.data;

public class NBAContext : DbContext
{
    public NBAContext(DbContextOptions<NBAContext> options) : base(options) { }
}
