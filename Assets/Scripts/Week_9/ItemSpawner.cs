using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemSpawner : MonoBehaviour
{
    public Item itemToSpawn;
    public float speed = 300f;
    public float spawnInterval = 1f;
    public float spawnZ = 1000f;
    public float despawnZ = -120f;
    public float groundY = -3f;
    public List<Transform> spawnPoints = new List<Transform>();
    public List<Item> items = new List<Item>();
    public Transform parent;
    public float maxHorizontal;
    public float minHorizontal;
    public float maxVertical;
    public float minVertical;

    private void Start()
    {
        InvokeRepeating(nameof(BeginSpawn), 1, spawnInterval);
    }

    private void BeginSpawn()
    {
        var spawnedItem = Instantiate(itemToSpawn, parent);
        Vector3 pos = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        pos.y = groundY;
        pos.z = spawnZ;
        spawnedItem.itemPosition = pos;
        spawnedItem.hasHit = false;
        spawnedItem.UpdateVisual();
        items.Add(spawnedItem);
    }

    private Vector3 GetRandomLocation()
    {
        var xRand = Random.Range(minHorizontal, maxHorizontal);
        var yRand = Random.Range(minVertical, maxVertical);
        return new Vector3(xRand, yRand, 0);
    }

    private void Update()
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            Item item = items[i];
            if (item == null)
            {
                items.RemoveAt(i);
                continue;
            }

            ItemMover(item);

            if (item.itemPosition.z < despawnZ)
            {
                Destroy(item.gameObject);
                items.RemoveAt(i);
            }
        }

        var sorted = items.OrderByDescending(x => x.itemPosition.z).ToList();
        for (int i = 0; i < sorted.Count; i++)
        {
            sorted[i].transform.SetSiblingIndex(i);
        }
    }

    private void ItemMover(Item item)
    {
        item.itemPosition.z -= speed * Time.deltaTime;
    }
}