using CountUpDownTimer.Enums;
using CountUpDownTimer.Helpers;
using Microsoft.Maui.Layouts;

namespace CountUpDownTimer.Layouts
{
#pragma warning disable CS0612 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete

    public class TimerLayout : FlexLayout
    {
        private static readonly Color Red = new Color(255, 0, 0);
        private static readonly Color Green = new Color(0, 255, 0);

        private readonly Label _labelName;
        private readonly Button _buttonDown;
        private readonly Entry _entryHours;
        private readonly Entry _entryMinutes;
        private readonly Entry _entrySeconds;
        private readonly Button _buttonPause;
        private readonly Button _buttonUp;

        public string Name { get; private set; }
        public ActiveState State { get; private set; }

        public TimerLayout() : this(string.Empty)
        {
        }

        public TimerLayout(string name)
        {
            Name = name;
            Direction = FlexDirection.Row;
            _labelName = new Label
            {
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                Text = name + ": ",
                VerticalOptions = LayoutOptions.Center,
            };
            this.SetGrow(_labelName, 1);

            _buttonDown = new Button
            {
                Text = "◀️",
                Margin = 2,
                VerticalOptions = LayoutOptions.Center,
            };
            _buttonDown.Clicked += new EventHandler(Down_Clicked);

            _entryHours = new Entry
            {
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                Keyboard = Keyboard.Numeric,
                Text = "00",
                InputTransparent = false,
                VerticalOptions = LayoutOptions.Center,
            };
            _entryHours.Focused += new EventHandler<FocusEventArgs>(Hours_Focused);
            _entryHours.Unfocused += new EventHandler<FocusEventArgs>(Hours_Unfocused);

            _entryMinutes = new Entry
            {
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                Keyboard = Keyboard.Numeric,
                Text = "00",
                InputTransparent = false,
                VerticalOptions = LayoutOptions.Center,
            };
            _entryMinutes.Focused += new EventHandler<FocusEventArgs>(Minutes_Focused);
            _entryMinutes.Unfocused += new EventHandler<FocusEventArgs>(Minutes_Unfocused);

            _entrySeconds = new Entry
            {
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                Keyboard = Keyboard.Numeric,
                Text = "00",
                InputTransparent = false,
                VerticalOptions = LayoutOptions.Center,
            };
            _entrySeconds.Focused += new EventHandler<FocusEventArgs>(Seconds_Focused);
            _entrySeconds.Unfocused += new EventHandler<FocusEventArgs>(Seconds_Unfocused);

            _buttonPause = new Button
            {
                Text = "⏸",
                Margin = 2,
                VerticalOptions = LayoutOptions.Center,
            };
            _buttonPause.Clicked += new EventHandler(Pause_Clicked);

            _buttonUp = new Button()
            {
                Text = "▶️",
                Margin = 2,
                VerticalOptions = LayoutOptions.Center,
            };
            _buttonUp.Clicked += new EventHandler(Up_Clicked);

            Children.Add(_labelName);
            Children.Add(_buttonDown);
            Children.Add(_entryHours);
            Children.Add(new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                Text = ":",
                VerticalOptions = LayoutOptions.Center,
            });
            Children.Add(_entryMinutes);
            Children.Add(new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                Text = ":",
                VerticalOptions = LayoutOptions.Center
            });
            Children.Add(_entrySeconds);
            Children.Add(_buttonPause);
            Children.Add(_buttonUp);

