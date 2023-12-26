using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matrix : MonoBehaviour
{
    public int size = 10;
    public int gridSize = 4;
    public float gridSpacing = 0.1f;

    void Start()
    {
        gameObject.transform.position = Vector3.zero;
        gameObject.transform.localScale = Vector3.one * size;

        DrawGrid();
    }

    void Update()
    {
    }

    void DrawGrid()
    {
        float gridLeft = -size/2f + gridSpacing;
        float gridRight = size/2f - gridSpacing;
        float gridTop = -size/2f + gridSpacing;
        float gridBottom = size/2f - gridSpacing;

        float gridWidth = (gridRight - gridLeft - (gridSize - 2) * gridSpacing) / gridSize;

        GameObject grid = GameObject.Find("Cell");
        grid.transform.position = Vector3.zero;
        grid.transform.localScale = Vector3.one * gridWidth;

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Vector3 position = new Vector3(gridLeft + gridWidth / 2f + gridWidth * i + gridSpacing * i, gridTop + gridWidth / 2f + gridWidth * j + gridSpacing * j, 0);
                Instantiate(grid, position, grid.transform.rotation);
            }
        }

        grid.SetActive(false);
    }
}
