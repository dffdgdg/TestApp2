using TestApp.Model;

namespace TestApp.ViewModel
{
    public class TestResultVM : BaseViewModel
    {
        private int _totalQuestions;
        private double _totalScore;
        private double _averageScore;
        private DateTime _completionDate;
        private TimeSpan _duration;

        public int TotalQuestions
        {
            get => _totalQuestions;
            set => SetProperty(ref _totalQuestions, value);
        }

        public double TotalScore
        {
            get => _totalScore;
            set => SetProperty(ref _totalScore, value);
        }

        public double AverageScore
        {
            get => _averageScore;
            set => SetProperty(ref _averageScore, value);
        }

        public DateTime CompletionDate
        {
            get => _completionDate;
            set => SetProperty(ref _completionDate, value);
        }

        public TimeSpan Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }
        public string FormattedDuration
        {
            get
            {
                var hours = Duration.Hours;
                var minutes = Duration.Minutes;
                var seconds = Duration.Seconds;

                var result = new List<string>();
                if (hours > 0) result.Add($"{hours} час{(hours > 1 ? "а" : "")}");
                if (minutes > 0) result.Add($"{minutes} минут{(minutes > 1 ? "ы" : "")}");
                if (seconds > 0) result.Add($"{seconds} секунд{(seconds > 1 ? "ы" : "")}");

                return string.Join(", ", result);
            }
        }
        public TestResultVM(int totalQuestions, Userresult result, DateTime completionDate, TimeSpan duration)
        {
            TotalQuestions = totalQuestions;
            TotalScore = result.Score;
            CompletionDate = completionDate;
            Duration = duration;
        }
    }
}