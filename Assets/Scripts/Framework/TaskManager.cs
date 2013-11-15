using UnityEngine;
using System.Collections;

public class TaskManager : MonoBehaviour
{
	public class TaskState
	{
		public TaskState(IEnumerator coroutine)
		{
			m_coroutine = coroutine;
		}

		public void Start()
		{
			m_isRunning = true;
			s_instance.StartCoroutine(CoroutineWrapper());
		}

		public void Stop()
		{
			m_isRunning = false;
		}

		public void Pause()
		{
			m_isPaused = true;
		}

		public void Resume()
		{
			m_isPaused = false;
		}

		public bool IsRunning
		{
			get { return m_isRunning; }
		}

		public bool IsPaused
		{
			get { return m_isPaused; }
		}

		public delegate void FinishedEventHandler();
		public event FinishedEventHandler Finished;

		private IEnumerator CoroutineWrapper()
		{
			yield return null;
			while (m_isRunning)
			{
				if (m_isPaused)
					yield return null;
				else
				{
					if (m_coroutine != null && m_coroutine.MoveNext())
						yield return m_coroutine.Current;
					else
						m_isRunning = false;
				}
			}

			if (Finished != null)
				Finished();
		}

		private IEnumerator m_coroutine;
		private bool m_isRunning;
		private bool m_isPaused;
	}

	private static TaskManager s_instance;

	public static TaskState CreateTask(IEnumerator coroutine)
	{
		if (s_instance == null)
		{
			GameObject go = new GameObject("TaskManager");
			s_instance = go.AddComponent<TaskManager>();
		}
		return new TaskState(coroutine);
	}
}