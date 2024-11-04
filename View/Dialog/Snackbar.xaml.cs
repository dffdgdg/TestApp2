using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TestApp.View
{
    public partial class Snackbar : UserControl
    {
        private TranslateTransform _translateTransform;

        public Snackbar()
        {
            InitializeComponent();
            Visibility = Visibility.Collapsed; // Скрываем элемент по умолчанию
            _translateTransform = new TranslateTransform(0, 70); // Инициализируем с начальной позицией 70 (50 + 20)
            RenderTransform = _translateTransform; // Устанавливаем Transform
        }

        public void Show(string message)
        {
            Message = message;

            Visibility = Visibility.Visible; // Делаем видимым
            Opacity = 1; // Убираем прозрачность

            // Анимация появления
            var slideInAnimation = new DoubleAnimation(70, 0, TimeSpan.FromMilliseconds(300)); // Начальная позиция 70
            var fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));

            // Начинаем анимации
            _translateTransform.BeginAnimation(TranslateTransform.YProperty, slideInAnimation);
            BeginAnimation(OpacityProperty, fadeInAnimation);

            // Запускаем таймер на 3 секунды для скрытия Snackbar
            var timer = new System.Timers.Timer(3000);
            timer.Elapsed += (s, e) =>
            {
                Dispatcher.Invoke(() => { Hide(); });
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        public void Hide()
        {
            // Создаем анимации для исчезновения
            var slideOutAnimation = new DoubleAnimation(0, 70, TimeSpan.FromMilliseconds(300)); // Возвращаем на 70
            var fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));

            // Скрываем контрол и запускаем анимации
            _translateTransform.BeginAnimation(TranslateTransform.YProperty, slideOutAnimation);
            BeginAnimation(OpacityProperty, fadeOutAnimation);

            fadeOutAnimation.Completed += (s, e) =>
            {
                Message = string.Empty;
                Visibility = Visibility.Collapsed; // Скрываем элемент после исчезновения
            };
        }

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(Snackbar), new PropertyMetadata(string.Empty));
    }
}