using lab9;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace lab9
{
    // Реализация ICommand для MVVM
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();
        public void Execute(object parameter) => _execute();
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        private string _inputA = "1";
        private string _inputB = "0";
        private string _inputC = "0";

        private string _inputA2 = "1";
        private string _inputB2 = "0";
        private string _inputC2 = "0";

        private string _equationText = "-";
        private string _discriminantText = "-";
        private string _rootsText = "-";
        private string _implicitDoubleText = "-";
        private string _explicitBoolText = "-";
        private string _comparisonResult = "-";

        private QuadraticEquation _currentEquation;

        // Поля ввода для Уравнения 1
        public string InputA
        {
            get => _inputA;
            set { _inputA = value; OnPropertyChanged(); Calculate(); }
        }
        public string InputB
        {
            get => _inputB;
            set { _inputB = value; OnPropertyChanged(); Calculate(); }
        }
        public string InputC
        {
            get => _inputC;
            set { _inputC = value; OnPropertyChanged(); Calculate(); }
        }

        // Поля ввода для Уравнения 2 (для сравнения)
        public string InputA2
        {
            get => _inputA2;
            set { _inputA2 = value; OnPropertyChanged(); }
        }
        public string InputB2
        {
            get => _inputB2;
            set { _inputB2 = value; OnPropertyChanged(); }
        }
        public string InputC2
        {
            get => _inputC2;
            set { _inputC2 = value; OnPropertyChanged(); }
        }

        // Вывод результатов
        public string EquationText
        {
            get => _equationText;
            set { _equationText = value; OnPropertyChanged(); }
        }
        public string DiscriminantText
        {
            get => _discriminantText;
            set { _discriminantText = value; OnPropertyChanged(); }
        }
        public string RootsText
        {
            get => _rootsText;
            set { _rootsText = value; OnPropertyChanged(); }
        }
        public string ImplicitDoubleText
        {
            get => _implicitDoubleText;
            set { _implicitDoubleText = value; OnPropertyChanged(); }
        }
        public string ExplicitBoolText
        {
            get => _explicitBoolText;
            set { _explicitBoolText = value; OnPropertyChanged(); }
        }
        public string ComparisonResult
        {
            get => _comparisonResult;
            set { _comparisonResult = value; OnPropertyChanged(); }
        }

        // Команды для кнопок
        public ICommand IncrementCommand { get; }
        public ICommand DecrementCommand { get; }
        public ICommand CompareCommand { get; }

        public MainViewModel()
        {
            IncrementCommand = new RelayCommand(Increment);
            DecrementCommand = new RelayCommand(Decrement);
            CompareCommand = new RelayCommand(CompareWithSecond);

            Calculate();
        }

        private void Calculate()
        {
            if (!double.TryParse(InputA, out double a) ||
                !double.TryParse(InputB, out double b) ||
                !double.TryParse(InputC, out double c))
            {
                EquationText = "Ошибка ввода: введите корректные числа";
                ClearResults();
                return;
            }

            if (a == 0)
            {
                EquationText = "Ошибка: 'a' не может быть равен 0";
                ClearResults();
                return;
            }

            try
            {
                _currentEquation = new QuadraticEquation(a, b, c);

                EquationText = _currentEquation.ToString();
                DiscriminantText = _currentEquation.Discriminant.ToString("F4");

                // Вычисление корней
                var roots = _currentEquation.GetRoots();
                if (roots.Length == 0) RootsText = "Действительных корней нет";
                else if (roots.Length == 1) RootsText = $"x = {roots[0]:F4}";
                else RootsText = $"x1 = {roots[0]:F4};  x2 = {roots[1]:F4}";

                // Тестирование приведений типов
                double disc = _currentEquation; 
                bool hasRoots = (bool)_currentEquation; 

                ImplicitDoubleText = $"{disc:F4} (неявное приведение к double)";
                ExplicitBoolText = hasRoots ? "true (корни есть)" : "false (корней нет)";
            }
            catch (Exception ex)
            {
                EquationText = $"Ошибка: {ex.Message}";
                ClearResults();
            }
        }

        private void Increment()
        {
            if (_currentEquation == null) return;
            try
            {
                var next = ++_currentEquation;
                InputA = next.A.ToString();
                InputB = next.B.ToString();
                InputC = next.C.ToString();
            }
            catch (Exception ex)
            {
                EquationText = $"Ошибка при ++: {ex.Message}";
            }
        }

        private void Decrement()
        {
            if (_currentEquation == null) return;
            try
            {
                var prev = --_currentEquation;
                InputA = prev.A.ToString();
                InputB = prev.B.ToString();
                InputC = prev.C.ToString();
            }
            catch (Exception ex)
            {
                EquationText = $"Ошибка при --: {ex.Message}";
            }
        }

        private void CompareWithSecond()
        {
            if (_currentEquation == null) return;

            if (!double.TryParse(InputA2, out double a2) ||
                !double.TryParse(InputB2, out double b2) ||
                !double.TryParse(InputC2, out double c2))
            {
                ComparisonResult = "Ошибка: проверьте коэффициенты второго уравнения";
                return;
            }

            if (a2 == 0)
            {
                ComparisonResult = "Ошибка: 'a2' не может быть равен 0";
                return;
            }

            var eq2 = new QuadraticEquation(a2, b2, c2);
            bool isEqual = _currentEquation == eq2;
            bool isNotEqual = _currentEquation != eq2;

            ComparisonResult = $"Уравнение 1 == Уравнение 2: {isEqual}\nУравнение 1 != Уравнение 2: {isNotEqual}";
        }

        private void ClearResults()
        {
            _currentEquation = null;
            DiscriminantText = "-";
            RootsText = "-";
            ImplicitDoubleText = "-";
            ExplicitBoolText = "-";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}