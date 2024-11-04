using System.Windows.Controls;

namespace TestApp.Helpers
{
    public interface INavigationService
    {
        void Navigate(Type pageType);
        void Navigate(Type pageType, object vm);
        void GoBack();
        void GoForward();
    }

    public class NavigationService : INavigationService
    {
        private readonly Frame _frame;

        public NavigationService(Frame frame)
        {
            _frame = frame;
        }

        public void Navigate(Type pageType)
        {
            if (pageType == null)
                throw new ArgumentNullException(nameof(pageType));
            try
            {
                _frame.Navigate(Activator.CreateInstance(pageType));
            }
            catch (Exception ex)
            {
                
            }
        }

        public void Navigate(Type pageType, object viewModel)
        {
            if (pageType == null)
                throw new ArgumentNullException(nameof(pageType));

            try
            {
                var page = (Page)Activator.CreateInstance(pageType); 
                page.DataContext = viewModel; 
                _frame.Navigate(page); 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void GoBack()
        {
            if (_frame.CanGoBack)
                _frame.GoBack();
        }

        public void GoForward()
        {
            if (_frame.CanGoForward)
                _frame.GoForward();
        }
    }
}
