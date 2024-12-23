using TestApp.Model;

namespace UnitTests
{
    public static class TestData
    {
        public static List<User> GetTestUsers()
        {
            return [
            new() { Id = 1, Surname = "Иванов", Name = "Иван", Midname = "Иванович", Login = "user9", Password = "$2a$06$/M1zo1RZpLGu/IGaj8Y2EO/JidOLhFE7wwkpMCGM7lr6qcxOmbWqq", Birthdate = DateOnly.Parse("1990-01-01"), Usertype = 1, UsertypeNavigation = new Usertype { Id = 1, Name = "Пользователь" } },
            new() { Id = 2, Surname = "Петров", Name = "Петр", Midname = "Петрович", Login = "worker", Password = "$2a$06$/M1zo1RZpLGu/IGaj8Y2EO/JidOLhFE7wwkpMCGM7lr6qcxOmbWqq", Birthdate = DateOnly.Parse("1991-02-02"), Usertype = 2, UsertypeNavigation = new Usertype { Id = 2, Name = "Работник" } },
            new() { Id = 3, Surname = "Сидоров", Name = "Сидор", Midname = "Сидорович", Login = "admin", Password = "$2a$06$/M1zo1RZpLGu/IGaj8Y2EO/JidOLhFE7wwkpMCGM7lr6qcxOmbWqq", Birthdate = DateOnly.Parse("1992-03-03"), Usertype = 3, UsertypeNavigation = new Usertype { Id = 3, Name = "Администратор" } }];
        }
    }
}
