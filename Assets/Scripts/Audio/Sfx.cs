// Short way to play a sound from any script: Sfx.Play(Sound.Jump);
public static class Sfx
{
    public static void Play(Sound sound, float pitch = 1f, float delay = 0f)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(sound, pitch, delay);
    }
}
