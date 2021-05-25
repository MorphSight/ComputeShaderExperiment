using System;
using UnityEngine;

public class Timer
{
    public DateTime Start { get; private set; }
    public DateTime End { get; private set; }
    public string Name;
    
    public Timer(string name = "Timer")
    {
        Name = name;
    }
    
    public void StartTime()
    {
        Start = DateTime.Now;
    }

    public void EndTime()
    {
        End = DateTime.Now;
    }

    public TimeSpan GetInterval()
    {
        return End.Subtract(Start);
    }

    public double GetIntervalMilliseconds()
    {
        return GetInterval().TotalMilliseconds;
    }

    public void LogUnity()
    {
        Debug.Log(Name + " completed in " + GetIntervalMilliseconds() + " milliseconds.");
    }
}