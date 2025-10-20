using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerInputManager playerInputManager;
    public PlayerMovement playerMovement;
    public PlayerCombat playerCombat;

    void Start()
    {
        // SwipeDetector 컴포넌트 가져오기
        playerMovement = GetComponent<PlayerMovement>();
        playerInputManager = GetComponent<PlayerInputManager>();
        playerCombat = GetComponent<PlayerCombat>();

        // 이벤트 구독
        playerInputManager.OnSwipeUp += playerMovement.Jump;
        playerInputManager.OnSwipeProcessHorizontal += playerMovement.Move;
        playerInputManager.OnSwipeHorizontal += playerMovement.Dash;

        playerInputManager.OnTap += HandleTap;
        playerInputManager.OnLongPress += HandleLongPress;
        playerInputManager.OnLongPressRelease += HandleLongPressRelease;
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
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