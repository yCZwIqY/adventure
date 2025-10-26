using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer mainMixer;

    [Header("Sliders")]
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    private GameData data;

    private void Start()
    {
        // 데이터 불러오기
        data = SaveManager.Load();

        // 슬라이더 초기값 설정 (0~1 스케일로 변환)
        masterSlider.value = data.masterVolume;
        bgmSlider.value = data.bgmVolume;
        sfxSlider.value = data.sfxVolume;

        // 오디오에 반영
        ApplyVolumes();

        // 슬라이더 이벤트 연결
        masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    private void ApplyVolumes()
    {
        mainMixer.SetFloat("MasterVolume", Mathf.Log10(masterSlider.value) * 20);
        mainMixer.SetFloat("BGMVolume", Mathf.Log10(bgmSlider.value) * 20);
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(sfxSlider.value) * 20);
    }

    private void OnMasterVolumeChanged(float value)
    {
        data.masterVolume = value * 100f;
        mainMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        SaveManager.Save(data);
    }

    private void OnBGMVolumeChanged(float value)
    {
        data.bgmVolume = value * 100f;
        mainMixer.SetFloat("BGMVolume", Mathf.Log10(value) * 20);
        SaveManager.Save(data);
    }

    private void OnSFXVolumeChanged(float value)
    {
        data.sfxVolume = value * 100f;
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        SaveManager.Save(data);
    }
}