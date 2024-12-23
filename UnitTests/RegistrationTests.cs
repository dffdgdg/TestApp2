using Microsoft.EntityFrameworkCore;
using Moq;
using TestApp.Model;
using TestApp.Services;
using TestApp.View.Authentication;
using TestApp.ViewModel;

namespace UnitTests
{
    public class RegistrationVMTests
    {
        private readonly Mock<INavigationService> _navigationServiceMock;
        private readonly Mock<TestDbContext> _dbContextMock;
        private readonly RegistrationVM _registrationVM;

        public RegistrationVMTests()
        {
            _navigationServiceMock = new Mock<INavigationService>();
            _dbContextMock = new Mock<TestDbContext>();
            var users = TestData.GetTestUsers().AsQueryable();
            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());
            _dbContextMock.Setup(dc => dc.Users).Returns(mockSet.Object);
            _registrationVM = new RegistrationVM(_navigationServiceMock.Object);
        }

        [Fact]
        public void OnRegistration_EmptyLastName_ShowsPopup()
        {
            _registrationVM.LastName = "";
            _registrationVM.FirstName = "John";
            _registrationVM.MiddleName = "Doe";
            _registrationVM.Login = "johndoe";
            _registrationVM.Password = "123";
            _registrationVM.OnRegistration();
            _navigationServiceMock.Verify(n => n.Navigate(typeof(LoginPage)), Times.Never);
        }

        [Fact]
        public void OnRegistration_UserAlreadyExists_ShowsPopup()
        {
            _registrationVM.LastName = "Doe";
            _registrationVM.FirstName = "John";
            _registrationVM.MiddleName = "Smith";
            _registrationVM.Login = "user9";
            _registrationVM.Password = "123";
            _registrationVM.OnRegistration();
            _navigationServiceMock.Verify(n => n.Navigate(typeof(LoginPage)), Times.Never);
        }

        [Fact]
        public void OnRegistration_ValidData_NavigatesToLoginPage()
        {
            _registrationVM.LastName = "Сергеев";
            _registrationVM.FirstName = "Сергей";
            _registrationVM.MiddleName = "Олегович";
            _registrationVM.Login = "serg431";
            _registrationVM.Password = "123";
            _registrationVM.OnRegistration();
            // Проверка, что пользователь был добавлен и навигация на страницу входа была выполнена
            _navigationServiceMock.Verify(n => n.Navigate(typeof(LoginPage)), Times.Once);
        }
    }
}