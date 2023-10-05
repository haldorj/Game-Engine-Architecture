using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float maxViewDist = 450;
    public Transform viewer;

    public static Vector2 viewerPosition;
    private int chunksVisibleInViewDst;

    private readonly Dictionary<Vector2, TerrainChunk> _terrainChunks = new();
    private List<TerrainChunk> _terrainChunksVisibleLastUpdate = new();

    private void Start()
    {
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDist/GridMetrics.Scale);
    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks()
    {
        for (int i = 0; i < _terrainChunksVisibleLastUpdate.Count; i++)
        {
            _terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        _terrainChunksVisibleLastUpdate.Clear();
        
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x/GridMetrics.Scale);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y/GridMetrics.Scale);

        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (_terrainChunks.ContainsKey(viewedChunkCoord))
                {
                    _terrainChunks[viewedChunkCoord].UpdateTerrainChunk();
                    if (_terrainChunks[viewedChunkCoord].IsVisible())
                    {
                        _terrainChunksVisibleLastUpdate.Add(_terrainChunks[viewedChunkCoord]);
                    }
                }
                else
                {
                    _terrainChunks.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, GridMetrics.Scale, transform));
                }
            }
        }
    }
    
    public class TerrainChunk
    {
        private readonly GameObject _meshObject;
        private readonly Vector2 _position;
        private Bounds _bounds;
        private Mesh _terrainMesh;
        public TerrainChunk(Vector2 coord, int size, Transform parent)
        {
            _position = coord * size;
            _bounds = new Bounds(_position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(_position.x, 0, _position.y);
            
            _meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            _meshObject.transform.position = positionV3;
            _meshObject.transform.localScale = Vector3.one * size / 10f;
            _meshObject.transform.parent = parent;
            SetVisible(false);
        }

        public void UpdateTerrainChunk()
        {
            float viewerDistanceFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(viewerPosition));
            bool visible = viewerDistanceFromNearestEdge <= maxViewDist;
            SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            _meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return _meshObject.activeSelf;
        }
    }
}
