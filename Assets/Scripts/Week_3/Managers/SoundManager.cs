using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Clips: ")]
    [SerializeField] private AudioClip coinPickupClip;
    [SerializeField] private AudioClip towerPlaceClip;
    [SerializeField] private AudioClip towerGunfireClip;
    [SerializeField] private AudioClip flameThrowerStartClip;
    [SerializeField] private AudioClip flameThrowerLoopClip;
    [SerializeField] private AudioClip backgroundMusicClip;
    GameManager gameManager;
    EnemyManager enemyManager;
    
    void Start()
    {
        gameManager = GameManager._instance;
        enemyManager = EnemyManager._instance;

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
}
