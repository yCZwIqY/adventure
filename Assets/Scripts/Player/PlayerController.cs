using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerInputManager playerInputManager;
    public PlayerMovement playerMovement;
    public PlayerCombat playerCombat;
    public PlayerHealth playerHealth;

    public Animator animator;

    void Start()
    {
        // SwipeDetector 컴포넌트 가져오기
        playerMovement = GetComponent<PlayerMovement>();
        playerInputManager = GetComponent<PlayerInputManager>();
        playerCombat = GetComponent<PlayerCombat>();
        playerHealth = GetComponent<PlayerHealth>();

        animator = GetComponent<Animator>();

        // 이벤트 구독
        SubscribeInputEvent();
    }

    void OnDestroy()
    {
        UnsubscribeInputEvent();
    }

    public void SubscribeInputEvent()
    {
        playerInputManager.OnSwipeUp += playerMovement.Jump;
        playerInputManager.OnSwipeProcessHorizontal += playerMovement.Move;
        playerInputManager.OnSwipeHorizontal += playerMovement.Dash;

        playerInputManager.OnTap += HandleTap;
        playerInputManager.OnLongPress += HandleLongPress;
        playerInputManager.OnLongPressRelease += HandleLongPressRelease;
    }

    public void UnsubscribeInputEvent()
    {
        if (playerInputManager != null)
        {
            playerInputManager.OnSwipeUp -= playerMovement.Jump;
            playerInputManager.OnSwipeProcessHorizontal += playerMovement.Move;

            playerInputManager.OnTap -= HandleTap;
            playerInputManager.OnLongPress -= HandleLongPress;
            playerInputManager.OnLongPressRelease -= HandleLongPressRelease;
        }
    }

    // 터치 핸들러
    private void HandleTap()
    {
        playerCombat.Attack();
    }

    private void HandleLongPress(Vector2 position)
    {
        playerCombat.Defense();
    }

    private void HandleLongPressRelease(Vector2 position)
    {
        playerCombat.ReleaseDefense();
    }
}