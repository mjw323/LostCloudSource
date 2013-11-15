using System.Collections;

/// <summary>
/// Improved coroutine type that provides pause, resume, and stop semantics.
/// </summary>
public class Task
{
	/// <summary>
	/// Creates a new Task object for a coroutine.
	/// </summary>
	/// <param name="coroutine">
	/// Coroutine for the Task object.
	/// </param>
	/// <param name="autoStart">
	/// If autoStart is true (default) the task begins immediately upon
	/// construction.
	/// </param>
	public Task(IEnumerator coroutine, bool autoStart = true)
	{
		m_task = TaskManager.CreateTask(coroutine);
		m_task.Finished += TaskFinished;
		if (autoStart)
			Start();
	}
	
	public void Start()
	{
		m_task.Start();
	}

	public void Stop()
	{
		m_task.Stop();
	}

	public void Pause()
	{
		m_task.Pause();
	}

	public void Resume()
	{
		m_task.Resume();
	}

	public bool IsRunning
	{
		get { return m_task.IsRunning; }
	}

	public bool IsPaused
	{
		get { return m_task.IsPaused; }
	}

	/// <summary>
	/// Delegate for the subscribers to the Finished event.
	/// </summary>
	public delegate void FinishedEventHandler();

	/// <summary>
	/// Fired when a coroutine either completes execution or is manually
	/// stopped.
	/// </summary>
	public event FinishedEventHandler Finished;

	private void TaskFinished()
	{
		if (Finished != null)
			Finished();
	}

	private TaskManager.TaskState m_task;
}