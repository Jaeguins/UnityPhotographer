using UnityEngine;
using Photograph;

class ExampleObject : MonoBehaviour {
    public Indicator Ind;
    public Photographer Photographer;
    [SerializeField]
    [Range(.01f, 100f)]
    private float multiplier = 1f;
    public bool IsOrtho=false;
    public Vector3 Aim;
    public Vector3 Direction;
    void OnMouseDown() {
        Debug.Log(gameObject.name + " clicked.");
        Ind.SetTexture(Photographer.TakePicture(gameObject, Direction, Aim,IsOrtho,multiplier));
    }
}