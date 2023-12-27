using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class Matrix : MonoBehaviour
{
    public int size = 10;
    public int gridSize = 4;
    public float gridSpacing = 0.8f;
    public float flipTime = 3f;
    public int flipCount = 5;

    private List<GameObject> grids = new List<GameObject>();
    private List<int> randomList = new List<int>();

    private string gridGray = "#4A4A4A";
    private string gridBlue = "#279BB7";
    private float timer = 0f;
    private bool flipOn = false;

    void Start()
    {
        gameObject.transform.position = Vector3.zero;
        gameObject.transform.localScale = Vector3.one * size;

        DrawGrid();
        for (int i = 0; i < this.grids.Count; i++)
        {
            this.SetGridColor(gridGray, i);
        }
    }

    private void Update()
    {
        if (timer < flipTime)
        {
            timer += Time.deltaTime;
        }
        else
        {
            if (flipOn)
            {
                for (int i = 0; i < this.randomList.Count; i++)
                {
                    this.SetGridColor(gridGray, this.randomList[i]);
                }
            }
            else
            {
                this.RandomNumber();
                for (int i = 0; i < this.randomList.Count; i++)
                {
                    this.SetGridColor(gridBlue, this.randomList[i]);
                }
            }
            flipOn = !flipOn;
            timer = 0f;
        }
    }

    public void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D collider = gameObject.GetComponent<Collider2D>();
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 localMousePos = gameObject.transform.InverseTransformPoint(mouseWorldPos) * size;

            Vector2 min = collider.bounds.min;
            Vector2 max = collider.bounds.max;

            float cellSpacing = gridSpacing / (float)gridSize;
            float cellSize = (max.x - min.x - (gridSize + 1) * cellSpacing) / gridSize;

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    float cellX = min.x + cellSpacing + j * (cellSize + cellSpacing);
                    float cellY = -min.y - cellSpacing - (i + 1) * (cellSize + cellSpacing);

                    Vector2 cellMin = new Vector2(cellX, cellY);
                    Vector2 cellMax = new Vector2(cellX + cellSize, cellY + cellSize);

                    int index = gridSize * i + j;

                    /*Debug.Log("Index: " + index + ", Cell Min: " + cellMin.ToString());
                    Debug.Log("Index: " + index + ", Cell Max: " + cellMax.ToString());
                    Debug.Log("Mouse: " + localMousePos.ToString());*/

                    if (cellMin.x <= localMousePos.x && cellMin.y <= localMousePos.y && cellMax.x >= localMousePos.x && cellMax.y >= localMousePos.y)
                    {
                        Debug.Log("Mouse Clicked At index: " + index);
                        return;
                    }
                }
            }
        }
    }

    void DrawGrid()
    {
        Vector2 bigSquareSize = new Vector2(size, size);

        float cellSpacing = gridSpacing / (float)gridSize;
        float cellSize = (bigSquareSize.x - (gridSize + 1) * cellSpacing) / gridSize;

        GameObject cellPrefab = GameObject.Find("Cell");
        cellPrefab.transform.localScale = new Vector3(cellSize, cellSize, 1f);

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                float cellLeft = -bigSquareSize.x / 2f + cellSize / 2f + cellSpacing;
                float cellTop = bigSquareSize.y / 2f - cellSize / 2f - cellSpacing;
                float cellX = cellLeft + j * (cellSize + cellSpacing);
                float cellY = cellTop - i * (cellSize + cellSpacing);

                Vector3 cellPosition = new Vector3(cellX, cellY, 0f);

                GameObject obj = Instantiate(cellPrefab, cellPosition, Quaternion.identity);

                grids.Add(obj);
            }
        }

        cellPrefab.SetActive(false);
    }

    void SetGridColor(string color, int index)
    {
        SpriteRenderer renderer = this.grids[index].GetComponent<SpriteRenderer>();
        Color gridColor;
        if (!ColorUtility.TryParseHtmlString(color, out gridColor))
        {
            return;
        }
        renderer.color = gridColor;
    }

    void RandomNumber()
    {
        this.randomList.Clear();
        for (int i = 0; i < flipCount; i++)
        {
            int rand;
            do
            {
                rand = UnityEngine.Random.Range(0, this.grids.Count);
            }
            while (this.randomList.Contains(rand));

            this.randomList.Add(rand);
        }
    }
}
