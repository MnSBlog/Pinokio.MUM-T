using System;
using System.Collections.Generic;
using System.Text;

namespace Pinokio.Simulation
{
    public struct SimTime : IComparable, IEquatable<SimTime>
    {
        private long _value; // Milliseconds

        public double TotalDays
        {
            get { return ToDay(); }
        }

        public double TotalHours
        {
            get { return ToHours(); }
        }

        public double TotalMinutes
        {
            get { return ToMinutes(); }
        }

        public double TotalSeconds
        {
            get { return ToSecond(); }
        }

        public SimTime(long second)
        {
            _value = second * 1000;
        }
        public SimTime(float second)
        {
            _value = (long)second * 1000;
        }
        public SimTime(double second)
        {
            _value = (long)(second * 1000);
        }

        public SimTime(DateTime dt)
        {
            TimeSpan ts = dt.TimeOfDay;
            _value = (long)ts.TotalMilliseconds;
        }
        public SimTime(TimeSpan ts)
        {
            _value = (long)ts.TotalMilliseconds;
        }

        public static SimTime operator -(SimTime time)
        {
            return (SimTime)(-time._value);
        }
        public static SimTime operator -(SimTime left, SimTime right)
        {
            SimTime time;
            time._value = left._value - right._value;
            return time;
        }
        public static SimTime operator +(SimTime left, SimTime right)
        {
            SimTime time;
            time._value = left._value + right._value;
            return time;
        }

        #region [:: Logical Operator]
        public static bool operator !=(double left, SimTime right)
        {
            return left != right._value;
        }
        public static bool operator !=(SimTime left, double right)
        {
            return right != left._value;
        }
        public static bool operator !=(SimTime left, SimTime right)
        {
            return left._value != right._value;
        }
        public static bool operator !=(SimTime left, float right)
        {
            return left._value != right;
        }
        public static bool operator !=(float left, SimTime right)
        {
            return right._value != left;
        }
        public static bool operator ==(double left, SimTime right)
        {
            return left == right._value;
        }
        public static bool operator ==(SimTime left, double right)
        {
            return left._value == right;
        }
        public static bool operator ==(SimTime left, SimTime right)
        {
            return left._value == right._value;
        }
        public static bool operator ==(SimTime left, float right)
        {
            return left._value == right;
        }
        public static bool operator ==(float left, SimTime right)
        {
            return right._value == left;
        }
        public static bool operator <(SimTime left, SimTime right)
        {
            return left._value < right._value;
        }
        public static bool operator <=(SimTime left, SimTime right)
        {
            return left._value <= right._value;
        }
        public static bool operator >(SimTime left, SimTime right)
        {
            return left._value > right._value;
        }
        public static bool operator >=(SimTime left, SimTime right)
        {
            return left._value >= right._value;
        }
        #endregion [Logical Operator ::]

        #region [:: Casting Operator]
        public static explicit operator double(SimTime time)
        {
            return time.ToSecond();
        }

        public static explicit operator float(SimTime time)
        {
            return (float)time.ToSecond();
        }

        public static explicit operator TimeSpan(SimTime time)
        {
            TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, (int)time._value);//수정
            return timeSpan;
        }

        public static explicit operator DateTime(SimTime time)
        {
            DateTime dateTime = new DateTime();
            dateTime += (TimeSpan)time;
            return dateTime;
        }

        public static implicit operator SimTime(double d)
        {
            SimTime time = new SimTime(d);
            return time;
        }

        public static implicit operator SimTime(float f)
        {
            SimTime time = new SimTime(f);
            return time;
        }

        public static implicit operator SimTime(DateTime dt)
        {
            return new SimTime(dt.Year);
        }

        public static implicit operator SimTime(TimeSpan timeSpan)
        {
            return new SimTime(timeSpan);
        }
        #endregion [Casting Operator ::]

        #region [:: Operator %]
        public static SimTime operator %(SimTime left, SimTime right)
        {
            return left._value % right._value;
        }
        public static SimTime operator %(SimTime left, double right)
        {
            return left._value % right;
        }
        public static SimTime operator %(double left, SimTime right)
        {
            return left % right._value;
        }
        public static SimTime operator %(SimTime left, float right)
        {
            return left._value % right;
        }
        public static SimTime operator %(float left, SimTime right)
        {
            return left % right._value;
        }
        #endregion [Operator % ::]

