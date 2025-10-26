using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("튜토리얼 이미지들")]
    public List<GameObject> tutorials;

    [Header("버튼")]
    public Button nextButton;
    public Button prevButton;

    private int currentStep = 0;

    private void Start()
    {
        UpdateTutorial();
    }

    public void OnNext()
    {
        if (currentStep < tutorials.Count - 1)
        {
            currentStep++;
            UpdateTutorial();
        }
    }

    public void OnPrev()
    {
        if (currentStep > 0)
        {
            currentStep--;
            UpdateTutorial();
        }
    }

    private void UpdateTutorial()
    {
        // 모든 튜토리얼 이미지 비활성화
        for (int i = 0; i < tutorials.Count; i++)
        {
            tutorials[i].SetActive(i == currentStep);
        }

        // 버튼 상태 갱신
        prevButton.interactable = currentStep > 0;
        nextButton.interactable = currentStep < tutorials.Count - 1;
    }
    
}