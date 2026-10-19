using UnityEngine;

public class Flamethrower : BaseTower
{
    [SerializeField] private GameObject flameParticles;
    

    public override void Start()
    {
        base.Start();
        flameParticles.SetActive(false);
    }

    public override void Update()
    {
        HandleSelectionInput();

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