        #region [:: Operator *]
        public static SimTime operator *(double left, SimTime right)
        {
            return left * right._value;
        }
        public static SimTime operator *(SimTime left, double right)
        {
            return left._value * right;
        }
        public static SimTime operator *(SimTime left, SimTime right)
        {
            return left._value * right._value;
        }
        public static SimTime operator *(float left, SimTime right)
        {
            return left * right._value;
        }
        public static SimTime operator *(SimTime left, float right)
        {
            return left._value * right;
        }
        #endregion [Operator * ::]

        #region [:: Operator /]
        public static SimTime operator /(SimTime left, double right)
        {
            return left._value / right;
        }
        public static SimTime operator /(double left, SimTime right)
        {
            return left / right._value;
        }
        public static SimTime operator /(SimTime left, SimTime right)
        {
            return left._value / right._value;
        }
        public static SimTime operator /(float left, SimTime right)
        {
            return left / right._value;
        }
        public static SimTime operator /(SimTime left, float right)
        {
            return left._value / right;
        }
        #endregion [Operator / ::]

        #region [:: Interface Implementation]
        public int CompareTo(object other)
        {
            SimTime time = (SimTime)other;
            return this._value.CompareTo(time._value);
        }

        public override bool Equals(object obj)
        {
            return obj.Equals(this);
        }
        public override int GetHashCode()
        {
            byte[] data = BitConverter.GetBytes(_value);
            int x = BitConverter.ToInt32(data, 0);
            int y = BitConverter.ToInt32(data, 4);
            return x ^ y;
        }
        public override string ToString()
        {
            return this.ToSecond().ToString();
        }
        #endregion [Interface Implementation ::]

        public int ToDay()
        {
            return (int)(_value / (3600 * 24 * 1000));
        }
        public int ToHours()
        {
            return (int)(_value / (3600 * 1000));
        }
        public int ToMinutes()
        {
            return (int)(_value / (60 * 1000));
        }
        public double ToSecond()
        {
            return (double)_value / 1000.0;
        }

        public DateTime ToDateTime()
        {
            TimeSpan span = new TimeSpan(0, 0, (int)ToSecond());
            return SimParameter.StartDateTime + span;
        }

        public static SimTime FromDays(int day)
        {
            SimTime time;
            time._value = day * 3600 * 24 * 1000;
            return time;
        }
        public static SimTime FromHours(int hour)
        {
            SimTime time;
            time._value = hour * 3600 * 1000;
            return time;
        }
        public static SimTime FromMinutes(int min)
        {
            SimTime time;
            time._value = min * 60 * 1000;
            return time;
        }

        public static SimTime FromSeconds(int sec)
        {
            SimTime time;
            time._value = sec * 1000;
            return time;
        }

        public static SimTime FromMilliseconds(int ms)
        {
            SimTime time;
            time._value = ms;
            return time;
        }

        public static SimTime Max(SimTime left, SimTime right)
        {
            return (left._value >= right._value) ? left : right;
        }
        public static SimTime Min(SimTime left, SimTime right)
        {
            return (left._value <= right._value) ? left : right;
        }
        public static SimTime Abs(SimTime time)
        {
            return (time._value >= 0) ? time : -time;
        }

        public string ToString(string format)
        {
            SimTime time = new SimTime(_value);
            DateTime dateTime = (DateTime)time;
            return dateTime.ToString(format);
        }

        public bool Equals(SimTime other)
        {
            if (System.Object.ReferenceEquals(other, null)) return false;

            if (this.GetType() != other.GetType()) return false;

            if (System.Object.ReferenceEquals(this, other)) return true;

            if (this._value == other._value) return true;

            return false;
        }

        public void AddSeconds(double seconds)
        {
            _value += (int)(seconds * 1000);
        }

        public void AddMiniutes(double minutes)
        {
            _value += (int)(60 * 1000 * minutes);
        }
    }
}
