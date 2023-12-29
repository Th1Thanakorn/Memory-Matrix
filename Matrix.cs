using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Matrix : MonoBehaviour
{
    public int size = 14;
    public int gridSize = 5;
    public float gridSpacing = 0.8f;
    public float flipTime = 2f;
    public int flipCount = 5;

    private List<GameObject> grids = new List<GameObject>();
    private List<GameObject> wrongCopy = new List<GameObject>();
    private List<int> randomList = new List<int>();
    private List<int> clicked = new List<int>();

    private string gridGray = "#4A4A4A";
    private string gridBlue = "#279BB7";
    private string gridRed = "#FF4C4C";
    private float timer = -2.5f;
    private bool doneRandom = false;
    private bool gameShowed = false;
    private bool playerMode = false;
    private bool gameFailed = false;
    private bool wait = false;
    private int failedCount = 0;
    private int score = 0;

    private GameObject cell;
    private GameObject correct;
    private GameObject wrong;
    public GameObject scoreText;

    void Start()
    {
        this.cell = GameObject.Find("Cell");
        this.correct = GameObject.Find("Correct");
        this.wrong = GameObject.Find("Wrong");

        DefaultScale();
        TableStart();
    }

    private void Update()
    {
        if (!playerMode)
        {
            if (Mathf.Abs(timer + 1f) <= 0.1f && !this.doneRandom)
            {
                // Restore clicked
                foreach (int clicked in this.clicked)
                {
                    this.SetGridColor(gridGray, clicked);
                    this.FlipCell(clicked, false);
                }
                foreach (GameObject wrong in this.wrongCopy)
                {
                    Destroy(wrong);
                }

                this.clicked.Clear();
                this.wrongCopy.Clear();

                RandomNumber();

                correct.SetActive(false);
                wrong.SetActive(false);

                this.doneRandom = true;
            }

            if (Mathf.Abs(timer) <= 0.1f && !this.gameShowed)
            {
                foreach (var index in this.randomList)
                {
                    SetGridColor(gridBlue, index);
                    FlipCell(index, false);

                    PlaySound(0);
                }

                this.gameShowed = true;
            }

            if (timer < flipTime)
            {
                timer += Time.deltaTime;
            }
            else
            {
                foreach (var index in this.randomList)
                {
                    SetGridColor(gridGray, index);
                    FlipCell(index, false);

                    PlaySound(0);
                }
                this.playerMode = true;
                timer = 0f;
            }
        }
    }

    public void OnMouseDown()
    {
        if (this.playerMode && Input.GetMouseButtonDown(0))
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

                    if (cellMin.x <= localMousePos.x && cellMin.y <= localMousePos.y && cellMax.x >= localMousePos.x && cellMax.y >= localMousePos.y)
                    {
                        this.GridClicked(index);
                        return;
                    }
                }
            }
        }
    }

    void DefaultScale()
    {
        correct.transform.position = Vector3.zero;
        correct.transform.localScale *= size / 1.5f;
        correct.SetActive(false);

        wrong.transform.position = Vector3.zero;
        wrong.transform.localScale *= size / 1.5f;
        wrong.SetActive(false);
    }

    void TableStart()
    {
        this.cell.SetActive(true);

        gameObject.transform.position = Vector3.zero;
        gameObject.transform.localScale = Vector3.one * size;

        DrawGrid();
        for (int i = 0; i < this.grids.Count; i++)
        {
            this.SetGridColor(gridGray, i);
        }
    }

    void DrawGrid()
    {
        Vector2 bigSquareSize = new Vector2(size, size);

        float cellSpacing = gridSpacing / (float)gridSize;
        float cellSize = (bigSquareSize.x - (gridSize + 1) * cellSpacing) / gridSize;

        GameObject cellPrefab = this.cell;
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

    void GridClicked(int index)
    {
        if (!clicked.Contains(index))
        {
            clicked.Add(index);

            // Wrong Selection
            if (!this.randomList.Contains(index))
            {
                this.SetGridColor(gridRed, index);
                this.gameFailed = true;
                this.failedCount++;

                int i = index / this.gridSize;
                int j = index % this.gridSize;

                GameObject obj = Instantiate(this.wrong, this.getCellMiddle(i, j), Quaternion.identity);
                obj.SetActive(true);

                this.wrongCopy.Add(obj);
            }
            else
            {
                this.SetGridColor(gridBlue, index);
                this.FlipCell(index, true);
            }

            if (this.failedCount > 2)
            {
                // Flip all correct grid
                foreach (int correct in this.randomList)
                {
                    if (!clicked.Contains(correct))
                    {
                        this.clicked.Add(correct);
                        this.SetGridColor(gridBlue, correct);
                        this.FlipCell(correct, false);
                    }
                }
                PlaySound(0);
                this.TriggerRestart();
                return;
            }

            bool allCorrect = true;
            foreach (int correct in this.randomList)
            {
                if (!clicked.Contains(correct))
                {
                    allCorrect = false;
                    break;
                }
            }

            if (allCorrect)
            {
                if (!this.gameFailed)
                {
                    this.correct.SetActive(true);
                }
                this.TriggerRestart();
                this.CalculateScore();
            }
        }
    }

    void FlipCell(int index, bool playSound)
    {
        Animator animator = this.grids[index].GetComponent<Animator>();
        animator.SetTrigger("Flip");

        if (playSound)
        {
            PlaySound(index);
        }
    }

    void TriggerRestart()
    {
        this.timer = -2.5f;
        this.gameFailed = false;
        this.failedCount = 0;
        this.randomList.Clear();
        this.gameShowed = false;
        this.doneRandom = false;
        this.playerMode = false;
    }

    IEnumerator waitFor(int secs)
    {
        yield return new WaitForSeconds(1);
    }

    Vector2 getCellMiddle(int row, int column)
    {
        Collider2D collider = gameObject.GetComponent<Collider2D>();

        Vector2 min = collider.bounds.min;
        Vector2 max = collider.bounds.max;

        float cellSpacing = gridSpacing / (float)gridSize;
        float cellSize = (max.x - min.x - (gridSize + 1) * cellSpacing) / gridSize;

        float cellX = min.x + cellSpacing + column * (cellSize + cellSpacing);
        float cellY = -min.y - cellSpacing - (row + 1) * (cellSize + cellSpacing);

        Vector2 cellMin = new Vector2(cellX, cellY);
        Vector2 cellMax = new Vector2(cellX + cellSize, cellY + cellSize);

        return new Vector2((cellMin.x + cellMax.x) / 2f, (cellMin.y + cellMax.y) / 2f);
    }

    void PlaySound(int index)
    {
        AudioSource source = this.grids[index].GetComponent<AudioSource>();
        source.volume = 2f;
        source.Play();
    }

    void CalculateScore()
    {
        int scorePerFlip = (int) (Math.Round((float) this.flipCount / this.grids.Count, 2) * 1000f);
        int wrong = (int) Mathf.Pow(2, this.failedCount) * scorePerFlip;
        int score = this.flipCount * scorePerFlip - wrong;

        this.score += score;

        Text text = this.scoreText.GetComponent<Text>();
        text.text = "";
        text.text = "Score: " + this.score;
    }

    void GenerateNew()
    {
        bool flag = this.flipCount <= this.grids.Count / 2;

        if (!flag)
        {
            this.gridSize++;
            this.flipCount += UnityEngine.Random.Range(1, 3);

            foreach (GameObject grid in this.grids)
            {
                Destroy(grid);
            }

            this.grids.Clear();
            this.clicked.Clear();

            this.wait = true;
        }
        else
        {
            this.flipCount++;
        }
    }
}
