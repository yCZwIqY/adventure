using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerController controller;
    private Vector2 touchStartPos;
    private float touchStartTime;
    private float maxTapTime = 0.2f;

    [Header("Swipe Settings")] public float minSwipeDist = 50f;
    public float defendHoldTime = 0.5f;

    private void Start()
    {
        controller = GetComponent<PlayerController>();
    }

    public void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            HandleTouch(Input.GetTouch(0));
        }

#if UNITY_EDITOR
        HandleMouse();
#endif
    }

    private void HandleTouch(Touch touch)
    {
        Vector2 swipe = touch.position - touchStartPos;
        switch (touch.phase)
        {
            case TouchPhase.Began:
                touchStartPos = touch.position;
                touchStartTime = Time.time;
                break;
            case TouchPhase.Moved:
                ProcessSwipe(swipe, false);
                break;
            case TouchPhase.Ended:
                ProcessSwipe(swipe, true, Time.time - touchStartTime);
                break;
        }
    }

    private void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
            touchStartTime = Time.time;
        }
        else if (Input.GetMouseButton(0))
        {
            ProcessSwipe((Vector2)Input.mousePosition - touchStartPos, false);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ProcessSwipe((Vector2)Input.mousePosition - touchStartPos, true, Time.time - touchStartTime);
        }
    }

    private void ProcessSwipe(Vector2 swipe, bool released, float duration = 0f)
    {
        var move = controller.movement;
        var combat = controller.combat;

        if (!released)
        {
            float holdTime = Time.time - touchStartTime;

            // 방어
            if (holdTime > defendHoldTime && !combat.isDefending && swipe.magnitude < minSwipeDist)
            {
                combat.StartDefend();
                return;
            }

            // 이동 또는 점프
            if (swipe.magnitude >= minSwipeDist)
            {
                if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
                {
                    move.SetDirection(swipe.x > 0 ? 1 : -1, swipe.magnitude);
                }
                else if (swipe.y > 0 && move.isGrounded)
                {
                    move.Jump();
                }
            }
        }
        else
        {
            move.Stop();

            if (combat.isDefending)
            {
                combat.EndDefend();
                return;
            }

            if (duration < maxTapTime && swipe.magnitude < minSwipeDist)
                combat.Attack();
        }
    }
}