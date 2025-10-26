using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public PlayerInputManager playerInputManager;
    public PlayerMovement playerMovement;
    public PlayerCombat playerCombat;
    public PlayerHealth playerHealth;
    public PlayerWallet playerWallet;

    public Animator animator;

    public GameData gameData;

    void Awake()
    {
        gameData = SaveManager.Load();
        
        // 저장된 위치로 복원
        // transform.position = new Vector3(
        //     gameData.playerPosition[0],
        //     gameData.playerPosition[1],
        //     gameData.playerPosition.Length > 2 ? gameData.playerPosition[2] : 0f
        // );
        //
        // SwipeDetector 컴포넌트 가져오기
        playerMovement = GetComponent<PlayerMovement>();
        playerInputManager = GetComponent<PlayerInputManager>();
        playerCombat = GetComponent<PlayerCombat>();
        playerHealth = GetComponent<PlayerHealth>();
        playerWallet = GetComponent<PlayerWallet>();

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