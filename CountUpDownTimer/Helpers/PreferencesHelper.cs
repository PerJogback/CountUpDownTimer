using CountUpDownTimer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountUpDownTimer.Helpers
{
    public static class PreferencesHelper
    {
        private const string LAST_UPDATED_TIME = "LastUpdatedTime";
        private const string KEY_TIMER = "Timer";
        private const string KEY_STATE = "State";
        private const string KEY_TIMER_NAMES = "TimerNames";
        private const string TIMER_NAME_SPLITTER = "|TimerNameSplitter|";

        public static IEnumerable<string> GetTimerNames()
        {
            string str = Preferences.Get("TimerNames", string.Empty);
            if (string.IsNullOrEmpty(str))
                return (IEnumerable<string>)new string[0];
            return (IEnumerable<string>)str.Split(new string[1]
            {
        "|TimerNameSplitter|"
            }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static void SaveTimerNames(IEnumerable<string> timerNames)
        {
            Preferences.Set("TimerNames", string.Join("|TimerNameSplitter|", timerNames));
        }

        public static void AddTimerName(string timerName)
        {
            List<string> list = PreferencesHelper.GetTimerNames().ToList<string>();
            list.Add(timerName);
            Preferences.Set("TimerNames", string.Join("|TimerNameSplitter|", (IEnumerable<string>)list));
        }

        public static void RemoveTimerName(string timerName)
        {
            List<string> list = PreferencesHelper.GetTimerNames().ToList<string>();
            list.Remove(timerName);
            Preferences.Set("TimerNames", string.Join("|TimerNameSplitter|", (IEnumerable<string>)list));
        }

        public static TimeSpan GetLastUpdatedTime()
        {
            string s = Preferences.Get("LastUpdatedTime", string.Empty);
            return !string.IsNullOrEmpty(s) ? DateTime.Now.Subtract(DateTime.Parse(s)) : TimeSpan.Zero;
        }

        public static void SaveLastUpdatedTime(DateTime time)
        {
            Preferences.Set("LastUpdatedTime", time.ToString());
        }

        public static TimeSpan GetTimer(string name)
        {
            TimeSpan result;
            return TimeSpan.TryParse(Preferences.Get("Timer" + name, string.Empty), out result) ? result : TimeSpan.Zero;
        }

        public static void SaveTimer(string name, TimeSpan timeSpan)
        {
            Preferences.Set("Timer" + name, timeSpan.ToString());
        }

        public static ActiveState GetActiveState(string name)
        {
            string str = Preferences.Get("State" + name, string.Empty);
            return !string.IsNullOrWhiteSpace(str) ? (ActiveState)Enum.Parse(typeof(ActiveState), str) : ActiveState.OFF;
        }

        public static void SaveActiveState(string name, ActiveState state)
        {
            Preferences.Set("State" + name, state.ToString());
        }
    }
}