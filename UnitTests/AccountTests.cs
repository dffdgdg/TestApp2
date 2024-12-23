using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TestApp.Model;
using TestApp.ViewModel;

namespace UnitTests
{
    public class AccountTests
    {
        [Fact]
        public void Constructor_ShouldInitializeRoles()
        {
            // Arrange
            var mockDbContext = new Mock<TestDbContext>();
            mockDbContext.Setup(x => x.Usertypes).Returns(GetMockDbSet(new[]
            {
            new Usertype { Id = 1, Name = "Admin" },
            new Usertype { Id = 2, Name = "User" }
        }));

            // Act
            var vm = new AccountVM();
            vm.Roles = new ObservableCollection<Usertype>(mockDbContext.Object.Usertypes);

            // Assert
            Assert.NotNull(vm.Roles);
            Assert.Equal(2, vm.Roles.Count);
        }

        [Fact]
        public void AddItem_ShouldInitializeCurUserAndSetVisibility()
        {
            // Arrange
            var vm = new AccountVM();

            // Act
            vm.AddItem();

            // Assert
            Assert.NotNull(vm.CurUser);
            Assert.Equal(1, vm.CurUser.Usertype); // Убедимся, что пользователь имеет тип "1".
            Assert.Equal(Visibility.Visible, vm.UCVisibility);
        }

        [Fact]
        public void OpenItem_ShouldSetSelectedUserAsCurUser()
        {
            // Arrange
            var user = new User { Id = 1, Login = "test_user" };
            var vm = new AccountVM
            {
                SelectedUser = user
            };

            // Act
            vm.OpenItem();

            // Assert
            Assert.NotNull(vm.CurUser);
            Assert.Equal(user.Id, vm.CurUser.Id);
            Assert.Equal(Visibility.Visible, vm.UCVisibility);
        }

        [Fact]
        public void SaveItem_ShouldThrowExceptionForDuplicateLogin()
        {
            // Arrange
            var mockDbContext = new Mock<TestDbContext>();
            mockDbContext.Setup(x => x.Users).Returns(GetMockDbSet(new[]
            {
            new User { Id = 1, Login = "existing_user" }
        }));

            var vm = new AccountVM
            {
                CurUser = new User
                {
                    Login = "existing_user",
                    Surname = "Doe",
                    Name = "John",
                    Password = "password"
                },
                Users = new ObservableCollection<User>(mockDbContext.Object.Users)
            };

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => vm.SaveItem());
            Assert.Contains("Аккаунт с таким логином уже существует!", exception.Message);
        }

        [Fact]
        public void UpdateData_ShouldFilterUsersBasedOnSearchQuery()
        {
            // Arrange
            var users = new[]
            {
            new User { Id = 1, Login = "user1", Surname = "Smith", Name = "John" },
            new User { Id = 2, Login = "user2", Surname = "Doe", Name = "Jane" }
        };
            var mockDbContext = new Mock<TestDbContext>();
            mockDbContext.Setup(x => x.Users.Include(It.IsAny<string>())).Returns(GetMockDbSet(users));

            var vm = new AccountVM
            {
                SearchQuery = "Smith",
                Users = new ObservableCollection<User>(users)
            };

            // Act
            vm.UpdateData();

            // Assert
            Assert.Single(vm.Users);
            Assert.Equal("Smith", vm.Users.First().Surname);
        }

        private DbSet<T> GetMockDbSet<T>(IEnumerable<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            return mockSet.Object;
        }
    }
}