using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject prefab;
    public GameObject attackEffect;
    public GameObject parringEffect;
    public AudioClip attackSound;
    public AudioClip parringSound;

    public float attackPower;

    protected virtual void Attack()
    {
        if (SFXManager.instance != null)
            SFXManager.instance.PlaySFX(attackSound);

        GameObject effect = Instantiate(attackEffect, transform);
    }

    protected virtual void Parring()
    {
        if (SFXManager.instance != null)
            SFXManager.instance.PlaySFX(parringSound);

        GameObject effect = Instantiate(parringEffect, transform);
    }
}