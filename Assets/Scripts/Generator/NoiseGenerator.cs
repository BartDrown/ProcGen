using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour {
    GameObject noiseMap;
    GameObject colorMap;

    [SerializeField]
    bool liveReload = false;

    [SerializeField]
    int width = 10, height = 10;

    [SerializeField]
    float scale = 4f;

    [SerializeField]
    float heightMultiplier = 1f;
    [SerializeField]
    private AnimationCurve heightCurve;
    [SerializeField]
    TerrainType[] terrainTypes;

    void Start() {
        this.noiseMap = new GameObject();
        this.noiseMap.AddComponent<MeshRenderer>();
        this.noiseMap.AddComponent<MeshFilter>();

        this.colorMap = new GameObject();
        this.colorMap.transform.position = new Vector3(0f, 0f, -5f);
        this.colorMap.AddComponent<MeshRenderer>();
        this.colorMap.AddComponent<MeshFilter>();
        this.colorMap.AddComponent<MeshCollider>();
    }

    // Update is called once per frame
    void Update() {
        if (liveReload) {
            this.generateTerrain();
        }

    }

    public void generateTerrain(){
        this.generateNoiseMap();
        this.generateColorMap();
    }

    void generateNoiseMap() {
        Mesh mesh = RendererUtils.Generate(this.width, this.height);
        this.noiseMap.GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] meshVertices = mesh.vertices;
        int tileDepth = (int)Mathf.Sqrt(meshVertices.Length);
        int tileWidth = (int)Mathf.Sqrt(meshVertices.Length);

        float offsetX = this.noiseMap.transform.position.x;
        float offsetZ = this.noiseMap.transform.position.y;

        float[,] noiseMap = RendererUtils.generateNoiseMap(tileDepth, tileWidth, scale, offsetX, offsetZ);

        this.noiseMap.GetComponent<MeshRenderer>().material.mainTexture = RendererUtils.buildTextureNoise(noiseMap);
    }

    void generateColorMap() {
        Mesh mesh = RendererUtils.Generate(this.width, this.height);
        this.colorMap.GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] meshVertices = mesh.vertices;
        int tileDepth = (int)Mathf.Sqrt(meshVertices.Length);
        int tileWidth = (int)Mathf.Sqrt(meshVertices.Length);

        float offsetX = this.colorMap.transform.position.x;
        float offsetZ = this.colorMap.transform.position.y;

        float[,] noiseMap = RendererUtils.generateNoiseMap(tileDepth, tileWidth, scale, offsetX, offsetZ);

        this.colorMap.GetComponent<MeshRenderer>().material.mainTexture = RendererUtils.buildTextureColor(noiseMap, terrainTypes);

        RendererUtils.updateMeshVertices(noiseMap, colorMap, this.heightMultiplier, this.heightCurve);
    }

}
