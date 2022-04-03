using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour {
    [SerializeField]
    NoiseMapGenerator noiseMapGeneration;
    [SerializeField]
    private MeshRenderer tileRenderer;
    [SerializeField]
    private MeshFilter meshFilter;
    [SerializeField]
    private MeshCollider meshCollider;
    [SerializeField]
    private float mapScale = 0.95f;

    void Update() {
        if (Input.GetKeyDown(KeyCode.G)) {
            GenerateTile();
        }
    }

    void GenerateTile() {
        Vector3[] meshVertices = this.meshFilter.mesh.vertices;
        int tileDepth = (int)Mathf.Sqrt(meshVertices.Length);
        int tileWidth = tileDepth;

        float[,] noiseMap = this.noiseMapGeneration.generateNoiseMap(tileDepth, tileWidth, this.mapScale);

        Texture2D tileTexture = this.buildTexture(noiseMap);
        this.tileRenderer.material.mainTexture = tileTexture;
    }

    private Texture2D buildTexture(float[,] noiseMap) {
        int tileDepth = noiseMap.GetLength(0);
        int tileWidth = noiseMap.GetLength(1);

        Color[] colorMap = new Color[tileDepth * tileWidth];
        for (int zIndex = 0; zIndex < tileDepth; zIndex++) {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++) {
                int colorIndex = zIndex * tileWidth + xIndex;
                float height = noiseMap[zIndex, xIndex];
                colorMap[colorIndex] = Color.Lerp(Color.black, Color.white, height);
                Debug.Log(Color.Lerp(Color.black, Color.white, height));
            }
        }
        Texture2D tileTexture = new Texture2D(tileWidth, tileDepth);
        tileTexture.wrapMode = TextureWrapMode.Clamp;
        tileTexture.SetPixels(colorMap);
        tileTexture.Apply();

        return tileTexture;
    }

}
