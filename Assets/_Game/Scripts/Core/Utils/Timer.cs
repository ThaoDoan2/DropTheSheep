using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A timer
/// </summary>
public class Timer : MonoBehaviour
{
    #region Fields

    // timer duration
    float totalSeconds = 0;

    // timer execution
    float elapsedSeconds = 0;
    bool running = false;

    // support for Finished property
    bool started = false;

    // event invoked by class
    TimerFinishedEvent _timerFinishedEvent = new TimerFinishedEvent();
    UnityAction miniAction = null;
    float miniSeconds = 0;
    float miniElapsedSeconds = 0;

    #endregion

    #region Properties

    /// <summary>
    /// Sets the duration of the timer
    /// The duration can only be set if the timer isn't currently running
    /// </summary>
    /// <value>duration</value>
    public float Duration
    {
        set
        {
            if (!running)
            {
                totalSeconds = value;
            }
        }
        get { return totalSeconds; }
    }

    /// <summary>
    /// Gets whether or not the timer has finished running
    /// This property returns false if the timer has never been started
    /// </summary>
    /// <value>true if finished; otherwise, false.</value>
    public bool Finished
    {
        get { return started && !running; }
    }

    /// <summary>
    /// Gets whether or not the timer is currently running
    /// </summary>
    /// <value>true if running; otherwise, false.</value>
    public bool Running
    {
        get { return running; }
    }

    /// <summary>
    /// Gets how many seconds are left on the timer
    /// </summary>
    /// <value>seconds left</value>
    public float SecondsLeft
    {
        get
        {
            return totalSeconds - elapsedSeconds;
        }
    }

    public bool UnscaledTime { get; set; } = false;

    #endregion

    #region Methods

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // update timer and check for finished
        if (running)
        {
            float dt = UnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            elapsedSeconds += dt;
            if (elapsedSeconds >= totalSeconds)
            {
                running = false;
                _timerFinishedEvent.Invoke();
            }
            if (miniSeconds > 0)
            {
                miniElapsedSeconds += dt;
                if (miniElapsedSeconds >= miniSeconds)
                {
                    miniElapsedSeconds = 0;
                    miniAction?.Invoke();
                }
            }
        }
    }

    /// <summary>
    /// Runs the timer
    /// Because a timer of 0 duration doesn't really make sense,
    /// the timer only runs if the total seconds is larger than 0
    /// This also makes sure the consumer of the class has actually 
    /// set the duration to something higher than 0
    /// </summary>
    public void Run()
    {
        // only run with valid duration
        if (totalSeconds > 0)
        {
            started = true;
            running = true;
            elapsedSeconds = 0;
            miniElapsedSeconds = 0;
        }
    }

    /// <summary>
    /// Stops the timer
    /// </summary>
    public void Stop()
    {
        started = false;
        running = false;
    }

    public void Pause()
    {
        if (running)
            running = false;
    }

    public void Resume()
    {
        if (started)
            running = true;
    }

    /// <summary>
    /// Adds the given number of seconds to the timer
    /// </summary>
    /// <param name="seconds">time to add</param>
    public void AddTime(float seconds)
    {
        totalSeconds += seconds;
    }

    /// <summary>
    /// Adds the given listener to the timer finished event
    /// </summary>
    /// <param name="listener">listener</param>
    public void AddTimerFinishedListener(UnityAction listener)
    {
        _timerFinishedEvent.AddListener(listener);
    }

    /// <summary>
    /// Remove all listener
    /// </summary>
    public void ClearFinishedListener()
    {
        _timerFinishedEvent.RemoveAllListeners();
    }

    public void SetMiniAction(UnityAction miniAction, float miniSeconds)
    {
        this.miniAction = miniAction;
        this.miniSeconds = miniSeconds;
    }

    #endregion
}
