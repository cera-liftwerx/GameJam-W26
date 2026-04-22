using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Character : MonoBehaviour
{
    protected bool isAlive = true;
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float currentHealth = 100f;
    protected CapsuleCollider mainCollider;
    protected Rigidbody rb;
    public float hitCooldown = 1f;
    public float lastHitTime = float.NegativeInfinity;

    protected AudioSource audioSource;
    [SerializeField] protected AudioClip characterSound;
    [SerializeField] protected AudioClip healSound;
    [SerializeField] protected AudioClip hitSound;
    [SerializeField] protected AudioClip deathSound;

    protected virtual void Awake()
    {
        mainCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        audioSource.clip = characterSound;
        audioSource.loop = true;
        audioSource.Play();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        

    }

    public virtual void Heal(float hp)
    {
        currentHealth += hp;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        audioSource.PlayOneShot(healSound);
    }

    public virtual void TakeDamage(float damage)
    {
        Debug.Log("damage taken" + damage);
        currentHealth -= damage;
        if (damage > 1f)
        {
            audioSource.PlayOneShot(hitSound);
        }
        CheckDeath();
    }

    protected virtual IEnumerator ShowDamageText(float damage, Color color)
    {
        float duration = 1f;
        float elapsed = 0f;
        float riseSpeed = 1f;
        Vector3 spawnPos = mainCollider.bounds.max; 
        spawnPos.x = mainCollider.bounds.center.x; // keep it centered horizontally
        spawnPos.z = mainCollider.bounds.center.z;
        GameObject dmgTextObject = Instantiate(
            damageTextPrefab,
            spawnPos,
            Quaternion.identity
        );
        Destroy(dmgTextObject,duration);
        Debug.Log("dmg text instantiated");

        TextMeshProUGUI dmgText = dmgTextObject.GetComponentInChildren<TextMeshProUGUI>();
        dmgText.text = $"{Mathf.RoundToInt(damage * 100f) / 100f}";
        float invLerp = Mathf.InverseLerp(10f, 100f, damage);
        dmgText.color = Color.Lerp(color, Color.red, invLerp);

        Vector3 startPos = dmgTextObject.transform.position;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Rise upward
            dmgTextObject.transform.position =
                startPos + Vector3.up * riseSpeed * t;

            // Fade out
            Color c = dmgText.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            dmgText.color = c;

            // Face the camera (optional, if world-space)
            dmgTextObject.transform.LookAt(Camera.main.transform);

            yield return null;
        }
    }

    protected virtual void CheckDeath()
    {
        if (currentHealth <= 0f && isAlive)
        {
            isAlive = false;
            Debug.Log(gameObject + "died");
            StartCoroutine(Die());
        }
    }

    protected virtual IEnumerator Die()
    {
        yield break;
    }

    public Rigidbody GetRigidbody()
    {
        return rb;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}
