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

    [SerializeField] private float dashPower = 1f;
    [SerializeField] private float dashDuration = 0.25f;

    public GameObject groundHitEffect;
    public bool isGrounded = true;
    public bool isDashing = false;

    public GameObject dashEffect;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pc = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        isGrounded = true;
    }

    public void Jump(float distance)
    {
        if (!isGrounded && isDashing) return;

        rb.linearVelocity = Vector3.zero;
        isGrounded = false;
        animator.SetTrigger("Jump");
        animator.SetBool("Grounded", isGrounded);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void Move(int dir, float distance)
    {
        if (!isGrounded && isDashing) return;
        float swipeRatio = distance / Screen.width;
        float speed = dir * swipeRatio * 10;
        animator.SetFloat("MoveSpeed", Mathf.Abs(speed));
        Vector3 move = new Vector3(dir * swipeRatio * 10 * Time.deltaTime, 0, 0);
        // transform.position += move;
        transform.rotation = Quaternion.Euler(0f, dir * 110f, 0f);

        Debug.DrawRay(transform.position, Vector3.right * dir, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.right * dir, out hit, 1))
        {
            if (hit.transform.CompareTag("Wall"))
            {
                // transform.position = hit.point;
                return;
            }
        }

        transform.position += move;
    }

    public void Dash(int dir)
    {
        if (isDashing) return;
        StartCoroutine(DashRoutine(dir));
    }

    private IEnumerator DashRoutine(int dir)
    {
        animator.SetTrigger("Dash");
        GameObject effect = Instantiate(dashEffect, transform);
        isDashing = true;

        float elapsed = 0f;
        float dashSpeed = dashPower; // 10 정도부터 조절

        while (elapsed < dashDuration)
        {
            transform.position += Vector3.right * dir * dashSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.useGravity = true;
        isDashing = false;
    }


    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !isGrounded)
        {
            isGrounded = true;
            animator.SetBool("Grounded", isGrounded);
            GameObject effect = Instantiate(groundHitEffect, transform);
            effect.transform.localPosition = Vector3.zero;
        }
    }
}