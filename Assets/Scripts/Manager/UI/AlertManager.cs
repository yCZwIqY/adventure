using System;
using UnityEngine;

public class AlertManager : MonoBehaviour
{
    public static AlertManager Instance { get; private set; }
    public Canvas canvas;
    public GameObject alertPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (canvas == null)
            canvas = FindFirstObjectByType<Canvas>();
    }

    // -----------------------------
    // ✅ 간단한 자동 닫힘 알림 (닫기 버튼만, 자동 사라짐)
    // -----------------------------
    public void ShowAutoCloseAlert(string desc, AlertStatus status = AlertStatus.Info)
    {
        ShowAlert("Info", desc, status, isAutoClose: true);
    }

    public void ShowSuccessAlert(string desc)
    {
        ShowAlert("Success", desc, AlertStatus.Success, isAutoClose: true);
    }

    public void ShowAutoCloseInfoAlert(string desc)
    {
        ShowAlert("Info", desc, AlertStatus.Info, isAutoClose: true);
    }

    public void ShowInfoAlert(string desc, Action onConfirm = null)
    {
        ShowAlert("Info", desc, AlertStatus.Info, isAutoClose: false, onConfirm);
    }

    // -----------------------------
    // ⚠️ 수동 닫힘 (확인 + 닫기 버튼)
    // -----------------------------
    public void ShowWarningAlert(string desc, Action onConfirm = null)
    {
        ShowAlert("Warning", desc, AlertStatus.Warning, isAutoClose: false, onConfirm);
    }

    public void ShowErrorAlert(string desc, Action onConfirm = null)
    {
        ShowAlert("Error", desc, AlertStatus.Error, isAutoClose: false, onConfirm);
    }

    // -----------------------------
    // 🧩 공통 Alert 생성 로직
    // -----------------------------
    public void ShowAlert(string title, string description, AlertStatus status, bool isAutoClose = false,
        Action onConfirm = null)
    {
        if (alertPrefab == null)
        {
            Debug.LogError("⚠️ Alert Prefab이 AlertManager에 연결되지 않았습니다.");
            return;
        }

        // 캔버스 기준으로 생성 (없으면 자기 transform)
        Transform parent = canvas != null ? canvas.transform : transform;
        GameObject alertObject = Instantiate(alertPrefab, parent);

        Alert alert = alertObject.GetComponent<Alert>();
        if (alert == null)
        {
            Debug.LogError("⚠️ Alert Prefab에 Alert 컴포넌트가 없습니다.");
            Destroy(alertObject);
            return;
        }

        // 기본 데이터 세팅
        alert.title.text = title;
        alert.desc.text = description;
        alert.status = status;
        alert.isAutoClose = isAutoClose;

        if (!isAutoClose && onConfirm != null)
        {
            alert.onConfirm = onConfirm;
        }
    }
}