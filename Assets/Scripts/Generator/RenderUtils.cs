using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererUtils {
    static public Mesh Generate(int xSize, int ySize) {

        Mesh mesh = new Mesh();
        mesh.name = "Procedural Grid";

        Vector3[] vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, y = 0; y <= ySize; y++) {
            for (int x = 0; x <= xSize; x++, i++) {
                vertices[i] = new Vector3(x, y);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
                tangents[i] = tangent;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;

        int[] triangles = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
            for (int x = 0; x < xSize; x++, ti += 6, vi++) {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    static public Texture2D buildTextureNoise(float[,] noiseMap) {
        int tileDepth = noiseMap.GetLength(0);
        int tileWidth = noiseMap.GetLength(1);

        Color[] colorMap = new Color[tileDepth * tileWidth];
        for (int zIndex = 0; zIndex < tileDepth; zIndex++) {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++) {
                int colorIndex = zIndex * tileWidth + xIndex;
                float height = noiseMap[zIndex, xIndex];
                colorMap[colorIndex] = Color.Lerp(Color.black, Color.white, height);
            }
        }
        Texture2D tileTexture = new Texture2D(tileWidth, tileDepth);
        tileTexture.wrapMode = TextureWrapMode.Clamp;
        tileTexture.SetPixels(colorMap);
        tileTexture.Apply();

        return tileTexture;
    }

    static public float[,] generateNoiseMap(int mapDepth, int mapWidth, float scale) {
        float[,] noiseMap = new float[mapDepth, mapWidth];

        for (int zIndex = 0; zIndex < mapDepth; zIndex++) {
            for (int xIndex = 0; xIndex < mapWidth; xIndex++) {
                float sampleX = xIndex / scale;
                float sampleZ = zIndex / scale;

                float noise = 1 * Mathf.PerlinNoise(sampleX * 1, sampleZ * 1)
                + 0.5f * Mathf.PerlinNoise(sampleX * 2, sampleZ * 2)
                + 0.25f * Mathf.PerlinNoise(sampleX * 4, sampleZ * 4);

                noiseMap[zIndex, xIndex] = noise / (1 + 0.5f + 0.25f);
            }
        }
        return noiseMap;
    }

    static public Texture2D buildTextureColor(float[,] heightMap, TerrainType[] terrainTypes) {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);
        Color[] colorMap = new Color[tileDepth * tileWidth];
        for (int zIndex = 0; zIndex < tileDepth; zIndex++) {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++) {
                // transform the 2D map index is an Array index
                int colorIndex = zIndex * tileWidth + xIndex;
                float height = heightMap[zIndex, xIndex];
                // choose a terrain type according to the height value
                TerrainType terrainType = evaluateTerrainType(height, terrainTypes);
                // assign the color according to the terrain type
                colorMap[colorIndex] = terrainType.color;
            }
        }
        // create a new texture and set its pixel colors
        Texture2D tileTexture = new Texture2D(tileWidth, tileDepth);
        tileTexture.wrapMode = TextureWrapMode.Clamp;
        tileTexture.SetPixels(colorMap);
        tileTexture.Apply();
        return tileTexture;
    }

    static TerrainType evaluateTerrainType(float height, TerrainType[] terrainTypes) {
        // for each terrain type, check if the height is lower than the one for the terrain type
        foreach (TerrainType terrainType in terrainTypes) {
            // return the first terrain type whose height is higher than the generated one
            if (height < terrainType.height) {
                return terrainType;
            }
        }
        return terrainTypes[terrainTypes.Length - 1];
    }

    static public void updateMeshVertices(float[,] heightMap, GameObject meshObject, float heightMultiplier, AnimationCurve heightCurve) {
        MeshFilter meshFilter = meshObject.GetComponent<MeshFilter>();
        MeshCollider meshCollider = meshObject.GetComponent<MeshCollider>();

        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);
        Vector3[] meshVertices = meshFilter.mesh.vertices;
        // iterate through all the heightMap coordinates, updating the vertex index
        int vertexIndex = 0;
        for (int zIndex = 0; zIndex < tileDepth; zIndex++) {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++) {
                float height = heightMap[zIndex, xIndex];
                Vector3 vertex = meshVertices[vertexIndex];
                // change the vertex Y coordinate, proportional to the height value
                meshVertices[vertexIndex] = new Vector3(vertex.x, vertex.y, heightCurve.Evaluate(height) * heightMultiplier);
                vertexIndex++;
            }
        }
        // update the vertices in the mesh and update its properties
        meshFilter.mesh.vertices = meshVertices;
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();
        // update the mesh collider
        meshCollider.sharedMesh = meshFilter.mesh;

    }


}