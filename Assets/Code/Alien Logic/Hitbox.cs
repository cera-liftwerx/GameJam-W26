using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
public class Hitbox : MonoBehaviour
{
    [HideInInspector] public Collider hitCollider;
    public float damage;
    protected AudioSource audioSource;
    [SerializeField] protected AudioClip impactSound;

    protected virtual void Awake()
    {
        hitCollider = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        hitCollider.isTrigger = true;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        HandleTriggerEvent(other);
    }

    protected virtual void HandleTriggerEvent(Collider other)
    {
        Debug.Log("Trigger event occured");
    }
}
