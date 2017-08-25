using System;
using System.Collections.Generic;
using XUtliPoolLib;
using UnityEngine;

namespace XMainClient
{
    
    public class XDateTime
    {
        public const long SecondsPerYear = 525600; //365*24min
        public const long SecondsPerDay = 1440; //24min
        public const long SecondsPerHour = 60;
        public const long SecondsPerMinute = 1;

        public const int DayPerYear = 365;
        public const int HourPerDay = 24;
        public const int MinutePerHour = 60;

        public static int[] Days = {31,28,31,30,31,30,31,31,30,31,30,31};
        public static float[] DayBegin = { 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f };
        public static float[] DayPercent = {0.47f,0.48f,0.49f,0.5f,0.5f,0.51f,0.52f,0.53f,0.52f,0.51f,0.50f,0.48f };

        public static bool IsLeapYear(int Year)
        {
            if ((Year % 4 == 0 && Year % 100 != 0) || (Year % 400 == 0))
            {
                return true;
            }
            else
                return false;
        }

        public static bool IsSpring(int iMonth)
        {
            if (iMonth >= 3 && iMonth <= 5) return true;
            return false;
        }

        public static bool IsSummer(int iMonth)
        {
            if (iMonth >= 6 && iMonth <= 8) return true;
            return false;
        }

        public static bool IsAutumn(int iMonth)
        {
            if (iMonth >= 9 && iMonth <= 11) return true;
            return false;
        }

        public static bool IsWinter(int iMonth)
        {
            if (iMonth >= 12 && iMonth <= 2) return true;
            return false;
        }

        public static int GetDays(int iMonth, bool bLeapYear = false)
        {
            int ret = GetDays(iMonth);
            if (bLeapYear && iMonth == 2) ret++;
            return ret;
        }

        public static int GetDays(int iMonth)
        {
            if (iMonth <= 12 && iMonth >= 1)
                return Days[iMonth - 1];

            XDebug.singleton.AddErrorLog("Wrong Month Index!!");
            return 0;
        }
        
        public XDateTime(long dateTime)
        {
            Reset(dateTime);
        }

        public void Reset(long dateTime)
        {
            CurrentTime = dateTime;

            Calculate();
        }

        private void Calculate()
        {
            YY = 1+(int)(CurrentTime / SecondsPerYear);
            long temp = CurrentTime % SecondsPerYear;
            int i = 0;
            for(;i<12;++i)
            {
                long t = Days[i] * SecondsPerDay;
                if (temp> t)
                {
                    temp -= t;
                }else
                {
                    break;
                }                
            }
            MM = i + 1;
            DD = 1 + (int)(temp / SecondsPerDay);
            temp = temp % SecondsPerDay;
            hh = (int)(temp / SecondsPerHour);
            mm = (int)(temp % SecondsPerHour);

            bIsDay = IsDay;
        }

        public EXSeason Season
        {
            get
            {
                if (IsSpring(MM)) return EXSeason.ESpring;
                if (IsSummer(MM)) return EXSeason.ESummer;
                if (IsAutumn(MM)) return EXSeason.EAutumn;
                return EXSeason.EWinter;
            }
        }

        public bool IsDay
        {
            get{
                float begin = DayBegin[MM - 1];
                float end = begin + DayPercent[MM - 1];
                long cur = hh * SecondsPerHour + mm;
                return (cur <= SecondsPerHour*end && cur > SecondsPerHour*begin);
            }
        }

        public bool IsNight
        {
            get { return !IsDay; }
        }

        public override string ToString()
        {
            return string.Format("{0}年{1}月{2}日 {3}:{4}",YY,MM,DD,hh,mm);
        }

        public void AddYear(int t)
        {
            YY += t;
        }

        public void AddDay(int t)
        {
            int tdds = 0;
            for(int j=0;j<MM-1;++j)
            {
                tdds += Days[j];
            }

            int temp = tdds + DD + t;
            if(temp > DayPerYear)
            {
                AddYear(temp / DayPerYear);

                temp = temp % DayPerYear;
            }
            int i = 0;
            for(;i<12;++i)
            {
                if(temp > Days[i])
                {
                    temp -= Days[i];
                }
                else
                {
                    break;
                }
            }
            MM = 1 + i;
            DD = temp;

            if (DayHandler != null)
                DayHandler(this);
        }

