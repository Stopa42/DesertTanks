using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RTSTutorial
{
    [RequireComponent(typeof(UnitSpawner))]
    public class UnitSpawnerUI : MonoBehaviour
    {
        [SerializeField] private Image _unitProgressImage;
        [SerializeField] private GameObject _spawnerUIParent;
        [SerializeField] private TMP_Text _unitsQueuedText;

        private UnitSpawner _unitSpawner;
        private float _progressImageVelocity;
        private bool _isQueued;

        private void Awake()
        {
            _unitSpawner = GetComponent<UnitSpawner>();
            _unitSpawner.QueuedUnitsChanged += HandleQueuedUnitsChanged;
        }

        private void Start()
        {
            UpdateVisibility();
            UpdateTimerDisplay();
        }

        private void Update()
        {
            if (_unitSpawner.isClient && _isQueued) UpdateTimerDisplay();
        }

        private void UpdateTimerDisplay()
        {
            var newProgress = _unitSpawner.UnitTimerPercentage;

            if (newProgress > _unitProgressImage.fillAmount)
            {
                UpdateTimerDisplaySmoothly(newProgress);
            }
            else
            {
                _unitProgressImage.fillAmount = newProgress;
            }
        }

        private void UpdateTimerDisplaySmoothly(float newProgress)
        {
            _unitProgressImage.fillAmount = Mathf.SmoothDamp(
                _unitProgressImage.fillAmount,
                newProgress,
                ref _progressImageVelocity,
                0.1f);
        }

        private void HandleQueuedUnitsChanged()
        {
            if (UpdateVisibility()) return;
            UpdateQueueText(_unitSpawner.UnitsQueued);
        }

        private bool UpdateVisibility()
        {
            if (_unitSpawner.UnitsQueued == 0)
            {
                SetActive(false);
                return true;
            }
            SetActive(true);
            return false;
        }

        private void SetActive(bool state)
        {
            _spawnerUIParent.SetActive(state);
            _isQueued = state;
        }

        private void UpdateQueueText(int remainingUnits)
        {
            _unitsQueuedText.text = remainingUnits.ToString();
        }

        private void OnDestroy()
        {
            _unitSpawner.QueuedUnitsChanged -= HandleQueuedUnitsChanged;
        }
    }
}