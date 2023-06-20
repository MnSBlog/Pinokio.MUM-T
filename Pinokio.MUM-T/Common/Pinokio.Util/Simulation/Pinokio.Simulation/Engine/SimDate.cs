using System;
using System.Collections.Generic;
using System.Text;

namespace Pinokio.Simulation
{
    public class SimDate : IComparable<SimDate>, IEquatable<SimDate>
    {
        public int Year = DateTime.Now.Year;
        public int Month = DateTime.Now.Month;
        public int Day = DateTime.Now.Day;

        public SimDate() { }

        public SimDate(string dateString)
        {
            string[] splitedStr = dateString.Split('/');
            if (splitedStr.Length == 3)
            {
                Year = Convert.ToInt32(splitedStr[0]);
                Month = Convert.ToInt32(splitedStr[1]);
                Day = Convert.ToInt32(splitedStr[2]);
            }
        }

        public SimDate(SimDate otherDate)
        {
            Year = otherDate.Year;
            Month = otherDate.Month;
            Day = otherDate.Day;
        }

        public void DayUp()
        {
            if (DateTime.DaysInMonth(Year, Month) >= Day + 1)
            {
                Day += 1;
                return;
            }
            else if (12 > Month + 1)
            {
                Month += 1;
            }
            else
            {
                Year += 1;
                Month = 1;
            }

            Day = 1;
        }

        public void DayDown()
        {
            if (Day > 1)
            {
                Day -= 1;
                return;
            }
            else if (Month > 1)
            {
                Month -= 1;
            }
            else
            {
                Month = 12;
                Year -= 1;
            }

            Day = DateTime.DaysInMonth(Year, Month);
        }

        int IComparable<SimDate>.CompareTo(SimDate other)
        {
            if (this == other) return 0;
            else if (this < other) return -1;
            else return 1;
        }

        bool IEquatable<SimDate>.Equals(SimDate other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            string yearString = this.Year.ToString().PadRight(6, '0');
            string monthString = this.Month.ToString().PadLeft(2, '0');
            string dayString = this.Day.ToString().PadLeft(2, '0');

            string temp = yearString + monthString + dayString;

            return Convert.ToInt32(temp);
        }
        public override string ToString()
        {
            return Year + "/" + Month + "/" + Day;
        }

        public static bool operator ==(SimDate date1, SimDate date2)
        {
            return date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day;            
        }

        public static bool operator !=(SimDate date1, SimDate date2)
        {
            return !(date1 == date2);
        }

        public static bool operator <(SimDate date1, SimDate date2)
        {
            if (date1.Year > date2.Year)
            {
                return false;
            }
            else if (date1.Year < date2.Year)
            {
                return true;
            }
            else
            {
                if (date1.Month > date2.Month)
                {
                    return false;
                }
                else if (date1.Month < date2.Month)
                {
                    return true;
                }
                else
                {
                    if (date1.Day > date2.Day)
                    {
                        return false;
                    }
                    else if (date1.Day < date2.Day)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public static bool operator >(SimDate date1, SimDate date2)
        {
            if (date1 == date2) return false;
            else if (date1 < date2) return false;
            else return true;
        }

        public static bool operator <=(SimDate date1, SimDate date2)
        {
            return !(date1 > date2);
        }

        public static bool operator >=(SimDate date1, SimDate date2)
        {
            return !(date1 < date2);
        }

        public DateTime ToDateTime()
        {
            return new DateTime(Year, Month, Day);
        }
    }
}
