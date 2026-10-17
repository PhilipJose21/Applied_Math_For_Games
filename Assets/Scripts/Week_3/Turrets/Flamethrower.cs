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
        if (targetDetection.IsTargetDetected())
        {
            flameParticles.SetActive(true);
            flameParticles.GetComponent<AreaDamage>().SetDamage(damage);
            flameParticles.GetComponent<AreaDamage>().SetDelayedDamageTime(fireRate / 100f);
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
