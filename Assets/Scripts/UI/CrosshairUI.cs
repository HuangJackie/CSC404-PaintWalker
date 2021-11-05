using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    public Sprite crosshair;
    private Image _crosshairImage;
    

    // Start is called before the first frame update
    void Start()
    {
        _crosshairImage = GetComponent<Image>();
    }

    public void EnableCrossHair(bool isEnabled)
    {
        _crosshairImage.enabled = isEnabled;
    }
    
}
