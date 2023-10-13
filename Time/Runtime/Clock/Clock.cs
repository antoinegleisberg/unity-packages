using UnityEngine;


namespace antoinegleisberg.Time.Clock
{
    public class Clock
    {
        private float _timeInSeconds;
        private bool _isRunning;
        private float _timeScale;

        private float _realtimeAtLastCall;
        
        public Clock(bool startClock = true, float timeScale = 1f)
        {
            _timeInSeconds = 0f;
            _isRunning = startClock;
            _timeScale = timeScale;
            _realtimeAtLastCall = UnityEngine.Time.realtimeSinceStartup;
        }

        public float TotalElapsedSeconds {
            get
            {
                UpdateTime();
                return _timeInSeconds;   
            }
        }

        public bool IsRunning { get => _isRunning; }

        public string GetFormattedTime()
        {
            UpdateTime();
            int totalSeconds = Mathf.RoundToInt(_timeInSeconds);
            int hours = totalSeconds / 3600;
            totalSeconds %= 3600;
            int minutes = totalSeconds / 60;
            totalSeconds %= 60;
            string hoursString = hours.ToString();
            string minutesString = minutes.ToString();
            string secondsString = totalSeconds.ToString();
            if (secondsString.Length < 2)
            {
                secondsString = "0" + secondsString;
            }
            if (minutesString.Length < 2)
            {
                minutesString = "0" + minutesString;
            }
            if (hoursString == "0")
            {
                return minutesString + ":" + secondsString; 
            }
            if (hoursString.Length < 2)
            {
                hoursString = "0" + hoursString;
            }
            return hoursString + ":" + minutesString + ":" + secondsString;
        }

        public void Pause()
        {
            if (!_isRunning)
            {
                return;
            }
            UpdateTime();
            _isRunning = false;
        }

        public void Unpause()
        {
            if (_isRunning)
            {
                return;
            }
            _realtimeAtLastCall = UnityEngine.Time.realtimeSinceStartup;
            _isRunning = true;
        }

        private void UpdateTime()
        {
            if (!_isRunning)
            {
                return;
            }
            float currentRealTime = UnityEngine.Time.realtimeSinceStartup;
            float elapsed = currentRealTime - _realtimeAtLastCall;
            _timeInSeconds += elapsed * _timeScale;
            _realtimeAtLastCall = currentRealTime;
        }
    }
}
