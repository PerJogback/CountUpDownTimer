using CountUpDownTimer.Enums;
using CountUpDownTimer.Helpers;
using CountUpDownTimer.Layouts;

namespace CountUpDownTimer
{
    public partial class MainPage : ContentPage
    {
        private readonly List<TimerLayout> _timerLayouts = [];

        public MainPage()
        {
            InitializeComponent();
            foreach (string timerName in PreferencesHelper.GetTimerNames())
            {
                var timerLayout = new TimerLayout(timerName);
                _timerLayouts.Add(timerLayout);
                mainStackLayout.Children.Add(timerLayout);
            }
            UpdateTimersIfHasBeenAsleep();
            ResetBottomButtonsPosition();
        }

        public void UpdateTimersIfHasBeenAsleep()
        {
            TimeSpan lastUpdatedTime = PreferencesHelper.GetLastUpdatedTime();
            if (Math.Abs(lastUpdatedTime.TotalSeconds) < 5.0)
                return;
            this.UpdateTimers((int)lastUpdatedTime.TotalSeconds);
        }

        public void UpdateTimers(int modifier)
        {
            foreach (var timerLayout in _timerLayouts)
            {
                timerLayout.RefreshSavedState();
                switch (timerLayout.State)
                {
                    case ActiveState.UP:
                        timerLayout.UpdateTimer(modifier);
                        timerLayout.StartUpTimer();
                        break;

                    case ActiveState.DOWN:
                        timerLayout.UpdateTimer(modifier * -1);
                        timerLayout.StartDownTimer();
                        break;

                    default:
                        timerLayout.UpdateTimer(0);
                        break;
                }
            }
        }

        public void StopTimers()
        {
            foreach (var timerLayout in _timerLayouts)
                timerLayout.StopTimer();
        }

        private void ResetBottomButtonsPosition()
        {
            mainStackLayout.Children.Remove(buttonsStackLayout);
            mainStackLayout.Children.Add(buttonsStackLayout);
        }

        private async void PlusButtonClicked(object sender, EventArgs e)
        {
            var name = await DisplayPromptAsync("Choose label", "What is the timer label?");
            if (string.IsNullOrWhiteSpace(name))
                return;

            PreferencesHelper.AddTimerName(name);
            var newTimerLayout = new TimerLayout(name);
            _timerLayouts.Add(newTimerLayout);
            mainStackLayout.Children.Add(newTimerLayout);
            ResetBottomButtonsPosition();
        }

        private void MinusButtonClicked(object sender, EventArgs e)
        {
            if (_timerLayouts.Count == 0)
                return;

            var timerLayout = _timerLayouts.Last();
            PreferencesHelper.RemoveTimerName(timerLayout.Name);
            mainStackLayout.Children.Remove(timerLayout);
            _timerLayouts.Remove(timerLayout);
        }
    }
}