        public void AddHour(int t)
        {
            int temp = hh + t;
            if (temp >= HourPerDay)
            {
                hh = temp % HourPerDay;
                AddDay(temp / HourPerDay);
            }
            else
            {
                hh = temp;
            }

            bool prev = bIsDay;
            bIsDay = IsDay;

            if (DayNightHandler != null && prev != bIsDay)
            {
                DayNightHandler(bIsDay);
            }
        }

        public void AddMinute(int t)
        {
            int temp = mm + t;
            if( temp >= MinutePerHour)
            {
                mm = temp % MinutePerHour;
                AddHour(temp / MinutePerHour);
            }
            else
            {
                mm = temp;
            }
            if (MinuteHandler != null)
                MinuteHandler(this);
        }

        public long CurrentTime = 0;
        public int YY;
        public int MM;
        public int DD;
        public int hh;
        public int mm;
        public bool bIsDay = true;

        public TimeMinuteHandler MinuteHandler;
        public TimeDayNightHandler DayNightHandler;
        public TimeDayHandler DayHandler;

    }

    public delegate void TimeMinuteHandler(XDateTime time);
    public delegate void TimeDayNightHandler(bool isDayTime);
    public delegate void TimeDayHandler(XDateTime time);

    public class XTimeSys : XSingleton<XTimeSys>, IXSys
    {
        public bool Deprecated { get; set; }

        private long BeginTime = 0;

        public bool Pause { get; set; }

        public bool IsInitDone { get; private set; }
        private float count = 0f;
        private float secondCounter = 0f;
        
        private XDateTime Current;

        public XDateTime Today { get { return Current; } }

        private XTimeData timeData = null;
        public XTimeData TimeData { get
            {
                if (null == timeData) timeData = new XTimeData();
                if (Current != null)
                {
                    XDateTime t = GetCurrent();
                    timeData.Time = t.CurrentTime;
                    timeData.IsDay = t.IsDay;
                }
                return timeData;
            } }

        public override bool Init()
        {
            XInterfaceMgr.singleton.AttachInterface<XTimeSys>(XCommon.singleton.XHash("XTimeSys"), this);
            Pause = true;

            count = 0f;
            secondCounter = 0f;

            IsInitDone = false;

            return true;
        }

        public override void Uninit()
        {
            PauseRecord();
        }

        public void BeginRecord(long dateTime)
        {
            BeginTime = dateTime;
            Current = new XDateTime(dateTime);
            Pause = false;

            IsInitDone = true;
        }
        public void ResetRecord()
        {
            Pause = false;
        }

        public void PauseRecord()
        {
            Pause = true;
        }

        public void RegisterMinuteHandler(TimeMinuteHandler handler)
        {
            if (!IsInitDone) return;
            Current.MinuteHandler -= handler;
            Current.MinuteHandler += handler;
        }

        public void RegisterDayNightHandler(TimeDayNightHandler handler)
        {
            if (!IsInitDone) return;
            Current.DayNightHandler -= handler;
            Current.DayNightHandler += handler;
        }

        public void RegisterDayChangeHandler(TimeDayHandler handler)
        {
            if (!IsInitDone) return;
            Current.DayHandler -= handler;
            Current.DayHandler += handler;
        }


        public void Update()
        {
            if (!IsInitDone) return;
            if (Pause) return;

            count += Time.deltaTime;
            secondCounter += Time.deltaTime;

            if(secondCounter >= XDateTime.SecondsPerMinute)
            {
                secondCounter = 0f;
                Current.AddMinute(1);
            }
        }

        public XDateTime GetCurrent()
        {
            if(Current != null)
            {
                Current.Reset(BeginTime + (int)count);
                return Current;
            }
            return null;
        }

    }
}
