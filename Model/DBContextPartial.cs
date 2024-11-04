using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace TestApp.Model
{
    partial class TestDbContext : DbContext
    {
        public DbSet<AuthResult> AuthResults { get; set; }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthResult>(entity =>
            {
                entity.HasNoKey();
                entity.ToTable("AuthResult", t => t.ExcludeFromMigrations());
            });
        }

        public (int userId, int userType) AuthenticateUser(string login, string password)
        {
            var loginParameter = new NpgsqlParameter("p_login", login);
            var passwordParameter = new NpgsqlParameter("p_password", password);

            var result = this.Set<AuthResult>().FromSqlRaw(
                "SELECT user_id as id, user_type as type FROM authenticate_user(@p_login, @p_password)",
                loginParameter, passwordParameter).AsEnumerable().FirstOrDefault();

            return result != null ? (result.id, result.type) : (0, 0);
        }
    }

    public class AuthResult
    {
        public int id { get; set; }
        public int type { get; set; }
    }
}