using UnityEngine;

public class AudioSystem : Singleton<AudioSystem>
{
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource sound;

    public bool MuteM { get; private set; }
    public bool MuteS { get; private set; }

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void SetMusic(AudioClip clip)
    {
        if (clip == null) return;

        if (clip == music.clip) return;

        music.Pause();
        music.clip = clip;
        music.Play();
    }

    public void SetSound(AudioClip clip)
    {
        if (clip == null) return;

        sound.PlayOneShot(clip);
    }

    public void MuteMusic(bool val)
    {
        MuteM = val;
        music.mute = MuteM;
    }

    public void MuteSound(bool val)
    {
        MuteS = val;
        sound.mute = MuteS;
    }

    public void SetMusicVolume(float val)
    {
        music.volume = val;
    }

    public void SetSoundVolume(float val)
    {
        sound.volume = val;
    }

    public float GetMusicVolume()
    {
        return music.volume;
    }

    public float GetSoundVolume()
    {
        return sound.volume;
    }
}
