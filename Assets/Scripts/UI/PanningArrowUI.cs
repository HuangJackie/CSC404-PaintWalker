using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PanningArrowUI : MonoBehaviour
    {
        private List<Image> arrowImages;

        void Start()
        {
            arrowImages = new List<Image>();
            foreach (Transform childArrow in transform)
            {
                Image imageChild = childArrow.gameObject.GetComponent<Image>();
                arrowImages.Add(imageChild);
                imageChild.enabled = false;
            }
        }

        public void EnablePanningArrows(bool isEnabled)
        {
            foreach (Image childArrow in arrowImages)
            {
                childArrow.enabled = isEnabled;
            }
        }
    }
}