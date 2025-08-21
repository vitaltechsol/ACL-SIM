using System;
using System.ComponentModel;   // ISynchronizeInvoke (WinForms)
using System.Collections.Generic;
using System.Timers;

namespace ACLSim
{
    public sealed class InactivityDebouncer<T> : IDisposable
    {
        private readonly Timer _timer;
        private readonly object _gate = new object();
        private readonly Func<T, T, bool> _equals;
        private readonly Action<T> _onStart;   // e.g., APIsOn
        private readonly Action<T> _onIdle;    // e.g., APIsOff
        private readonly Action<T> _onChange;  // e.g., MoveTo

        private bool _hasValue;
        private bool _active;   // started = true (APIsOn called), waiting for idle to turn off
        private T _last;

        public TimeSpan Delay { get; private set; }

        public InactivityDebouncer(
            TimeSpan delay,
            Action<T> onStart,
            Action<T> onIdle,
            Func<T, T, bool> equals,
            ISynchronizeInvoke marshalTo,
            Action<T> onChange)
        {
            if (onStart == null) throw new ArgumentNullException("onStart");
            if (onIdle == null) throw new ArgumentNullException("onIdle");
            if (equals == null) throw new ArgumentNullException("equals");
            if (onChange == null) throw new ArgumentNullException("onChange");

            Delay = delay;
            _onStart = onStart;
            _onIdle = onIdle;
            _equals = equals;
            _onChange = onChange;

            _timer = new Timer(delay.TotalMilliseconds);
            _timer.AutoReset = false;      // fire once, then stop
            _timer.Enabled = false;
            _timer.SynchronizingObject = marshalTo;
            _timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            T value;
            bool shouldFire = false;

            lock (_gate)
            {
                _timer.Stop(); // make sure it’s stopped after firing

                if (_active && _hasValue)
                {
                    value = _last;
                    _active = false;   // back to idle; next Signal() will call onStart again
                    shouldFire = true;
                }
                else
                {
                    return;
                }
            }

            _onIdle(value);
        }

        // Feed values; only treat as "activity" if changed per _equals.
        public void Signal(T value)
        {
            lock (_gate)
            {
                if (!_hasValue)
                {
                    _last = value;
                    _hasValue = true;

                    if (!_active)
                    {
                        _onStart(value); // APIsOn
                        _active = true;
                    }

                    _onChange(value);    // first value counts as an actionable change
                    _timer.Stop();
                    _timer.Start();
                    return;
                }

                if (!_active)
                {
                    _onStart(value);     // APIsOn again after idle
                    _active = true;

                    _onChange(value);
                    _timer.Stop();
                    _timer.Start();
                    return;
                }

                // Change gate: only when NOT equal per comparer
                if (!_equals(value, _last))
                {
                    _last = value;
                    _onChange(value);    // e.g., axisRoll.MoveTo(value)
                    _timer.Stop();
                    _timer.Start();
                }
                // If equal (within threshold), let the countdown continue unchanged.
            }
        }

        public void Cancel()
        {
            lock (_gate)
            {
                _timer.Stop();
                _active = false;
                _hasValue = false;
            }
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
