using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerController pc;
    private Rigidbody rb;
    public Animator animator;
    public float dashAccelerationTime = 1f;
    public float jumpForce = 7f;

    [SerializeField] private float dashPower = 30f;
    [SerializeField] private float dashDuration = 0.25f;
    [SerializeField] private float dashDrag = 8f;

    public GameObject groundHitEffect;
    public bool isGrounded;
    public bool isDashing = false;

    public GameObject groundHItEffect;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pc = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        isGrounded = true;
    }

    public void Jump(float distance)
    {
        if (!isGrounded) return;

        isGrounded = false;
        animator.SetTrigger("Jump");
        animator.SetBool("Grounded", isGrounded);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void Move(int dir, float distance)
    {
        if (!isGrounded) return;
        float swipeRatio = distance / Screen.width;
        float speed = dir * swipeRatio * 10;
        animator.SetFloat("MoveSpeed", Mathf.Abs(speed));
        Vector3 move = new Vector3(dir * swipeRatio * 10 * Time.deltaTime, 0, 0);
        transform.position += move;

        transform.rotation = Quaternion.Euler(0f, dir * 110f, 0f);
    }

    public void Dash(int dir, float distance)
    {
        Debug.Log("Dash: " + dir);
        if (isDashing) return;
        StartCoroutine(DashRoutine(dir, distance));
    }

    private IEnumerator DashRoutine(int dir, float distance)
    {
        isDashing = true;

        float originalDrag = rb.linearDamping;
        bool wasUsingGravity = rb.useGravity;

        // 중력 영향 제거 (공중에서도 유지)
        rb.useGravity = false;
        rb.linearDamping = dashDrag;
        rb.linearVelocity = Vector3.zero;

        // 전방으로 순간 가속
        transform.rotation = Quaternion.Euler(0f, dir * 110f, 0f);
        rb.AddForce(new Vector3(dir * dashPower, 0f, 0f), ForceMode.Impulse);
        animator.SetTrigger("Dash");

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 대시 종료 후 원상 복귀
        rb.linearDamping = originalDrag;
        rb.useGravity = wasUsingGravity;
        isDashing = false;
    }


    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !isGrounded)
        {
            isGrounded = true;
            animator.SetBool("Grounded", isGrounded);
            GameObject effect = Instantiate(groundHItEffect, transform);
            effect.transform.localPosition = Vector3.zero;
        }
    }
}