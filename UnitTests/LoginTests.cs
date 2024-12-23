using Microsoft.EntityFrameworkCore;
using Moq;
using TestApp.Model;
using TestApp.Services;
using TestApp.View;
using TestApp.View.Authentication;
using TestApp.ViewModel;

namespace UnitTests
{
    public class LoginTests
    {
        private readonly Mock<INavigationService> _navigationServiceMock;
        private readonly Mock<TestDbContext> _dbContextMock;
        private LoginVM _viewModel;
        public LoginTests()
        {
            _navigationServiceMock = new Mock<INavigationService>();
            _dbContextMock = new Mock<TestDbContext>(); // Создаем мок для TestDbContext

            // Создаем список пользователей для имитации базы данных
            var users = TestData.GetTestUsers().AsQueryable();
            // Настраиваем мок для DbSet<User>
            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());
            // Настраиваем контекст базы данных
            _dbContextMock.Setup(dc => dc.Users).Returns(mockSet.Object);
            // Передаем мок контекста базы данных в RegistrationVM
            _viewModel = new LoginVM(_navigationServiceMock.Object);
        }

        private void SetupViewModel()
        {
            _viewModel = new LoginVM(_navigationServiceMock.Object)
            {
                Username = "user9",
                Password = "123",
                SavePassword = false
            };
        }

        [Fact]
        public void OnTestFailLogin()
        {
            SetupViewModel();
            _viewModel.OnLoginAsync(false);
            _navigationServiceMock.Verify(nav => nav.Navigate(It.Is<Type>(t => t == typeof(MainPage)), It.IsAny<object>()), Times.Never);
        }
        [Fact]
        public void OnRegister_ShouldNavigateToRegistrationPage()
        {
            SetupViewModel();
            _viewModel.Register.Execute(null);
            _navigationServiceMock.Verify(nav => nav.Navigate(It.Is<Type>(t => t == typeof(RegistrationPage)),It.IsAny<object>()), Times.Once);
        }
    }
}
