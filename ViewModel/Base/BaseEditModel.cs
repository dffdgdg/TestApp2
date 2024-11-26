using CommunityToolkit.Mvvm.Input;

namespace TestApp.ViewModel.Base;
public abstract class BaseEditModel<T> : BaseUserControlViewModel
{
    private RelayCommand<T>? open;
    private RelayCommand? add, save, loadImage;
    private RelayCommand<T>? remove, edit;

    public RelayCommand<T> Open => open ??= new RelayCommand<T>(OnOpen);
    public RelayCommand Add => add ??= new RelayCommand(OnAdd);
    public RelayCommand<T> Remove => remove ??= new RelayCommand<T>(OnRemove);
    public RelayCommand Save => save ??= new RelayCommand(OnSave);
    public RelayCommand<T> Edit => edit ??= new RelayCommand<T>(OnEdit);

    protected abstract void OnOpen(T item);
    protected abstract void OnAdd();
    protected abstract void OnRemove(T item);
    protected abstract void OnSave();
    protected abstract void OnEdit(T item);
}
