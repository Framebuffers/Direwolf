using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Direwolf.Driver.MCP;

    public class StreamingConsole : Window, INotifyPropertyChanged
    {
        private TextBox? _outputTextBox;
        private TextBox? _inputTextBox;
        private ScrollViewer? _scrollViewer;
        private StringBuilder? _consoleOutput;
        private DispatcherTimer? _streamingTimer;
        private Queue<string>? _pendingMessages;
        
        public event PropertyChangedEventHandler? PropertyChanged;
        
        public StreamingConsole()
        {
            InitializeWindow();
            InitializeConsole();
            StartStreamingTimer();
            InitialMessage("""
                            Direwolf Streaming Console
                            (C) 2025 Sebastian Torres Sagredo (Framebuffer)
                            
                            Revision v0.2-alpha.
                            
                           """);
        }
        
        private void InitializeWindow()
        {
            Title = "Direwolf Prompt Output";
            Width = 800;
            Height = 600;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.CanResize;
            Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            Foreground = Brushes.White;
        }
        
        private void InitializeConsole()
        {
            _consoleOutput = new StringBuilder();
            _pendingMessages = new Queue<string>();
            
            CreateLayout();
        }
        
        private void CreateLayout()
        {
            Grid mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            
            _scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            
            _outputTextBox = new TextBox
            {
                Background = new SolidColorBrush(Color.FromRgb(20, 20, 20)),
                Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                FontFamily = new FontFamily("Consolas"),
                FontSize = 12,
                IsReadOnly = true,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(5),
                Padding = new Thickness(10)
            };
            
            _scrollViewer.Content = _outputTextBox;
            Grid.SetRow(_scrollViewer, 0);
            mainGrid.Children.Add(_scrollViewer);
            
            Grid inputGrid = new Grid();
            inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            
            Label promptLabel = new Label
            {
                Content = ">",
                Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 255)),
                FontFamily = new FontFamily("Consolas"),
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5, 0, 0, 0)
            };
            Grid.SetColumn(promptLabel, 0);
            inputGrid.Children.Add(promptLabel);
            
            _inputTextBox = new TextBox
            {
                Background = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                Foreground = Brushes.White,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 12,
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                BorderBrush = new SolidColorBrush(Color.FromRgb(100, 100, 100))
            };
            
            Grid.SetColumn(_inputTextBox, 1);
            inputGrid.Children.Add(_inputTextBox);
            
            Grid.SetRow(inputGrid, 1);
            mainGrid.Children.Add(inputGrid);
            
            Content = mainGrid;
            
            Loaded += (s, e) => _inputTextBox.Focus();
        }
        
        private void StartStreamingTimer()
        {
            _streamingTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _streamingTimer.Tick += ProcessPendingMessages!;
            _streamingTimer.Start();
        }
        
        private void ProcessPendingMessages(object sender, System.EventArgs e)
        {
            if (_pendingMessages?.Count > 0)
            {
                var message = _pendingMessages.Dequeue();
                _consoleOutput?.AppendLine(message);
                UpdateOutput();
            }
        }
        
        private void InitialMessage(string message)
        {
            AppendToConsole(message);
            
            Task.Run(async () =>
            {
              
                    await Task.Delay(500);
                    StreamMessage($"{message} - {DateTime.Now:HH:mm:ss.fff}");
                
            });
        }
   
        public void StreamMessage(string message)
        {
            _pendingMessages?.Enqueue($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
        }
        
        private void AppendToConsole(string message)
        {
            _consoleOutput?.AppendLine(message);
            UpdateOutput();
        }
        
        private void UpdateOutput()
        {
            Dispatcher.Invoke(() =>
            {
                _outputTextBox!.Text = _consoleOutput?.ToString() ?? string.Empty;
                _scrollViewer?.ScrollToEnd();
            });
        }
        
        protected override void OnClosing(CancelEventArgs e)
        {
            _streamingTimer?.Stop();
            base.OnClosing(e);
        }
    }