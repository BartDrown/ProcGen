using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    private int mapWidthInTiles, mapDepthInTiles;
    [SerializeField]
    private GameObject tilePrefab;

    void Start(){
        this.generateMap();
    }

    void generateMap(){
        int tileWidth = (int)tilePrefab.GetComponent<NoiseGenerator>().width;
        int tileDepth = (int)tilePrefab.GetComponent<NoiseGenerator>().height;

        for (int xTileIndex = 0; xTileIndex < this.mapWidthInTiles; xTileIndex++) {
            for (int zTileIndex = 0; zTileIndex < this.mapDepthInTiles; zTileIndex++) {
                Vector3 tilePosition = new Vector3(
                this.gameObject.transform.position.x + xTileIndex * tileWidth, 
                this.gameObject.transform.position.y + zTileIndex * tileDepth, 
                this.gameObject.transform.position.z );
                
                GameObject tile = Instantiate (tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                tile.transform.SetParent(this.gameObject.transform);

            }
        }
    }
}
