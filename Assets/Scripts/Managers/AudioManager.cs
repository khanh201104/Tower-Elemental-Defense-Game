using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("--- Nguồn Phát Nhạc (Audio Sources) ---")]
    public AudioSource bgmSource; // Phát nhạc nền (Lặp lại)
    public AudioSource sfxSource; // Phát hiệu ứng ngắn (Đè nhau)

    [Header("--- Kho Nhạc Nền (BGM) ---")]
    public AudioClip mainMenuBGM;
    public AudioClip gameplayBGM;

    [Header("--- Kho Hiệu Ứng (SFX) ---")]
    public AudioClip buttonClickSFX;
    public AudioClip victorySFX;
    public AudioClip gameOverSFX;
    public AudioClip bulletHitSFX;      // Đạn tháp trúng quái
    public AudioClip enemyShootSFX;     // Quái bắn xa
    public AudioClip enemyMeleeSFX;     // Quái chém gần
    public AudioClip enemyBulletHitSFX; // Đạn quái trúng tháp

    void Awake()
    {
        // Thiết lập Singleton không bị hủy khi chuyển Scene
        if (Instance == null)
        {
            Instance = this;
            // Tải lại mức âm lượng đã lưu, mặc định là 1 (max) nếu mới chơi lần đầu
            bgmSource.volume = PlayerPrefs.GetFloat("BGMVolume", 1f);
            sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // --- HÀM PHÁT BGM ---
    public void PlayMainMenuBGM()
    {
        if (bgmSource.clip == mainMenuBGM) return; // Nếu đang phát rồi thì thôi
        bgmSource.clip = mainMenuBGM;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlayGameplayBGM()
    {
        if (bgmSource.clip == gameplayBGM) return;
        bgmSource.clip = gameplayBGM;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // --- CÁC HÀM PHÁT SFX ---
    public void PlayButtonClick() => sfxSource.PlayOneShot(buttonClickSFX);
    public void PlayVictory() => sfxSource.PlayOneShot(victorySFX);
    public void PlayGameOver() => sfxSource.PlayOneShot(gameOverSFX);
    public void PlayBulletHit() => sfxSource.PlayOneShot(bulletHitSFX);
    public void PlayEnemyShoot() => sfxSource.PlayOneShot(enemyShootSFX);
    public void PlayEnemyMelee() => sfxSource.PlayOneShot(enemyMeleeSFX);
    public void PlayEnemyBulletHit() => sfxSource.PlayOneShot(enemyBulletHitSFX);

    // --- HÀM ĐIỀU CHỈNH ÂM LƯỢNG ---
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
        PlayerPrefs.SetFloat("BGMVolume", volume); // Lưu lại cài đặt
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume); // Lưu lại cài đặt
    }
}