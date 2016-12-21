# **Timer System**

If you were a **Flash** developer or **Unreal** developer, you must knew both them provide timer system. In **Flash**, there is a class named **Timer** to implement timer system. In **Unreal**, there is a class named **FTimerManager** to do that also. But in **Unity**, there is nothing to satisify timer requirement. So here we are, I desgin a timer system for **Unity**.

## **Simple Example**

### Limited Repeat Count

```c#
ITimer timer = new Timer(1.0f, 3);

// Timer event that when timer reach delay time each time.
timer.AddEventListener<TimerEvent>(TimerEvent.Timer, (timerEvent) => 
{
  // Your logic codes.
});

// Timer complete event that when repeat count reach to the end.
timer.AddEventListener<TimerEvent>(TimerEvent.TimerComplete, (timerEvent) => 
{
  // Your logic codes.
});

// Add timer object into timer manager.
TimerManager.instance.Add(timer);
```



### Infinite Repeat Count

```c#
// Just set repeatCount to 0 to implement infinite repeat count.
ITimer timer = new Timer(1.0f, 0);

// Timer event that when timer reach delay time each time.
timer.AddEventListener<TimerEvent>(TimerEvent.Timer, (timerEvent) => 
{
  // Your logic codes.
});

// Timer complete event that when repeat count reach to the end.
timer.AddEventListener<TimerEvent>(TimerEvent.TimerComplete, (timerEvent) => 
{
  // Your logic codes.
});

// Add timer object into timer manager.
TimerManager.instance.Add(timer);
```



## **Scaled Timer**

By default, the timer frequency will not change with **Time.timeScale**. Just because the constructor function of class **Timer** receive the default value of parameter **ignoreTimeScale** is true. If you want to get a scaled timer, just do like this:

```c#
// Set parameter 'ignoreTimeScale' to false to implement scaled timer.
ITimer timer = new Timer(1.0f, 3, false);

// Add timer object into timer manager.
TimerManager.instance.Add(timer);
```



## **Pause When Timer Disabled**

By default, the timer will stop when it be disabled (**TimerManager.instance.enabled = false**). Because the constructor function of class **Timer** receive the default value of parameter **stopOnDisable** is true. if you want to timer pause when it is disabled, pass false when you create a **Timer** object.

```c#
// Set parameter 'stopOnDisable' to false to implement that.
ITimer timer = new Timer(1.0f, 3, true, false);

// Add timer object into timer manager.
TimerManager.instance.Add(timer);
```


## **TimerGroup**

**TimerGroup** is a feature that allow you to gather some timers into a group. For example, skill cooldown timers. To use **TimerGroup** is simple, example usage:

```c#
// Skill A CD Timer.
ITimer skillACDTimer = new Timer(1, 3, true, true, false);

// Skill B CD Timer.
ITimer skillBCDTimer = new Timer(1, 5, true, true, false);

// Initialize skill CDs timer group.
ITimerGroup skillCDsTimerGroup = new TimerGroup("SkillCDTimers", true, skillACDTimer, skillBCDTimer);

// Add timer group object into timer manager.
TimerManager.instance.AddTimerGroup(skillCDsTimerGroup);
```