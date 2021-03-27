using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RTSTutorial
{
    [RequireComponent(typeof(Health))]
    public class HealthDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Health _health;
        [SerializeField] private GameObject _healthBarParent;
        [SerializeField] private Image _healthBarImage;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _health.ClientOnHealthUpdated += HandleHealthUpdated;
            _healthBarParent.SetActive(false);
        }

        private void OnDestroy()
        {
            _health.ClientOnHealthUpdated -= HandleHealthUpdated;
        }

        private void HandleHealthUpdated(int currentHealth, int maxHealth)
        {
            _healthBarImage.fillAmount = (float) currentHealth / maxHealth;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _healthBarParent.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _healthBarParent.SetActive(false);
        }
    }
}