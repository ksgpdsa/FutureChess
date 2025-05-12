using Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DefaultNamespace.UI
{
    public class ManageHoverImageSwitch : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private bool disableHandledImage = true;
        
        private Image _image;
        private Image _imageInChildren;
        private RawImage _rawImage;
        private RawImage _rawImageInChildren;

        private void Awake()
        {
            _image = GetComponent<Image>();

            if (_image)
            {
                _imageInChildren = gameObject.GetComponentOnlyInChildren<Image>();
            }
            else
            {
                _rawImage = GetComponent<RawImage>();

                if (_rawImage)
                {
                    _rawImageInChildren = gameObject.GetComponentOnlyInChildren<RawImage>();
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_image && _imageInChildren)
            {
                if (disableHandledImage)
                {
                    _image.enabled = false;
                }
                
                _imageInChildren.enabled = true;
            }
            else if (_rawImage && _rawImageInChildren)
            {
                if (disableHandledImage)
                {
                    _rawImage.enabled = false;
                }
                
                _rawImageInChildren.enabled = true;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_image && _imageInChildren)
            {
                if (disableHandledImage)
                {
                    _image.enabled = true;
                }
                
                _imageInChildren.enabled = false;
            }
            else if (_rawImage && _rawImageInChildren)
            {
                if (disableHandledImage)
                {
                    _rawImage.enabled = true;
                }
                
                _rawImageInChildren.enabled = false;
            }
        }
    }
}