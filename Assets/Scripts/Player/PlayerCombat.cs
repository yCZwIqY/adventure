using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public GameObject attackEffect;
    private Rigidbody rb;

    private PlayerController pc;

    public bool isDefend = false;
    public bool isParrying = false;
    public bool isAttack = false;

    public float power = 1;
    public float knockbackPower = 1;
    public float defensePower = 1;

    public AudioClip attackSound;
    public AudioClip defenseSound;


    void Start()
    {
        pc = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    public void Attack()
    {
        if (isAttack || isParrying) return;

        animator.SetTrigger("Attack");
        GameObject effect = Instantiate(attackEffect, transform);

        // SFX 재생
        if (SFXManager.instance != null)
            SFXManager.instance.PlaySFX(attackSound);

        if (pc.playerMovement.isGrounded)
        {
            effect.transform.localPosition = new Vector3(0.5f, 0.6f, 0.5f);
            effect.transform.localEulerAngles = new Vector3(90, 0, 160);
            isAttack = true;
            Invoke(nameof(OffAttack), 0.5f);
        }

        if (!pc.playerMovement.isGrounded)
        {
            effect.transform.localPosition = new Vector3(0, 0.7f, 0);
            effect.transform.localEulerAngles = new Vector3(0, -90, 130);
            isParrying = true;
            Invoke(nameof(OffParring), 0.5f);
        }
    }

    public void Defense()
    {
        if (isDefend) return;
        isDefend = true;
        animator.SetBool("Defend", isDefend);

        // SFX 재생
        if (SFXManager.instance != null)
            SFXManager.instance.PlaySFX(defenseSound);
    }

    //방어 해제
    public void ReleaseDefense()
    {
        if (!isDefend) return;
        isDefend = false;
        animator.SetBool("Defend", isDefend);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (isParrying && other.gameObject.CompareTag("Ground"))
        {
            isParrying = false;
        }
    }

    public void Parrying(float amount = 7f)
    {
        if (!isParrying) return;
        Debug.Log("Parrying");

        rb.useGravity = true; // 중력은 켜둠
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); // 기존 Y속도 제거
        rb.AddForce(Vector3.up * amount, ForceMode.VelocityChange);
    }

    public void OffParring()
    {
        isParrying = false;
    }


    public void OffAttack()
    {
        isAttack = false;
    }
}