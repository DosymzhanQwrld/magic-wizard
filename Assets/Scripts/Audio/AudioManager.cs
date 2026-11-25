using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// One audio object for the whole game. It is created before the first scene loads and
// survives scene loads (DontDestroyOnLoad), so music is not restarted on every level reload.
// It owns the mixer: volume in dB, the Paused snapshot, SFX voices and the MusicManager.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    const float SilentDb = -80f;
    const int SfxVoices = 12;
    const float SnapshotTime = 0.25f;
    const string MenuScene = "MainMenu";

    AudioLibrary library;
    AudioMixerSnapshot normalSnapshot;
    AudioMixerSnapshot pausedSnapshot;
    AudioSource[] voices;
    int nextVoice;
    MusicManager music;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Create()
    {
        if (Instance != null) return;
        AudioLibrary lib = Resources.Load<AudioLibrary>("AudioLibrary");
        if (lib == null || lib.mixer == null)
        {
            Debug.LogWarning("AudioManager: Resources/AudioLibrary is missing. Run Tools > Audio > Setup Audio Library.");
            return;
        }

        GameObject go = new GameObject("AudioManager");
        DontDestroyOnLoad(go);
        Instance = go.AddComponent<AudioManager>();
        Instance.Init(lib);
    }

    void Init(AudioLibrary lib)
    {
        library = lib;
        normalSnapshot = lib.mixer.FindSnapshot("Normal");
        pausedSnapshot = lib.mixer.FindSnapshot("Paused");

        AudioMixerGroup sfxGroup = FindGroup("SFX");
        voices = new AudioSource[SfxVoices];
        for (int i = 0; i < voices.Length; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.outputAudioMixerGroup = sfxGroup;
            voices[i] = source;
        }

        music = gameObject.AddComponent<MusicManager>();
        music.Init(FindGroup("MusicTrack"));

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // AudioMixer.SetFloat does not work in Awake, so volumes are applied here and on every scene load.
        ApplyVolumes();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyVolumes();
        SetPaused(false);
        AddClickSounds();
        music.Play(scene.name == MenuScene ? library.menuMusic : library.levelMusic);
    }

    // ---------- volume (slider 0..1 -> decibels) ----------

    public static float ToDecibels(float volume)
    {
        // The ear hears volume on a log scale: half the slider must be -6 dB, not half the amplitude.
        return volume <= 0.0001f ? SilentDb : Mathf.Log10(volume) * 20f;
    }

    public static void ApplyVolumes()
    {
        if (Instance == null) return;
        SaveData data = SaveSystem.Data;
        Instance.library.mixer.SetFloat("MusicVol", ToDecibels(data.musicVolume));
        Instance.library.mixer.SetFloat("SfxVol", ToDecibels(data.sfxVolume));
    }

    // ---------- pause (mixer snapshot) ----------

    public static void SetPaused(bool paused)
    {
        if (Instance == null) return;
        AudioMixerSnapshot target = paused ? Instance.pausedSnapshot : Instance.normalSnapshot;
        if (target != null) target.TransitionTo(SnapshotTime);
    }

    // ---------- sound effects ----------

    public void PlaySfx(Sound sound, float pitch, float delay)
    {
        AudioClip[] clips = library.Get(sound);
        if (clips == null || clips.Length == 0) return;
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        if (clip == null) return;

        AudioSource source = voices[nextVoice];
        nextVoice = (nextVoice + 1) % voices.Length;

        // Jingles keep their exact notes; everything else gets a small random pitch so repeats do not sound robotic.
        bool vary = sound != Sound.Win && sound != Sound.Lose;
        source.pitch = pitch * (vary ? Random.Range(0.92f, 1.08f) : 1f);
        source.clip = clip;
        if (delay > 0f) source.PlayDelayed(delay);
        else source.Play();
    }

    void AddClickSounds()
    {
        // Menus are built by editor scripts, so instead of editing every button we add the click sound at runtime.
        foreach (Button button in FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            button.onClick.AddListener(PlayClick);
    }

    static void PlayClick()
    {
        Sfx.Play(Sound.UiClick);
    }

    AudioMixerGroup FindGroup(string groupName)
    {
        foreach (AudioMixerGroup group in library.mixer.FindMatchingGroups(groupName))
            if (group.name == groupName) return group;
        Debug.LogWarning("AudioManager: mixer group not found: " + groupName);
        return null;
    }
}
