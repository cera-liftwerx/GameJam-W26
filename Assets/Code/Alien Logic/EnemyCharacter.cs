using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyCharacter : Character
{
    private NavMeshAgent agent;
    private Animator animator;
    private PlayerCharacter player;
    [SerializeField] private float aggroDistance = 5f; // radius to attack player in (main objective to to hit tower)
    [SerializeField] private float minAttackDelay = 5f;
    [SerializeField] private float maxAttackDelay = 10f;
    [SerializeField] private float attackBuildup = 1f;
    [SerializeField] private float attackDuration = 0.5f;
    [SerializeField] private float speed = 5f;
    private EnemyHitbox attackHitBox;
    private static readonly System.Random rng = new System.Random(); 
    protected MicrophoneDetector microphoneDetector;
    protected bool chasing;
    [SerializeField] private float chasingTime = 15f;
    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        player = FindObjectOfType<PlayerCharacter>();
        Debug.Log("test: " + player.transform.name);
        microphoneDetector = player.microphoneDetector;

        chasing = false;
    }

    protected override void Start()
    {
        base.Start();
        attackHitBox = GetComponentInChildren<EnemyHitbox>();
        attackHitBox.hitCollider.enabled = false;
    }

    protected override void Update()
    {
        base.Update();

        if (chasing) return; // Early exit - prevents any re-triggering

        float distToPlayer = Vector3.Distance(transform.position, player.transform.position);
        bool tooClose = distToPlayer < aggroDistance;
        bool tooLoud  = microphoneDetector.isDetecting && distToPlayer < 5f;

        if (tooClose || tooLoud)
        {
            chasing = true;
            StartCoroutine(ChasePlayer());
        }
    }

    private IEnumerator ChasePlayer()
    {
        animator.SetTrigger("Moving");
        agent.speed = speed;
        StartCoroutine(AutoAttack());
        float elapsed = 0f;
        while (elapsed < chasingTime)
        {
            agent.SetDestination(player.transform.position); // update every frame
            elapsed += Time.deltaTime;
            yield return null;
        }
        StopCoroutine(AutoAttack());
        // Stop and idle
        agent.ResetPath();         // clears destination so agent stops moving
        agent.speed = 0f;
        animator.SetTrigger("Idle");

        chasing = false;
    }
    private IEnumerator AutoAttack()
    {
        while (true)
        {
            float delay = UnityEngine.Random.Range(minAttackDelay, maxAttackDelay);
            yield return new WaitForSeconds(delay);
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(attackBuildup);
            attackHitBox.hitCollider.enabled = true;

            yield return new WaitForSeconds(attackDuration);
            attackHitBox.hitCollider.enabled = false;
        }        
    }

    // jump scare stuff

    public override void TakeDamage(float damage)
    {
        if (!isAlive) return;
        Debug.Log("enemy damage taken" + damage);
        StartCoroutine(ShowDamageText(damage, Color.white));
        base.TakeDamage(damage);
    }

    protected override IEnumerator Die()
    {
        animator.SetTrigger("Die");
        agent.enabled = false;
        audioSource.PlayOneShot(deathSound);
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }

    private IEnumerator ForceDeathAsync()
    {
        animator.SetTrigger("Die");
        agent.enabled = false;
        audioSource.PlayOneShot(deathSound);
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }

    public void ForceDeath()
    {
        StartCoroutine(ForceDeathAsync());
    }
}
