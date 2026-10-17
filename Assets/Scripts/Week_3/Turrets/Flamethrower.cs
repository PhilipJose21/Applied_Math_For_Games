using UnityEngine;

public class Flamethrower : BaseTower
{
    [SerializeField] private GameObject flameParticles;
    

    public override void Start()
    {
        targetDetection = GetComponent<TargetDetection>();
        flameParticles.SetActive(false);
    }

    public override void Update()
    {
        float fireRateMultiplier = 5f;
        if (targetDetection.IsTargetDetected())
        {
            flameParticles.SetActive(true);
            flameParticles.GetComponent<AreaDamage>().SetDamage(damage);
            flameParticles.GetComponent<AreaDamage>().SetDelayedDamageTime(fireRate / fireRateMultiplier);
        }
        else
        {
            flameParticles.SetActive(false);
        }
    }

    public void Upgrade(int level)
    {
        switch (level)
        {
            
        }
    }
}
