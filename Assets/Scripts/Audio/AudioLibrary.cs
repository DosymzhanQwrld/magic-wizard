using UnityEngine;
using UnityEngine.Audio;

// All clips and the mixer in one asset (Assets/Resources/AudioLibrary.asset).
// An array with 2-3 clips gives random variation: AudioManager picks one each time.
[CreateAssetMenu(fileName = "AudioLibrary", menuName = "Magic Wizard/Audio Library")]
public class AudioLibrary : ScriptableObject
{
    public AudioMixer mixer;

    [Header("Music")]
    public AudioClip menuMusic;
    public AudioClip levelMusic;

    [Header("Sound effects")]
    public AudioClip[] jump;
    public AudioClip[] fireball;
    public AudioClip[] arrow;
    public AudioClip[] hitPlayer;
    public AudioClip[] hitEnemy;
    public AudioClip[] pickup;
    public AudioClip[] windupGoblin;
    public AudioClip[] windupEye;
    public AudioClip[] windupArcher;
    public AudioClip[] windupBoss;
    public AudioClip[] deathPlayer;
    public AudioClip[] deathEnemy;
    public AudioClip[] uiClick;
    public AudioClip[] win;
    public AudioClip[] lose;

    public AudioClip[] Get(Sound sound)
    {
        switch (sound)
        {
            case Sound.Jump: return jump;
            case Sound.Fireball: return fireball;
            case Sound.Arrow: return arrow;
            case Sound.HitPlayer: return hitPlayer;
            case Sound.HitEnemy: return hitEnemy;
            case Sound.Pickup: return pickup;
            case Sound.WindupGoblin: return windupGoblin;
            case Sound.WindupEye: return windupEye;
            case Sound.WindupArcher: return windupArcher;
            case Sound.WindupBoss: return windupBoss;
            case Sound.DeathPlayer: return deathPlayer;
            case Sound.DeathEnemy: return deathEnemy;
            case Sound.UiClick: return uiClick;
            case Sound.Win: return win;
            case Sound.Lose: return lose;
            default: return null;
        }
    }
}
