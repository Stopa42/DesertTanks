using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RTSTutorial
{
    public class BuildingButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private BuilderPreviewHandler _previewHandler;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _priceText;

        private int _typeId;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            _previewHandler.SelectBuilding(_typeId);
        }
        
        public void SetForBuilding(Building building, int typeId)
        {
            _iconImage.sprite = building.Icon;
            _priceText.text = building.GetComponent<Purchasable>().Price.ToString();
            _typeId = typeId;
        }
    }
}
