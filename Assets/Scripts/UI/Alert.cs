using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Alert : MonoBehaviour
{
    [Header("UI References")]
    public Outline outline;
    public TMP_Text title;
    public TMP_Text desc;
    public Button confirmButton;
    public Button closeButton;

    [Header("Settings")]
    public AlertStatus status;
    public bool isAutoClose = false;
    public float autoCloseDelay = 1.5f;
    public float fadeDuration = 0.5f;

    // 외부에서 확인버튼 동작을 지정할 수 있음
    public Action onConfirm;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void Start()
    {
        outline = GetComponent<Outline>();
        SetColorsByStatus(status);

        // 버튼 이벤트 초기화
        confirmButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();

        if (isAutoClose)
        {
            confirmButton.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(false);
            closeButton.onClick.AddListener(() => StartCoroutine(FadeOutAndClose()));

            // 일정 시간 후 자동 페이드아웃
            StartCoroutine(AutoCloseRoutine());
        }
        else
        {
            // ✅ 수동 모드: 확인 + 닫기 버튼 모두 표시
            confirmButton.gameObject.SetActive(true);
            closeButton.gameObject.SetActive(true);

            confirmButton.onClick.AddListener(() =>
            {
                onConfirm?.Invoke(); // 외부 콜백 실행
                StartCoroutine(FadeOutAndClose());
            });

            closeButton.onClick.AddListener(() => StartCoroutine(FadeOutAndClose()));
        }
    }

    private IEnumerator AutoCloseRoutine()
    {
        yield return new WaitForSeconds(autoCloseDelay);
        yield return StartCoroutine(FadeOutAndClose());
    }

    private IEnumerator FadeOutAndClose()
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        Close();
    }

    private void SetColorsByStatus(AlertStatus currentStatus)
    {
        Color outlineColor;
        Color titleColor;

        switch (currentStatus)
        {
            case AlertStatus.Success:
                title.SetText("Success");
                ColorUtility.TryParseHtmlString("#3FE7AB", out outlineColor);
                ColorUtility.TryParseHtmlString("#3FE7AB", out titleColor);
                break;
            case AlertStatus.Warning:
                title.SetText("Warning");
                ColorUtility.TryParseHtmlString("#FFCA0D", out outlineColor);
                ColorUtility.TryParseHtmlString("#FFCA0D", out titleColor);
                break;
            case AlertStatus.Error:
                title.SetText("Error");
                ColorUtility.TryParseHtmlString("#FF532C", out outlineColor);
                ColorUtility.TryParseHtmlString("#FF532C", out titleColor);
                break;
            case AlertStatus.Info:
            default:
                title.SetText("Info");
                ColorUtility.TryParseHtmlString("#BDFBFF", out outlineColor);
                ColorUtility.TryParseHtmlString("#BDFBFF", out titleColor);
                break;
        }

        outline.effectColor = outlineColor;
        title.color = titleColor;
        desc.color = Color.white;
    }

    public void Close()
    {
        Destroy(gameObject);
    }
}