            RefreshSavedState();
        }

        protected override ILayoutManager CreateLayoutManager()
        {
            return new FlexLayoutManager(this);
        }

        public void RefreshSavedState()
        {
            State = PreferencesHelper.GetActiveState(Name);
            switch (State)
            {
                case ActiveState.UP:
                    _buttonUp.BorderWidth = 1.0;
                    break;

                case ActiveState.DOWN:
                    _buttonDown.BorderWidth = 1.0;
                    break;
            }
        }

        public bool UpActive() => State == ActiveState.UP;

        public bool DownActive() => State == ActiveState.DOWN;

        private void Pause()
        {
            _buttonUp.BorderWidth = 0.0;
            _buttonDown.BorderWidth = 0.0;
            State = ActiveState.OFF;
            PreferencesHelper.SaveActiveState(Name, State);
        }

        private void Up()
        {
            _buttonUp.BorderWidth = 1.0;
            _buttonDown.BorderWidth = 0.0;
            State = ActiveState.UP;
            PreferencesHelper.SaveActiveState(Name, State);
            StartUpTimer();
        }

        private void Down()
        {
            _buttonUp.BorderWidth = 0.0;
            _buttonDown.BorderWidth = 1.0;
            State = ActiveState.DOWN;
            PreferencesHelper.SaveActiveState(Name, State);
            StartDownTimer();
        }

        public void StopTimer() => State = ActiveState.OFF;

        public void StartUpTimer() => StartTimer(new Func<bool>(UpActive), 1);

        public void StartDownTimer() => StartTimer(new Func<bool>(DownActive), -1);

        public void StartTimer(Func<bool> active, int modifier)
        {
            Device.StartTimer(TimeSpan.FromSeconds(1.0), (Func<bool>)(() =>
            {
                Device.BeginInvokeOnMainThread((Action)(() =>
                {
                    if (!active())
                        return;
                    UpdateTimer(modifier);
                }));
                return active();
            }));
        }

        public void UpdateTimer(int modifier)
        {
            TimeSpan timeSpan = PreferencesHelper.GetTimer(Name);
            timeSpan = timeSpan.Add(TimeSpan.FromSeconds((double)modifier));
            PreferencesHelper.SaveTimer(Name, timeSpan);
            PreferencesHelper.SaveLastUpdatedTime(DateTime.Now);
            _entryHours.Text = string.Format("{0:00}", (object)Math.Abs(timeSpan.Hours));
            _entryMinutes.Text = string.Format("{0:00}", (object)Math.Abs(timeSpan.Minutes));
            _entrySeconds.Text = string.Format("{0:00}", (object)Math.Abs(timeSpan.Seconds));
            _labelName.TextColor = timeSpan < TimeSpan.Zero ? Red : Green;
        }

        private void Hours_TextChanged(object? sender, TextChangedEventArgs e)
        {
            TimeSpan timer = PreferencesHelper.GetTimer(Name);
            PreferencesHelper.SaveTimer(Name, timer < TimeSpan.Zero ? new TimeSpan(int.Parse(e.NewTextValue) * -1, timer.Minutes, timer.Seconds) : new TimeSpan(int.Parse(e.NewTextValue), timer.Minutes, timer.Seconds));
            UpdateTimer(0);
        }

        private void Minutes_TextChanged(object? sender, TextChangedEventArgs e)
        {
            TimeSpan timer = PreferencesHelper.GetTimer(Name);
            PreferencesHelper.SaveTimer(Name, timer < TimeSpan.Zero ? new TimeSpan(timer.Hours, int.Parse(e.NewTextValue) * -1, timer.Seconds) : new TimeSpan(timer.Hours, int.Parse(e.NewTextValue), timer.Seconds));
            UpdateTimer(0);
        }

        private void Seconds_TextChanged(object? sender, TextChangedEventArgs e)
        {
            TimeSpan timer = PreferencesHelper.GetTimer(Name);
            PreferencesHelper.SaveTimer(Name, timer < TimeSpan.Zero ? new TimeSpan(timer.Hours, timer.Minutes, int.Parse(e.NewTextValue) * -1) : new TimeSpan(timer.Hours, timer.Minutes, int.Parse(e.NewTextValue)));
            UpdateTimer(0);
        }

        private void Hours_Focused(object? sender, FocusEventArgs e)
        {
            _entryHours.TextChanged += new EventHandler<TextChangedEventArgs>(Hours_TextChanged);
        }

        private void Hours_Unfocused(object? sender, FocusEventArgs e)
        {
            _entryHours.TextChanged -= new EventHandler<TextChangedEventArgs>(Hours_TextChanged);
        }

        private void Minutes_Focused(object? sender, FocusEventArgs e)
        {
            _entryMinutes.TextChanged += new EventHandler<TextChangedEventArgs>(Minutes_TextChanged);
        }

        private void Minutes_Unfocused(object? sender, FocusEventArgs e)
        {
            _entryMinutes.TextChanged -= new EventHandler<TextChangedEventArgs>(Minutes_TextChanged);
        }

        private void Seconds_Focused(object? sender, FocusEventArgs e)
        {
            _entrySeconds.TextChanged += new EventHandler<TextChangedEventArgs>(Seconds_TextChanged);
        }

        private void Seconds_Unfocused(object? sender, FocusEventArgs e)
        {
            _entrySeconds.TextChanged -= new EventHandler<TextChangedEventArgs>(Seconds_TextChanged);
        }

        private void Pause_Clicked(object? sender, EventArgs e) => Pause();

        private void Up_Clicked(object? sender, EventArgs e) => Up();

        private void Down_Clicked(object? sender, EventArgs e) => Down();
    }

#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore CS0612 // Type or member is obsolete
}