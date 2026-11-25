using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

// Two AudioSources: one fades out while the other fades in (crossfade, no click).
// If the requested track is already playing, nothing happens, so a level reload does not restart the music.
public class MusicManager : MonoBehaviour
{
    [SerializeField] float fadeTime = 1f;

    AudioSource current;
    AudioSource previous;
    Coroutine fade;

    public void Init(AudioMixerGroup group)
    {
        current = CreateSource(group);
        previous = CreateSource(group);
    }

    AudioSource CreateSource(AudioMixerGroup group)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.loop = true;
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.volume = 0f;
        source.outputAudioMixerGroup = group;
        return source;
    }

    public void Play(AudioClip clip)
    {
        if (clip == null) return;
        if (current.clip == clip && current.isPlaying) return;

        AudioSource old = current;
        current = previous;
        previous = old;

        current.clip = clip;
        current.volume = 0f;
        current.Play();

        if (fade != null) StopCoroutine(fade);
        fade = StartCoroutine(Crossfade(current, previous));
    }

    IEnumerator Crossfade(AudioSource fadeIn, AudioSource fadeOut)
    {
        float startIn = fadeIn.volume;
        float startOut = fadeOut.volume;
        // Unscaled time: the fade still runs when Time.timeScale is 0 (pause, victory screen).
        for (float t = 0f; t < fadeTime; t += Time.unscaledDeltaTime)
        {
            float k = t / fadeTime;
            // Equal-power curve: the total loudness does not dip in the middle of the fade.
            fadeIn.volume = Mathf.Lerp(startIn, 1f, Mathf.Sin(k * Mathf.PI * 0.5f));
            fadeOut.volume = startOut * Mathf.Cos(k * Mathf.PI * 0.5f);
            yield return null;
        }
        fadeIn.volume = 1f;
        fadeOut.volume = 0f;
        fadeOut.Stop();
        fade = null;
    }
}
