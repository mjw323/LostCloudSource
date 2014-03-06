using System.Collections;

/// <summary>
/// Coroutine wrapper that provides pause, resume, delayed start, and stop
/// semantics.
/// </summary>
/// <remarks>
/// This class relies on the existence of a single instance of the TaskManager
/// behaviour and will create one automatically the first time that a Task is
/// instantiated.
/// </remarks>
public class Task
{
	/// <summary>
	/// Creates a new Task object.
	/// </summary>
	/// <param name="coroutine">
	/// Coroutine to be executed by the Task.
	/// </param>
	/// <param name="autoStart">
	/// If autoStart is true (default) the task begins executing after
	/// construction.
	/// </param>
	public Task(IEnumerator coroutine, bool autoStart = true)
	{
		task = TaskManager.CreateTask(coroutine);
		task.Finished += TaskFinished;
		if (autoStart)
			Start();
	}
	
	public void Start()
	{
		task.Start();
	}

	public void Stop()
	{
		task.Stop();
	}

	public void Pause()
	{
		task.Pause();
	}

	public void Resume()
	{
		task.Resume();
	}

	public bool IsRunning
	{
		get { return task.IsRunning; }
	}

	public bool IsPaused
	{
		get { return task.IsPaused; }
	}

	/// <summary>
	/// Delegate for responding to the end of a task's execution.
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

	private TaskManager.TaskState task;
}