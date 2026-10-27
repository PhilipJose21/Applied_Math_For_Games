using UnityEngine;
using System.Collections.Generic;
[DefaultExecutionOrder(-100)]
public class CollisionManager : MonoBehaviour
{
    public static CollisionManager Instance => instance;
    public static CollisionManager instance;
    private List<AABBCollider> ColliderList = new List<AABBCollider>();
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Register(AABBCollider collider)
    {
        if (ColliderList.Contains(collider)) return;
        ColliderList.Add(collider);
    }

    public void Unregister(AABBCollider collider)
    {
        if (!ColliderList.Contains(collider)) return;
        ColliderList.Remove(collider);
    }

    private void FixedUpdate()
    {
       for(int i = 0; i < ColliderList.Count; i++)
       {
            var colliderA = ColliderList[i];
            for(int j = i + 1; j < ColliderList.Count; j++)
            {
                if (i == j) continue;
                var colliderB = ColliderList[j];
                if (Overlaps(colliderA, colliderB))
                {
                    colliderA.OnCollision(colliderB);
                    colliderB.OnCollision(colliderA);
                }
            }
       }
    }

    bool Overlaps(AABBCollider a, AABBCollider b)
    {
        return a.min.x <= b.max.x && a.max.x >= b.min.x && a.min.y <= b.max.y && a.max.y >= b.min.y;
    }

    public AABBCollider GetOverlap(AABBCollider self, Vector2 center)
    {
        Vector2 half = self.HalfSize;
        Vector2 min = center - half;
        Vector2 max = center + half;

        foreach (var other in ColliderList)
        {
            if (other == self) continue;
            // strict < and > so merely touching doesn't count as overlapping
            if (min.x < other.max.x && max.x > other.min.x &&
                min.y < other.max.y && max.y > other.min.y)
                return other;
        }
        return null;
    }
}
