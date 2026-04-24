using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using System.Runtime.CompilerServices;
using UnityEngine.UI;

public class PlayerCharacter : Character
{
    [SerializeField] public MicrophoneDetector microphoneDetector;
    [SerializeField] SimpleFadeIn simpleFadeIn;
    [SerializeField] private Slider healthBarSlider;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        if (microphoneDetector == null) Debug.LogWarning("The microphone is null warning");
        if (simpleFadeIn == null) Debug.LogWarning("The fade in controller is null warning");
        audioSource.volume = 0.5f;
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        // update the fill amount
        healthBarSlider.value = currentHealth / maxHealth;
    }

    public override void Heal(float hp)
    {
        base.Heal(hp);
        // GameJamManager.Instance.SetPlayerHealthbar(currentHealth, maxHealth);
    }

    private void SetActiveForAllChildren(Transform root, bool active)
    {
        foreach (Transform t in root)
        {
            t.gameObject.SetActive(active);
            SetActiveForAllChildren(t, active);
        }
    }


    public override void TakeDamage(float damage)
    {
        //if (GameJamManager.gameState != GameJamManager.GameState.Playing) return;

        Debug.Log("!!!base player damage taken" + damage);
        if (currentHealth < 0) currentHealth = 0;
        currentHealth -= damage;
        //GameJamManager.Instance.SetPlayerHealthbar(currentHealth,maxHealth);

        audioSource.PlayOneShot(hitSound);

        CheckDeath();
    }

    protected override IEnumerator Die()
    {
        //GameJamManager.Instance.GameOver();
        simpleFadeIn.FadeFromTransparentToBlack();
        yield return null;
        GameManager.instance.OnPlayerDied();
    }
}