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

    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
