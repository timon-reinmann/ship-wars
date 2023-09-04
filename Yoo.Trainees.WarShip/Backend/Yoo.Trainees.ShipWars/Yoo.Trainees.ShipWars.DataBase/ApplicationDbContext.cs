using Microsoft.EntityFrameworkCore;

namespace Yoo.Trainees.ShipWars.DataBase
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
    }
}
