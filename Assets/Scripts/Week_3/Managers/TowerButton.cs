using TMPro;
using UnityEngine;

public class TowerButton : MonoBehaviour
{
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private TextMeshProUGUI buttonNameText;
    [SerializeField] private TextMeshProUGUI costText;

    void Start()
    {
        buttonNameText.text = towerPrefab.name;
        costText.text = towerPrefab.GetComponent<BaseTower>().GetCost().ToString();
    }

    public void OnClick()
    {
        PlacementManager._instance.SetSelectedTower(towerPrefab);
    }
}
