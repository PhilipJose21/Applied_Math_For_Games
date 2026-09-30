using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;


public class RotateTurret : MonoBehaviour
{
    public enum RotationType
    {
        None,
        AutoRotate,
        LookAtTarget,
        LookAtTargetWithDelay
    }

    public enum TargetType
    {
        First,
        Last,
        Closest,
        Furthest,
        LowestHealth,
        HighestHealth
    }


    [SerializeField] private List<Transform> allTargetsInRadius = new List<Transform>();
    private TargetDetection targetDetection;
    [SerializeField] private RotationType rotationType = RotationType.AutoRotate;
    [SerializeField] private TargetType targetType = TargetType.First;
    [SerializeField] private float rotationSpeed = 5f;
    Transform firstTarget;

    void Start()
    {
        targetDetection = GetComponent<TargetDetection>();
    }

    void Update()
    {
        allTargetsInRadius = targetDetection.GetTargetsInRadius();
        if (allTargetsInRadius.Count > 0)
        {
            if (firstTarget == null)
            {
                firstTarget = allTargetsInRadius[0];
            }
            else if (firstTarget != allTargetsInRadius[0])
            {
                firstTarget = allTargetsInRadius[0];
            }
        }

        if (targetDetection.IsTargetDetected())
        {
            checkRotateType();
        }
    }

    public void checkRotateType()
    {
        switch(rotationType)
        {
            case RotationType.LookAtTarget:
                LookAtTarget(GetTargetByType(targetType));
                break;
            case RotationType.LookAtTargetWithDelay:
                LookAtTargetWithDelay(GetTargetByType(targetType));
                break;
            case RotationType.AutoRotate:
                AutoRotate();
                break;
            case RotationType.None:
                return;
        }
    }

    private Transform GetTargetByType(TargetType type)
    {
        Transform target = null;
        for (int i = 0; i < allTargetsInRadius.Count; i++)
        {
            Transform t = allTargetsInRadius[i];
            if (t == null) continue;

            switch (type)
            {
                case TargetType.First:
                    return firstTarget;
                case TargetType.Last:
                    return t;
                case TargetType.Closest:
                    if (target == null || Vector3.Distance(transform.position, t.position) < Vector3.Distance(transform.position, target.position))
                    {
                        target = t;
                    }
                    return target;
                case TargetType.Furthest:
                    if (target == null || Vector3.Distance(transform.position, t.position) > Vector3.Distance(transform.position, target.position))
                    {
                        target = t;
                    }
                    return target;
            }
        }
        
        return target;
    } 

    void AutoRotate()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    void LookAtTargetWithDelay(Transform target)
    {
        if (target == null) return;//if there is no target, return

        //calculate the direction to the target and the angle to rotate towards it
        var dir = target.position - transform.position;
        var angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        var speed = rotationSpeed * Time.deltaTime;
       
        //rotates turret
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(0, angle, 0),speed
        );
    }

    void LookAtTarget(Transform target)
    {
        
        Debug.Log(target);
        if (target == null) return;//if there is no target, return

        //calculate the direction to the target and the angle to rotate towards it
        var dir = target.position - transform.position;
        var angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        //rotates turret
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
        Quaternion.Euler(0, angle, 0), 
        rotationSpeed * Time.deltaTime);
    }
}
