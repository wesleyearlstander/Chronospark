using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager
{
    public YearMonth _time;
    public GameObject _go;

    public YearMonth? _taskTime;
    public YearMonth? _taskStartTime;

    public int multiplier = 1;

    public struct YearMonth // Time struct for handling month and years
    {
        public int year;
        public float month;

        public YearMonth (int y, float m)
        {
            year = y;
            month = m;
        }

        public override string ToString()
        {
            return year.ToString() + " years & " + month.ToString("0") + " months"; 
        }

        public static bool operator < (YearMonth l, YearMonth f)
        {
            return (l.year + l.month/12) < (f.year + f.month/12);
        }
        public static bool operator > (YearMonth l, YearMonth f)
        {
            return (l.year + l.month/12) > (f.year + f.month/12);
        }
        public static YearMonth operator + (YearMonth l, YearMonth f)
        {
            float addedTime = l.year * 12 + f.year * 12 + l.month + f.month;
            float addedMonths = addedTime;
            int addedYears = 0;
            if (addedTime > 11)
            {
                addedMonths = addedTime % 12;
                addedYears = Mathf.FloorToInt(addedTime / 12);
            }
            YearMonth temp = new YearMonth (addedYears, addedMonths);

            return temp;
        }

        public static YearMonth operator - (YearMonth l, YearMonth f)
        {
            float leftTime = l.year * 12 + l.month;
            float rightTime = f.year * 12 + f.month;
            float final = leftTime - rightTime;
            float months, years;
            if (final > 11)
            {
                months = final % 12;
                years = Mathf.FloorToInt(final / 12);
            } else
            {
                years = 0;
                months = final;
            }
            YearMonth temp = new YearMonth((int)years, months);
            return temp;
        }

        public static YearMonth operator / (YearMonth l, float f)
        {
            float left = l.year * 12 + l.month;
            float temp = left / f;
            float months, years;
            if (temp > 11)
            {
                months = temp % 12;
                years = Mathf.FloorToInt(temp / 12);
            }
            else
            {
                years = 0;
                months = temp;
            }
            YearMonth final = new YearMonth((int)years, months);
            return final;
        }

        public static float operator / (YearMonth l, YearMonth f)
        {
            float left = l.year * 12 + l.month;
            float right = f.year * 12 + f.month;
            return left / right;
        }
    }

    public List<TimeEvent> timeEvents;
    public int _currentTimeEvent = 0;

    public struct TimeEvent
    {
        public EventType type;
        public YearMonth time;
        public bool isPublic;

        public TimeEvent (EventType t, YearMonth ym, bool p)
        {
            type = t;
            time = ym;
            isPublic = p;
        }

        public override string ToString()
        {
            return type.ToString() + " :" + time.ToString(); 
        }
    }

    public enum EventType { Created, End, Error, AgeZoneUp, TaskCompleted }

    public float ageSpeedModifier = 1;

    private bool beingAged = false;
    const float AgingRatio = 2;
    public float agingMultiplier = 1;
    public float agingTimer = 1;

    public TimeManager(GameObject go)
    {
        _time.year = 0;
        _time.month = 0;
        _go = go;
        timeEvents = new List<TimeEvent>();
        AddTimeEvent(EventType.Created, 0, 0, true);
    }

    public TimeManager(GameObject go, YearMonth age)
    {
        _time.year = age.year;
        _time.month = age.month;
        _go = go;
        timeEvents = new List<TimeEvent>();
        AddTimeEvent(EventType.Created, 0, 0, true);
    }

    public float deltaTime = 0;

    public void AddMonth(float time)
    {
        if (beingAged)
        {
            agingTimer += time * GameManager.secondsPerMonth;
            agingMultiplier = Mathf.Pow(AgingRatio, agingTimer);
        } else
        {
            agingTimer = 1;
            agingMultiplier = 1;
        }
        deltaTime = time * ageSpeedModifier * agingMultiplier;
        float addedTime = deltaTime;
        float addedMonths = addedTime;
        int addedYears = 0;
        if (addedTime > 12)
        {
            addedMonths = addedTime % 12;
            addedYears = Mathf.FloorToInt(addedTime / 12);
        }
        _time.month += addedMonths;
        _time.year += addedYears;
        if (_time.month > 12)
        {
            _time.month -= 12;
            _time.year++;
        }
        if (_taskTime != null) {
            if (_taskTime < _time)
            {
                YearMonth duration = (_taskTime - _taskStartTime).Value;
                float dur = duration.year * 12 + duration.month;
                multiplier = Mathf.RoundToInt(deltaTime / dur);
                if (multiplier < 1) multiplier = 1;
                EventHandler.TimeEvent temp;
                temp.timeEvent = new TimeEvent (EventType.TaskCompleted, (YearMonth)_taskTime, true);
                temp.go = _go;
                _taskTime = null;
                _taskStartTime = null;
                EventHandler.instance.AddPublicEvent(temp);
            }
        }
        if (_currentTimeEvent != timeEvents.Count)
        {
            if (_time.year >= timeEvents[_currentTimeEvent].time.year || (_time.month >= timeEvents[_currentTimeEvent].time.month && _time.year == timeEvents[_currentTimeEvent].time.year))
            {
                EventTriggered();
            }
        }
    }

    public void SetBeingAged (bool b)
    {
        beingAged = b; 
    }

    void EventTriggered()
    {
        EventHandler.TimeEvent temp;
        temp.timeEvent = timeEvents[_currentTimeEvent];
        temp.go = _go; //adding gameobject for public event
        EventHandler.instance.AddPublicEvent(temp);
        _currentTimeEvent++;
    }

    public void AddTaskEvent (YearMonth duration)
    {
        _taskStartTime = _time;
        _taskTime = _time + duration;
    }

    public void AddTaskEvent (YearMonth progress, YearMonth duration)
    {
        _taskStartTime = _time - progress;
        _taskTime = _taskStartTime + duration;
    }

    public float GetTaskPercentage () //returns value from 0 - 100
    {
        if (_taskTime == null)
        {
            return 0;
        }
        YearMonth duration = (YearMonth)(_taskTime - _taskStartTime);
        YearMonth percentage = (YearMonth)(_time - _taskStartTime);
        float dur = duration.year * 12 + duration.month;
        float perc = percentage.year * 12 + percentage.month;
        return perc / dur * 100;
    }

    public void AddTimeEvent(EventType eventType, int year, float month, bool isPublic)
    {
        TimeEvent timeEvent = new TimeEvent();
        timeEvent.time.year = year;
        timeEvent.time.month = month;
        timeEvent.type = eventType;
        timeEvent.isPublic = isPublic;
        AddTimeEvent(timeEvent);
    }

    public void AddTimeEvent(TimeEvent timeEvent)
    {
            int count = _currentTimeEvent;
            bool set = false;
            if (count == timeEvents.Count || timeEvents.Count == 0) 
            {
                timeEvents.Add(timeEvent);
                set = true;
            }
            else
            {
                while (!set)
                {
                    if (count < timeEvents.Count)
                    {
                        if (timeEvents[count].time.year <= timeEvent.time.year) // event year more than or equal to holder before
                        {
                            if (timeEvents[count].time.year == timeEvent.time.year)
                            {
                                if (timeEvent.time.month <= timeEvents[count].time.month)
                                {
                                    timeEvents.Insert(count, timeEvent);
                                    set = true;
                                }
                            }
                        } else
                        {
                            timeEvents.Insert(count, timeEvent);
                            set = true;
                        }
                    }
                    else
                    {
                        timeEvents.Insert(count,timeEvent);
                        set = true;
                    }
                    count++;
                }
            }
    }
}
