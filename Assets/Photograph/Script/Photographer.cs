using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photograph {

    public class Photographer : MonoBehaviour {
        [SerializeField]
        private Color chromaKey,altColor;
        [SerializeField]
        [Header("Layer to take picture")]
        private string mask;
        private Queue<string> targetLayer=new Queue<string>();
        [SerializeField]
        [Header("Object of Camera to use")]
        public GameObject CameraObject;
        private void SetLayerRecursively(GameObject target) {
            if(targetLayer.Count!=0)Debug.LogError("Target mask stack already exists, It'll be deleted");
            targetLayer.Clear();
            SetLayerRecursivelyInternal(target);
        }
        private void SetLayerRecursivelyInternal(GameObject target) {
            targetLayer.Enqueue(LayerMask.LayerToName(target.layer));
            target.layer=LayerMask.NameToLayer(mask);
            foreach(Transform t in target.transform)
                SetLayerRecursivelyInternal(t.gameObject);
        }
        private void ResetLayerRecursively(GameObject target) {
            if(targetLayer.Count==0){
                Debug.LogError("Layer mask stack not found.");
                return; 
            }
            ResetLayerRecursivelyInternal(target);
        }
        private void ResetLayerRecursivelyInternal(GameObject target) {
            target.layer=LayerMask.NameToLayer(targetLayer.Dequeue());
            foreach(Transform t in target.transform)
                ResetLayerRecursivelyInternal(t.gameObject);
        }
        /// <summary>
        /// Taking picture of GameObject on local position at specific location.
        /// </summary>
        /// <param name="target">GameObject which you want to take photo.</param>
        /// <param name="camPosition">World location of camera from target's pivot.</param>
        /// <param name="aimPosition">World location of camera's aim from target's pivot.</param>
        /// <param name="isOrtho">Check if photo is orthographic.</param>
        /// <param name="size">Size of camera if photo is orthographic.</param>
        /// <returns></returns>
        public Texture2D TakePicture(GameObject target, Vector3 camPosition, Vector3 aimPosition,bool isOrtho,float size=1f) {
            CameraObject.transform.parent = target.transform;
            CameraObject.transform.position = camPosition;
            CameraObject.transform.LookAt(target.transform.position+aimPosition);
            SetLayerRecursively(target);
            Camera cam = CameraObject.GetComponent<Camera>();
            cam.backgroundColor=chromaKey;
            cam.cullingMask = LayerMask.GetMask(mask);
            cam.orthographic=isOrtho;
            if(isOrtho)cam.orthographicSize=size;
            int tmpLayer = target.layer;
            RenderTexture nowRenderTexture = RenderTexture.active;
            RenderTexture rend =cam.targetTexture= new RenderTexture(cam.pixelWidth, cam.pixelHeight,32);
            RenderTexture.active = rend;
            cam.Render();
            Texture2D tempRet = new Texture2D(rend.width, rend.height, TextureFormat.RGB24, false);
            tempRet.name = "temp";
            rend.name = "tempRend";
            tempRet.ReadPixels(new Rect(0, 0, rend.width, rend.height), 0, 0);
            tempRet.Apply();
            Texture2D ret=new Texture2D(tempRet.width,tempRet.height,TextureFormat.RGBA32,false);
            for(int i = 0; i < tempRet.width; i++) {
                for(int j = 0; j < tempRet.height; j++) {
                    Color c=tempRet.GetPixel(i,j);
                    if (c.Equals(chromaKey)) {
                        ret.SetPixel(i,j,altColor);
                    }
                    else {
                        ret.SetPixel(i,j,c);
                    }
                }
            }
            ret.Apply();
            ResetLayerRecursively(target);
            RenderTexture.active = nowRenderTexture;
            cam.targetTexture = rend = null;
            CameraObject.transform.parent=transform;
            return ret;
        }
    }
}