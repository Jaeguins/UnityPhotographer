using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Custom {
    public class ExampleObject : MonoBehaviour {
        public CurvedStateStruct<float> HeightState;
        public CurvedStateStruct<Color> ColorState;
        public Color TargetColor;
        public float TargetHeight;
        public AnimationCurve[] Curves;
        Material mat;
        private void Start() {
            mat = GetComponent<MeshRenderer>().material;
            HeightState = CurvedStateStruct<float>.Init(
                1,
                (float left, float right) => { return left + right; },
                (float left, float right) => { return left * right; },
                .3f
                );

            ColorState = CurvedStateStruct<Color>.Init(
                Color.white,
                (Color left, Color right) => { return left + right; },
                (float left, Color right) => { return left * right; },
                .2f
                );
            HeightState.SetCurve(Curves[0]);
            ColorState.SetCurve(Curves[1]);
        }
        void Update() {
            mat.color = ColorState;
            transform.localScale = Vector3.one * HeightState;
        }
        private void OnMouseDown() {
            ColorState.To(new Color(Random.value, Random.value, Random.value));
            HeightState.To(Random.value * 3f + 1f);


            TargetColor = ColorState.Target;
            TargetHeight = HeightState.Target;
        }
        public void OnDestroy() {
            HeightState.RemoveSafely();
            ColorState.RemoveSafely();
        }
    }
}
