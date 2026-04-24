using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : Hitbox
{
    /* private CustomGrabInteractable grabInteractable; */
    protected Vector3 lastPosition;

    protected float multiplier = 1f;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        /* grabInteractable = GetComponent<CustomGrabInteractable>(); */
        StartCoroutine(RescanRoutine());
    }
    protected override void HandleTriggerEvent(Collider other)
    {
        /* if (grabInteractable != null && !grabInteractable.isSelected) return; */
        base.HandleTriggerEvent(other);
        EnemyCharacter ec = other.gameObject.GetComponent<EnemyCharacter>();
        if (ec != null)
        {
            if (Time.time - ec.lastHitTime >= ec.hitCooldown)
            {
                Debug.Log("mult: " + multiplier);
                ec.TakeDamage(damage * multiplier);
                audioSource.PlayOneShot(impactSound);
                Vector3 direction = (other.transform.position - transform.position).normalized;
                Vector3 horizontalDir = Vector3.ProjectOnPlane(direction, Vector3.up).normalized;
                Vector3 hitDirection = (horizontalDir + Vector3.up).normalized;
                //ec.GetRigidbody().AddForce(hitDirection * knockbackForce * multiplier, ForceMode.Impulse);
                ec.lastHitTime = Time.time;
            }
        }
    }

    protected override void Update()
    {
        base.Update();
        
    }

    private void FixedUpdate()
    {
        multiplier = Math.Max(Vector3.Distance(lastPosition, transform.position) * 10, 0.5f);
        lastPosition = transform.position;
    }

    private IEnumerator RescanRoutine()
    {
        while (true)
        {
            hitCollider.enabled = true;
            yield return new WaitForSeconds(0.1f);

            hitCollider.enabled = false;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
