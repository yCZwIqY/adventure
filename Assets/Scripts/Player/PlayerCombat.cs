using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public GameObject attackEffect;

    private PlayerController pc;

    public bool isDefend = false;

    void Start()
    {
        pc = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
        GameObject effect = Instantiate(attackEffect, transform);
        if (pc.playerMovement.isGrounded)
        {
            effect.transform.localPosition = new Vector3(0.5f, 0.6f, 0.5f);
            effect.transform.localEulerAngles = new Vector3(90, 0, 160);
        }
        else
        {
            effect.transform.localPosition = new Vector3(0, 0.7f, 0);
            effect.transform.localEulerAngles = new Vector3(0, -90, 130);
        }
    }

    public void Padding()
    {
    }

    public void Defense()
    {
        isDefend = true;
        animator.SetBool("Defend", isDefend);
    }
    
    //방어 해제
    public void ReleaseDefense()
    {
        isDefend = false;
        animator.SetBool("Defend", isDefend);
    }

}