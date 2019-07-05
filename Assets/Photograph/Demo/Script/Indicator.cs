using UnityEngine;
using UnityEngine.UI;

class Indicator : MonoBehaviour {
    private RawImage imgIndicator;
    public Image img;
    private float t;
    private void Start() {
        imgIndicator = GetComponent<RawImage>();
    }
    private void Update() {
        img.color=Color.HSVToRGB(t,1,.5f);
        t+=Time.deltaTime;
        if(t>1)t-=1f;
    }
    public void SetTexture(Texture2D target) {
        imgIndicator.texture = target;
    }

}