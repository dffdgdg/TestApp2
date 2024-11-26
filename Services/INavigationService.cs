namespace TestApp.Services;

public interface INavigationService
{
    void Navigate(Type pageType);
    void Navigate(Type pageType, object vm);
    public void Navigate(Type pageType, params object[] parameters);
    void GoBack();
    void GoForward();
}
