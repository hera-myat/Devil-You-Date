using UnityEngine;
using UnityEngine.UI;

public class AnimateOverlay : MonoBehaviour
{
    public RawImage image;
    public float speedX = 0.01f;
    public float speedY = 0.01f;

    // Update is called once per frame
    void Update()
    {
        if (image == null) return;
        
        Rect uv = image.uvRect;
        uv.x += speedX * Time.deltaTime;
        uv.y += speedY * Time.deltaTime;
        image.uvRect = uv;
        
    }
}
