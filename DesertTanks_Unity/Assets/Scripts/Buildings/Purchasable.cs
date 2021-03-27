using UnityEngine;

namespace RTSTutorial
{
    public class Purchasable : MonoBehaviour
    {
        [SerializeField] private int _price;
        
        public int Price => _price;
    }
}