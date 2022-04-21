using System;
using System.Collections.Generic;
using System.Diagnostics;
using Rilisoft;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class EveryplayWrapper
{
	public enum State
	{
		Initial,
		Recording,
		Paused,
		Idle
	}

	private State _currenState;

	private static EveryplayWrapper _instance;

	private Stopwatch _stopwatch = new Stopwatch();

	public TimeSpan Elapsed
	{
		get
		{
			return _stopwatch.Elapsed;
		}
	}

	public long ElapsedMilliseconds
	{
		get
		{
			return _stopwatch.ElapsedMilliseconds;
		}
	}

	public State CurrentState
	{
		get
		{
			return _currenState;
		}
	}

	public static EveryplayWrapper Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new EveryplayWrapper();
			}
			return _instance;
		}
	}

	public bool IsSupported()
	{
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
		{
			return false;
		}
		return Everyplay.IsSupported();
	}

	public bool IsRecordingSupported()
	{
		if (!IsSupported())
		{
			return false;
		}
		return Everyplay.IsRecordingSupported();
	}

	public bool IsPaused()
	{
		if (!IsSupported())
		{
			return false;
		}
		return Everyplay.IsPaused();
	}

	public bool IsRecording()
	{
		if (!IsSupported())
		{
			return false;
		}
		return Everyplay.IsRecording();
	}

	public void Pause()
	{
		if (IsSupported())
		{
			CheckCommand("Pause", State.Recording);
			Everyplay.PauseRecording();
			_currenState = State.Paused;
			_stopwatch.Stop();
		}
	}

	public void Record()
	{
		if (IsSupported())
		{
			CheckCommand("Record", State.Initial, State.Idle);
			Everyplay.StartRecording();
			_currenState = State.Recording;
			_stopwatch.Reset();
			_stopwatch.Start();
		}
	}

	public void Resume()
	{
		if (IsSupported())
		{
			CheckCommand("Resume", State.Paused);
			Everyplay.ResumeRecording();
			_currenState = State.Recording;
			_stopwatch.Start();
		}
	}

	public void Share()
	{
		if (IsSupported())
		{
			CheckCommand("Share", State.Idle);
			Everyplay.ShowSharingModal();
		}
	}

	public void Stop()
	{
		if (IsSupported())
		{
			CheckCommand("Stop", State.Recording, State.Paused);
			string text = SceneManager.GetActiveScene().name ?? string.Empty;
			UnityEngine.Debug.LogFormat("Trying to add metadata to shared video.    Map: '{0}'", text);
			Everyplay.SetMetadata("map", text);
			Everyplay.StopRecording();
			_currenState = State.Idle;
			_stopwatch.Stop();
		}
	}

	public void SetMetadata(Dictionary<string, object> dict)
	{
		if (IsSupported())
		{
			Everyplay.SetMetadata(dict);
		}
	}

	public bool CheckState()
	{
		bool result = true;
		switch (CurrentState)
		{
		case State.Initial:
			if (IsRecording())
			{
				string message8 = string.Format("Everyplay.IsRecording() in {0} state.", CurrentState);
				UnityEngine.Debug.LogError(message8);
				result = false;
			}
			if (_stopwatch.IsRunning)
			{
				string message9 = string.Format("Stopwatch.IsRunning in {0} state.", CurrentState);
				UnityEngine.Debug.LogError(message9);
				result = false;
			}
			break;
		case State.Recording:
			if (!IsRecording())
			{
				string message3 = string.Format("!Everyplay.IsRecording() in {0} state.", CurrentState);
				UnityEngine.Debug.LogError(message3);
				result = false;
			}
			if (IsPaused())
			{
				string message4 = string.Format("Everyplay.IsPaused() in {0} state.", CurrentState);
				UnityEngine.Debug.LogError(message4);
				result = false;
			}
			if (!_stopwatch.IsRunning)
			{
				string message5 = string.Format("!Stopwatch.IsRunning in {0} state.", CurrentState);
				UnityEngine.Debug.LogError(message5);
				result = false;
			}
			break;
		case State.Paused:
			if (!IsPaused())
			{
				string message6 = string.Format("!Everyplay.IsPaused() in {0} state.", CurrentState);
				UnityEngine.Debug.LogError(message6);
				result = false;
			}
			if (_stopwatch.IsRunning)
			{
				string message7 = string.Format("Stopwatch.IsRunning in {0} state.", CurrentState);
				UnityEngine.Debug.LogError(message7);
				result = false;
			}
			break;
		case State.Idle:
			if (IsRecording())
			{
				string message = string.Format("Everyplay.IsRecording() in {0} state.", CurrentState);
				UnityEngine.Debug.LogError(message);
				result = false;
			}
			if (_stopwatch.IsRunning)
			{
				string message2 = string.Format("Stopwatch.IsRunning in {0} state.", CurrentState);
				UnityEngine.Debug.LogError(message2);
				result = false;
			}
			break;
		}
		return result;
	}

	private void CheckCommand(string command, params State[] expectedStates)
	{
		if (Array.FindIndex(expectedStates, (State s) => s == CurrentState) == -1)
		{
			string message = string.Format("{0} command in invalid state {1}.", command, CurrentState);
			UnityEngine.Debug.LogError(message);
		}
		else
		{
			string message2 = string.Format("{0} command in valid state {1}.", command, CurrentState);
			UnityEngine.Debug.Log(message2);
		}
	}
}
