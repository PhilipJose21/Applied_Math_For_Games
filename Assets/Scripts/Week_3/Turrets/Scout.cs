using UnityEngine;

public class Scout : BaseTower
{
    public override void Upgrade(int level)
    {
        
        switch (level)
        {
            case 0:
                break;
            case 1:
                fireRate += upgrades[level].fireRateUPG.value;
                break;
            case 2:
                damage += upgrades[level].damageUPG.value;
                fireRate += upgrades[level].fireRateUPG.value;
                break;
            case 3:
                damage += upgrades[level].damageUPG.value;
                targetDetection.setDetectionRange(upgrades[level].rangeUPG.value + targetDetection.GetDetectionRange());
                hasHiddenDetection = upgrades[level].hiddenDetectionUPG;
                break;
            case 4:
                damage += upgrades[level].damageUPG.value;
                fireRate += upgrades[level].fireRateUPG.value;
                break;
        }
        UpdateSellValue();
    }
}
