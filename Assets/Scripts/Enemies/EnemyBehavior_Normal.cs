using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior_Normal : EnemyBehavior
{
    [Header("Animation Controller")]
    [SerializeField] protected RuntimeAnimatorController spawnController;
    [SerializeField] protected RuntimeAnimatorController walkController;
    [SerializeField] protected RuntimeAnimatorController attackController;
    [SerializeField] protected RuntimeAnimatorController idleController;
    [SerializeField] protected RuntimeAnimatorController deathController;

    [Header("Death Fade")]
    [SerializeField] protected int deathPulseCount = 3;
    [SerializeField] protected float deathPulseDuration = 0.3f; // duration for one half of pulse (fade out or fade in)

    protected override IEnumerator WalkPhase()
    {
        float elapsed = 0f;
        OnWalkStart();

        while (elapsed < walkDuration)
        {
            float distance = Vector3.Distance(transform.position, endPos);

            if (distance < offset)
            {
                break;
            }

            float step = walkSpeed * Time.deltaTime;

            if (dir != Vector3.zero)
            {
                transform.position += dir.normalized * step;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        OnWalkEnd();

        StartCoroutine(IdlePhase());
    }

    protected override IEnumerator AttackPhase()
    {
        OnAttack();
        animator.Rebind();
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !animator.IsInTransition(0));
        OnIdleEnter();
        yield return new WaitForSeconds(idleDuration);
        StartCoroutine(IdlePhase());
    }

    protected void ApplyController(RuntimeAnimatorController controller)
    {
        if (animator == null || controller == null)
            return;

        if (animator.runtimeAnimatorController != controller)
        {
            animator.runtimeAnimatorController = controller;
        }
    }


    protected override void OnWalkStart() { ApplyController(walkController); }

    protected override void OnIdleEnter() { ApplyController(idleController); }

    protected override void OnAttack()
    {
        ApplyController(attackController);
    }

    protected override void AttackLand()
    {
        PlayerStatsManager.instance.TakeDamage();
    }

    protected override void OnDeath()
    {
        ApplyController(deathController);
        StartCoroutine(DeathSequence());
    }

    protected virtual IEnumerator DeathSequence()
    {
        // Collect renderers to update alpha on (including children)
        var spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        var renderers = GetComponentsInChildren<Renderer>(true);

        // Store original colors
        Color[] spriteOriginalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
            spriteOriginalColors[i] = spriteRenderers[i].color;

        Color[] rendererOriginalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            // Try to read material color safely
            try
            {
                rendererOriginalColors[i] = r.material.color;
            }
            catch
            {
                rendererOriginalColors[i] = Color.white;
            }
        }

        for (int pulse = 0; pulse < deathPulseCount; pulse++)
        {
            // Fade out
            float t = 0f;
            while (t < deathPulseDuration)
            {
                float alpha = Mathf.Lerp(1f, 0f, t / deathPulseDuration);
                // apply alpha
                for (int i = 0; i < spriteRenderers.Length; i++)
                {
                    var c = spriteOriginalColors[i];
                    c.a = alpha * spriteOriginalColors[i].a;
                    spriteRenderers[i].color = c;
                }
                for (int i = 0; i < renderers.Length; i++)
                {
                    var r = renderers[i];
                    var c = rendererOriginalColors[i];
                    c.a = alpha * rendererOriginalColors[i].a;
                    try { r.material.color = c; } catch { }
                }

                t += Time.deltaTime;
                yield return null;
            }

            // Ensure fully invisible at end of fade-out
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                var c = spriteOriginalColors[i];
                c.a = 0f;
                spriteRenderers[i].color = c;
            }
            for (int i = 0; i < renderers.Length; i++)
            {
                var r = renderers[i];
                var c = rendererOriginalColors[i];
                c.a = 0f;
                try { r.material.color = c; } catch { }
            }

            // Fade in
            t = 0f;
            while (t < deathPulseDuration)
            {
                float alpha = Mathf.Lerp(0f, 1f, t / deathPulseDuration);
                for (int i = 0; i < spriteRenderers.Length; i++)
                {
                    var c = spriteOriginalColors[i];
                    c.a = alpha * spriteOriginalColors[i].a;
                    spriteRenderers[i].color = c;
                }
                for (int i = 0; i < renderers.Length; i++)
                {
                    var r = renderers[i];
                    var c = rendererOriginalColors[i];
                    c.a = alpha * rendererOriginalColors[i].a;
                    try { r.material.color = c; } catch { }
                }

                t += Time.deltaTime;
                yield return null;
            }

            // Restore original alpha at end of pulse
            for (int i = 0; i < spriteRenderers.Length; i++)
                spriteRenderers[i].color = spriteOriginalColors[i];
            for (int i = 0; i < renderers.Length; i++)
            {
                try { renderers[i].material.color = rendererOriginalColors[i]; } catch { }
            }
        }

        // finally destroy the object
        Destroy(gameObject);
    }
}
