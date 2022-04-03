using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rendering : MonoBehaviour {

    // Use this for initialization
    void Start() {

        MeshFilter meshFilterc = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        MeshRenderer meshRenderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

    }

    // Update is called once per frame
    void Update() {

    }
}