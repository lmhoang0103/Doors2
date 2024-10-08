using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using DG.Tweening;
using Sigtrap.Relays;
using Sirenix.OdinInspector;

[RequireComponent(typeof(NavMeshAgent))]
[Serializable]
public class HCNavMeshAgent : HCMonoBehaviour
{
    public NavMeshAgent agent;
    
    [SerializeField] private bool disableAgentWhenIdle;
    [SerializeField] private bool movingTarget;
    
    public Relay OnDestinationReach = new Relay();

    public float Speed => Vector3.Magnitude(agent.velocity);
    
    private Transform _currentTargetTrans;
    private float _initStoppingDistance;
    
    private void Awake()
    {
        _initStoppingDistance = agent.stoppingDistance;
    }

    private void Reset()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void OnDestroy()
    {
        this.DOKill();
    }

    void Update()
    {
        if(!agent.enabled)
            return;

        if (_currentTargetTrans && movingTarget)
        {
            agent.destination = _currentTargetTrans.position;
        }
        
        if (agent.IsDestinationReached())
        {
            OnDestinationReached();
        }
    }

    [Button]
    public void SetDestination(Transform target)
    {
        SetAgentEnable(true);
        _currentTargetTrans = target ? target : transform;
        agent.destination = target.position;
    }

    private void OnDestinationReached()
    {
        if (_currentTargetTrans)
        {
            _currentTargetTrans = null;
            
            if (disableAgentWhenIdle)
                SetAgentEnable(false);
            
            OnDestinationReach.Dispatch();
        }
    }

    public void SetAgentEnable(bool enable)
    {
        agent.enabled = enable;
    }
    
    public void GoTo(Transform target, Action onDestinationReach = null, bool snapDestination = false)
    {
        SetDestination(target);
        OnDestinationReach.RemoveAll();
        
        agent.stoppingDistance = _initStoppingDistance;
        if (snapDestination)
        {
            agent.stoppingDistance = .1f;
            OnDestinationReach.AddOnce(() => agent.stoppingDistance = _initStoppingDistance);
        }
        
        OnDestinationReach.AddOnce(onDestinationReach);
    }
}

public static class NavMeshHelper
{
    public static bool IsDestinationReached(this NavMeshAgent agent)
    {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }
}