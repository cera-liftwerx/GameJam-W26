using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyCharacter : Character
{
    private NavMeshAgent agent;
    private Animator animator;
    private PlayerCharacter player;
    [SerializeField] private float aggroDistance = 10f; // radius to attack player in (main objective to to hit tower)
    [SerializeField] private float minAttackDelay = 1f;
    [SerializeField] private float maxAttackDelay = 1f;
    [SerializeField] private float attackBuildup = 0f;
    [SerializeField] private float attackDuration = 0.5f;
    [SerializeField] private float speed = 5f;
    private EnemyHitbox attackHitBox;
    private static readonly System.Random rng = new System.Random(); 
    protected MicrophoneDetector microphoneDetector;
    protected bool chasing;
    [SerializeField] private Canvas healthBarCanvas;
    [SerializeField] private Slider healthBarSlider;
    private Coroutine autoAttackCoroutine;
    public bool isBoss = false;

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

    void OnParticleCollision(GameObject other)
    {
        Debug.Log("[sadge] OnParticleCollision");
        TakeDamage(attackHitBox.damage);
    }

    protected override void Update()
    {
        // make the health bar always face the main camera
        healthBarCanvas.transform.LookAt(
            healthBarCanvas.transform.position - Camera.main.transform.rotation * Vector3.forward,
            Camera.main.transform.rotation * Vector3.up
        );

        // update the fill amount
        healthBarSlider.value = currentHealth / maxHealth;
    }

    protected void FixedUpdate()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.transform.position);
        bool tooClose = distToPlayer < aggroDistance;
        bool tooLoud = microphoneDetector.isDetecting && distToPlayer < 15f;

        // movement is completely separate, just follow if close or loud
        if (tooClose || tooLoud)
        {
            agent.SetDestination(player.transform.position);
            agent.speed = speed;

            if (!chasing)
            {
                chasing = true;
                animator.SetTrigger("Moving");
            }
        }
        else if (chasing)
        {
            // player left range, stop moving
            chasing = false;
            agent.ResetPath();
            agent.speed = 0f;
            animator.SetTrigger("Idle");
        }

        // attack loop runs on its own, no connection to chasing
        bool inMeleeRange = distToPlayer < aggroDistance;
        if (inMeleeRange && autoAttackCoroutine == null)
        {
            autoAttackCoroutine = StartCoroutine(AutoAttack());
        }
        else if (!inMeleeRange && autoAttackCoroutine != null)
        {
            StopCoroutine(autoAttackCoroutine);
            autoAttackCoroutine = null;
            attackHitBox.hitCollider.enabled = false;
        }
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
        Debug.Log("[sadge] enemy damage taken" + damage);
        StartCoroutine(ShowDamageText(damage, Color.white));
        base.TakeDamage(damage);
    }

    protected override IEnumerator Die()
    {
        Debug.Log("[sadge] enemy Die");
        animator.SetTrigger("Die");
        agent.enabled = false;
        audioSource.PlayOneShot(deathSound);
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
        
        if (isBoss)
        {
            GameManager.instance.OnBossDefeated();
        }
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
