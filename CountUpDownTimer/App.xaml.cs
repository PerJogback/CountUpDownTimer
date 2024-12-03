using Microsoft.Extensions.Logging.Abstractions;

namespace CountUpDownTimer
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window window = base.CreateWindow(activationState);

            window.Created += (s, e) => LogEvent(nameof(window.Created));
            window.Activated += (s, e) => LogEvent(nameof(window.Activated));
            window.Deactivated += (s, e) => LogEvent(nameof(window.Deactivated));
            window.Stopped += (s, e) => LogEvent(nameof(window.Stopped));
            window.Resumed += (s, e) => LogEvent(nameof(window.Resumed));
            window.Destroying += (s, e) => LogEvent(nameof(window.Destroying));

            window.Activated += UpdateTimersIfHasBeenAsleep;
            window.Created += UpdateTimersIfHasBeenAsleep;
            window.Resumed += UpdateTimersIfHasBeenAsleep;
            window.Deactivated += StopTimers;
            window.Stopped += StopTimers;
            window.Destroying += StopTimers;

            return window;
        }

        private void StopTimers(object? sender, EventArgs e)
        {
            (MainPage as MainPage)?.StopTimers();
        }

        private void UpdateTimersIfHasBeenAsleep(object? sender, EventArgs e)
        {
            (MainPage as MainPage)?.UpdateTimersIfHasBeenAsleep();
        }

        static bool LogEvent(string eventName, string? type = null)
        {
            System.Diagnostics.Debug.WriteLine($"Window Lifecycle event: {eventName}{(type == null ? string.Empty : $" ({type})")}");
            return true;
        }
    }
}