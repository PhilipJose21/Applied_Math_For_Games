using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager _instance;

    [Header("Audio Sources: ")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips: ")]
    public AudioClip coinPickupClip;
    public AudioClip towerPlaceClip;
    public AudioClip towerGunfireClip;
    public AudioClip flameThrowerStartClip;
    public AudioClip flameThrowerLoopClip;
    public AudioClip backgroundMusicClip;
    GameManager gameManager;
    EnemyManager enemyManager;
    
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        gameManager = GameManager._instance;
        enemyManager = EnemyManager._instance;

        musicSource.clip = backgroundMusicClip;
        musicSource.Play();

        // HealthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        // HealthSystem.OnDead += HealthSystem_OnDead;
        // GameManager.OnCoinChanged += GameManager_OnCoinChanged;
        // GameManager.OnSelectedTowerChanged += GameManager_OnSelectedTowerChanged;
        // GameManager.OnTowerUpgraded += GameManager_OnTowerUpgraded;
        // EnemyManager.OnWaveEnded += EnemyManager_OnWaveEnded;
    }

    // private void EnemyManager_OnWaveEnded(object sender, EventArgs e)
    // {
    //     throw new NotImplementedException();
    // }

    // private void GameManager_OnTowerUpgraded(object sender, EventArgs e)
    // {
    //     throw new NotImplementedException();
    // }

    // private void GameManager_OnSelectedTowerChanged(object sender, EventArgs e)
    // {
    //     throw new NotImplementedException();
    // }

    // private void GameManager_OnCoinChanged(object sender, EventArgs e)
    // {
    //     throw new NotImplementedException();
    // }

    // private void HealthSystem_OnDead(object sender, EventArgs e)
    // {
    //     throw new NotImplementedException();
    // }

    // private void HealthSystem_OnHealthChanged(object sender, EventArgs e)
    // {
    //     throw new NotImplementedException();
    // }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
