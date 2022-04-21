using UnityEngine;

public class AudioSourseOnOff : MonoBehaviour
{
	private AudioSource myAudioSourse;

	private void Awake()
	{
		myAudioSourse = GetComponent<AudioSource>();
		if (myAudioSourse != null)
		{
			myAudioSourse.enabled = Defs.isSoundFX;
		}
	}

	private void Update()
	{
		if (myAudioSourse != null && myAudioSourse.enabled != Defs.isSoundFX)
		{
			myAudioSourse.enabled = Defs.isSoundFX;
		}
	}
}
