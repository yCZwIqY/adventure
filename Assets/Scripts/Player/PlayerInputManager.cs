using System;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public Animator animator;

    [Header("Swipe Settings")] [SerializeField]
    private float minSwipeDistance = 50f;

    [SerializeField] private float dashSwipeDistance = 150f;
    [SerializeField] private float maxSwipeTime = 1f;

    [Header("Touch Settings")] [SerializeField]
    private float tapTimeThreshold = 0.2f;

    [SerializeField] private float longPressTimeThreshold = 0.5f;
    [SerializeField] private float tapDistanceThreshold = 10f;

    private Vector2 startPos;
    private Vector2 endPos;
    private float startTime;
    private float endTime;
    private bool isTouching = false;
    private bool longPressTriggered = false;

    // 스와이프 이벤트
    public event Action<float> OnSwipeUp;
    public event Action<int, float> OnSwipeHorizontal;
    public event Action<int, float> OnSwipeProcessHorizontal;

    // 터치 이벤트
    public event Action OnTap;
    public event Action<Vector2> OnLongPress;
    public event Action<Vector2> OnLongPressRelease;

    private PlayerController pc;

    void Start()
    {
        pc = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
        CheckLongPress();
    }

    public void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            HandleTouch(Input.GetTouch(0));
        }
#if UNITY_EDITOR
        else
        {
            HandleMouse();
        }
#endif
    }

    private void HandleTouch(Touch touch)
    {
        Vector2 swipe = touch.position - startPos;
        switch (touch.phase)
        {
            case TouchPhase.Began:
                StartTouch(touch.position);
                break;
            case TouchPhase.Moved:
                ProcessSwipe(swipe, touch.position);
                break;
            case TouchPhase.Ended:
                EndTouch(touch.position);
                break;
        }
    }

    private void HandleMouse()
    {
        Vector2 swipe = (Vector2)Input.mousePosition - startPos;
        if (Input.GetMouseButtonDown(0))
        {
            StartTouch(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            ProcessSwipe(swipe, Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndTouch(Input.mousePosition);
        }
    }

    private void ProcessSwipe(Vector2 swipe, Vector2 pos)
    {
        if (!isTouching) return;

        float distance = Mathf.Abs(startPos.x - pos.x);
        bool isVertical = (pos.y - startPos.y) > (Screen.height / 4);

        // 최소 거리 체크
        if (distance < minSwipeDistance || isVertical) return;

        float currentTouchTime = Time.time - startTime;
        if (currentTouchTime >= tapTimeThreshold)
        {
            int direction = swipe.x > 0 ? 1 : -1;
            OnSwipeProcessHorizontal?.Invoke(direction, distance);
        }
    }

    private void StartTouch(Vector2 position)
    {
        isTouching = true;
        longPressTriggered = false;
        startPos = position;
        startTime = Time.time;
    }

    private void EndTouch(Vector2 position)
    {
        if (!isTouching) return;

        animator.SetFloat("MoveSpeed", 0);

        endPos = position;
        endTime = Time.time;
        float touchTime = endTime - startTime;
        float touchDistance = Vector2.Distance(startPos, endPos);

        // 롱프레스 체크
        if (longPressTriggered)
        {
            // 롱프레스였으면 다른 이벤트 무시
            isTouching = false;
            longPressTriggered = false;
            OnLongPressRelease(endPos);
            return;
        }

        // 탭 판별
        if (touchTime <= tapTimeThreshold && touchDistance <= tapDistanceThreshold)
        {
            Debug.Log("Tap");
            OnTap?.Invoke();
        }
        // 점프 판별
        else if (endPos.y - startPos.y > (Screen.height / 4))
        {
            OnSwipeUp?.Invoke(touchDistance);
        }
        else if (Mathf.Abs(endPos.x - startPos.x) > dashSwipeDistance && touchTime < tapTimeThreshold)
        {
            int dir = (endPos.x > startPos.x ? 1 : -1);
            OnSwipeHorizontal(dir, touchDistance);
        }

        isTouching = false;
        longPressTriggered = false;
    }

    private void CheckLongPress()
    {
        if (!isTouching || longPressTriggered) return;

        float currentTouchTime = Time.time - startTime;
        Vector2 currentPos = Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;
        float currentDistance = Vector2.Distance(startPos, currentPos);

        // 롱프레스 판별
        if (currentTouchTime >= longPressTimeThreshold && currentDistance <= tapDistanceThreshold)
        {
            longPressTriggered = true;
            OnLongPress?.Invoke(startPos);
        }
    }

    // 입력 정보를 반환하는 메서드
    public InputData GetInputData()
    {
        return new InputData
        {
            StartPosition = startPos,
            EndPosition = endPos,
            Direction = (endPos - startPos).normalized,
            Distance = Vector2.Distance(startPos, endPos),
            Duration = endTime - startTime
        };
    }

    // 스와이프 강도 판별
    public SwipeIntensity GetSwipeIntensity(float distance)
    {
        if (distance >= dashSwipeDistance)
            return SwipeIntensity.Dash;
        else
            return SwipeIntensity.Walk;
    }
}

// 스와이프 강도 열거형
public enum SwipeIntensity
{
    Walk,
    Dash
}

// 입력 데이터 구조체
public struct InputData
{
    public Vector2 StartPosition;
    public Vector2 EndPosition;
    public Vector2 Direction;
    public float Distance;
    public float Duration;
}