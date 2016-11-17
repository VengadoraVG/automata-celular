using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {
    int i=0;

    void Start () {
        StartCoroutine(f());
        Debug.Log("starting...");
    }
    
    void Update () {
	
    }

    IEnumerator f () {
        i++;
        Debug.Log(i);
        yield return new WaitForSeconds(1);
        StartCoroutine(f());
    }
}
