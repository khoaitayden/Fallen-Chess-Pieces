using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("Volume Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    private void Start()
    {
        masterSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
        sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
        musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
    }
        public void ToggleVisibility()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}