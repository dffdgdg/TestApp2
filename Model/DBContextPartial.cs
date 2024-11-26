using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace TestApp.Model
{
    partial class TestDbContext : DbContext
    {
        public DbSet<AuthResult> AuthResults { get; set; }
        public DbSet<BoolResult> BoolResults { get; set; }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthResult>(entity =>
            {
                entity.HasNoKey();
                entity.ToTable("AuthResult", t => t.ExcludeFromMigrations());
            });

            modelBuilder.Entity<BoolResult>(entity =>
            {
                entity.HasNoKey();
                entity.ToTable("BoolResult", t => t.ExcludeFromMigrations());
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

        public bool CheckPassword(string login, string password)
        {
            var loginParameter = new NpgsqlParameter("p_login", login);
            var passwordParameter = new NpgsqlParameter("p_password", password);

            var result = this.Set<BoolResult>().FromSqlRaw(
                "SELECT check_password(@p_login, @p_password) AS result",
                loginParameter, passwordParameter).AsEnumerable().FirstOrDefault();

            return result != null && result.result;
        }

        public bool ChangePassword(string login, string oldPassword, string newPassword)
        {
            var loginParameter = new NpgsqlParameter("p_login", login);
            var oldPasswordParameter = new NpgsqlParameter("p_old_password", oldPassword);
            var newPasswordParameter = new NpgsqlParameter("p_new_password", newPassword);

            var result = this.Set<BoolResult>().FromSqlRaw(
                "SELECT change_password(@p_login, @p_old_password, @p_new_password) AS result",
                loginParameter, oldPasswordParameter, newPasswordParameter).AsEnumerable().FirstOrDefault();

            return result != null && result.result;
        }
    }

    public class AuthResult
    {
        public int id { get; set; }
        public int type { get; set; }
    }

    public class BoolResult
    {
        public bool result { get; set; }
    }
}