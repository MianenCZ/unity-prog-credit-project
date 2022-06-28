using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class EnemyColectorAI : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Camera _camera;

    private const string _animIDIsWalking = "IsWalking";
    private const string _animIDJump = "Jump";
    private const string _animIDIsAttacking = "IsAttacking";
    private const string _animIDDie = "Die";
    private const string _animIDTakeDamage = "TakeDamage";
    private Animator _animator;
    private SpawnDropScript _spawnDropScript;

    public DestructionTargetScript activeTarget;
    public string TargetName;
    public Func<DestructionTargetScript> ObtainTarget;
    public Action<DestructionTargetScript> OnTargetDestroyed;
    private bool destructionActive = false;


    [Header("Healt points")]
    public float MaxHealthPonts = 100;
    public float HealthPonts = 100;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _camera = Camera.main;
        _animator = GetComponent<Animator>();
        _animator.SetBool(_animIDDie, false);
        _spawnDropScript = GetComponent<SpawnDropScript>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {
        _animator.SetBool(_animIDIsAttacking, destructionActive);
        _animator.SetBool(_animIDIsWalking, !destructionActive && _agent.velocity.magnitude > 0.01f);
    }

    public void BeginAttack()
    {
        UpdateTarget();
        gameObject.SetActive(true);
    }

    public void UpdateTarget()
    {
        activeTarget = ObtainTarget();

        if(activeTarget != null)
        {
            _agent.SetDestination(activeTarget.transform.position + (activeTarget.transform.rotation * activeTarget.SoftPoint));
            TargetName = activeTarget.name;
            destructionActive = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter: {other.name}");

        var otherDestructionTargetScript = other.GetComponent<DestructionTargetScript>();
        if(otherDestructionTargetScript != null && activeTarget != null && otherDestructionTargetScript == activeTarget)
        {
            destructionActive = true;
        }
        var otherTriggerWeaponHitScript = other.GetComponent<TriggerWeaponHitScript>();
        if(otherTriggerWeaponHitScript != null)
        {
            TakeHit(25);
        }
    }

    private void Strike()
    {
        if(activeTarget != null && activeTarget.Damage(UnityEngine.Random.Range(2f, 7f)))
        {
            UpdateTarget();
        }
    }

    private void TakeHit(float hitPoints)
    {
        HealthPonts -= hitPoints;
        Debug.Log($"TakeHit({hitPoints} => {HealthPonts})");

        if (HealthPonts <= 0)
        {
            _animator.SetBool(_animIDDie, true);
            _agent.ResetPath();
        }
        else
        {
            _animator.SetBool(_animIDTakeDamage, true);
        }

    }

    public void OnDeath()
    {
        if (HealthPonts <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void OnTakeDamage01End()
    {
        _animator.SetBool(_animIDTakeDamage, false);
    }

    public void OnDeathEnd()
    {
        Destroy(gameObject);
        _spawnDropScript?.Drop();
    }

    public void OnDeathStart()
    {

    }
}

