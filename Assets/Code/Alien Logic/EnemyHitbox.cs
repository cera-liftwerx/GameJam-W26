using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : Hitbox
{
    protected override void HandleTriggerEvent(Collider other)
    {
        base.HandleTriggerEvent(other);
        Character character = other.gameObject.GetComponent<Character>();
        if (character != null && !(character is EnemyCharacter))
        {
            if (Time.time - character.lastHitTime >= character.hitCooldown)
            {
                Debug.Log("Test1: " + character.transform.name);
                character.TakeDamage(damage);
                audioSource.PlayOneShot(impactSound);
                character.lastHitTime = Time.time;
            }
        }
    }
}
