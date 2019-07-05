using System.Collections;
using UnityEngine;
using Custom;
public class ExampleMonitor : MonoBehaviour {
    private void Start() {
        StartCoroutine(routine());
    }
    IEnumerator routine() {
        while (true) {
            Debug.Log(IndividualRunner.Count + (IndividualRunner.IsAlive ? " running" : " not running"));
            yield return new WaitForSeconds(.25f);
        }
    }

}
