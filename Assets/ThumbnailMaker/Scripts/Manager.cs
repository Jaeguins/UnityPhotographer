using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photograph;
using UnityEngine.UI;
namespace ThumbnailMaker {
    public class Manager : MonoBehaviour {
        public Material grayscale,silhouette;
        public Photographer photographer;
        public string folderPath;
        public float size;
        public List<ThumbnailObject> Objects = new List<ThumbnailObject>();
        public bool Starter;
        string realPath;
        public Image img;
        public void Start() {
            foreach (Transform t in transform) {
                ThumbnailObject tObj = t.GetComponent<ThumbnailObject>();
                if (tObj)
                    Objects.Add(tObj);
            }
        }
        public void ChangeMaterialObjectRecursive(GameObject obj,Material mat) {
            MeshRenderer t=obj.GetComponent<MeshRenderer>();
            if (t != null) {
                t.material=mat;
            }
            foreach(Transform k in obj.transform) {
                ChangeMaterialObjectRecursive(k.gameObject,mat);
            }
        }

        public void Update() {
            if (Starter) {
                Starter = false;
                StartCoroutine(Run());
            }
        }
        public IEnumerator Run() {
            foreach (ThumbnailObject t in Objects) {
                
                Texture2D texture = photographer.TakePicture(t.gameObject, t.camPosition, t.aimPosition, true, size);
                img.sprite=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(texture.width/2,texture.height/2));
                SaveFile(t.gameObject.name+"_colored", texture);
                yield return new WaitForEndOfFrame();

                ChangeMaterialObjectRecursive(t.gameObject,grayscale);

                texture = photographer.TakePicture(t.gameObject, t.camPosition, t.aimPosition, true, size);
                img.sprite=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(texture.width/2,texture.height/2));
                SaveFile(t.gameObject.name+"_garyscale", texture);
                yield return new WaitForEndOfFrame();

                ChangeMaterialObjectRecursive(t.gameObject,silhouette);

                texture = photographer.TakePicture(t.gameObject, t.camPosition, t.aimPosition, true, size);
                img.sprite=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(texture.width/2,texture.height/2));
                SaveFile(t.gameObject.name+"_silhouette", texture);
                yield return new WaitForEndOfFrame();

                Debug.Log(t.gameObject.name + " taked.");
            }
        }


        public void SaveFile(string filename, Texture2D data) {
            filename += ".png";
            realPath = Path.Combine(Application.persistentDataPath, folderPath);
            if (!Directory.Exists(realPath)) Directory.CreateDirectory(realPath);
            File.WriteAllBytes(Path.Combine(realPath,filename),data.EncodeToPNG());
        }

    }

}
