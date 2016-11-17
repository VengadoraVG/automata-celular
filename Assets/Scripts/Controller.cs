using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Controller : MonoBehaviour {
    public string AllRequirements = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public Cell[,] M;
    public int rows;
    public int cols;
    public GameObject CellPrototype;
    public int QtyCellChoises;
    public List<Cell> CellChoises;
    public int ReproductionTolerancy = 1;

    public float InitialLifeProbability = .5f;
    public float Offset;
    public float IdleTime;
    public Vector2 BottomLeftCorner;
    public int MaxLife;
    public int PrescindibleRequirements = 3;
    private List<Color> _colorChoises = new List<Color>();

    void Start () {
        BottomLeftCorner = CellPrototype.transform.position;
        M = new Cell[rows,cols];
        InitializeColorChoises();

        for (int i=0; i<QtyCellChoises; i++) {
            Cell cell;
            SpriteRenderer renderer;
            cell = Instantiate(CellPrototype).GetComponent<Cell>();
            renderer = cell.GetComponent<SpriteRenderer>();
            renderer.color = _colorChoises[i];
            CellChoises.Add(cell);
        }

        for (int i=0; i<rows; i++) {
            for (int j=0; j<cols; j++) {
                Cell cell;
                SpriteRenderer sr;
                if (Random.Range(0, 1.0f) < InitialLifeProbability) {
                    cell = Instantiate(CellChoises[Random.Range(0, CellChoises.Count)])
                        .GetComponent<Cell>();
                    cell.Position = new Vector2(i, j);
                    cell.gameObject.SetActive(true);
                }
            }
        }
    }
    
    void Update () {
    }

    public string GetRandomRequirement () {
        return AllRequirements[Random.Range(0, AllRequirements.Length)] + "";
    }

    public string GetDifferentRandomRequirement (List<string> used) {
        while (true) {
            string r = GetRandomRequirement();
            if (false == used.Contains(r)) {
                return r;
            }
        }
    }

    public void InitializeColorChoises () {
        Color[] ColorChoises =
            { new Color(1,1,1),
              new Color(1,1,0), new Color(1,0,1), new Color(0,1,1),
              new Color(1,0,0), new Color(0,1,0), new Color(0,0,1),
              new Color(0,0,0) };

        for (int i=0; i<ColorChoises.Length; i++) {
            for (int j=1; j<=4; j++) {
                Color c = ColorChoises[i] / j;
                _colorChoises.Add(new Color(c.r, c.g, c.b, 1));
            }
        }
    }
}
