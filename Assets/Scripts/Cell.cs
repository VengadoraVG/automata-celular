using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cell : MonoBehaviour {
    public int QtyRequirements;
    public List<string> Requirements;
    public int QtyProductions;
    public List<string> Productions;
    public int Life;
    public Vector2 Position;
    public GameObject dad;

    public Controller _controller;
    public int RequirementsMet;
    public bool WillSplit;
    public bool WillMove;
    public float MutationProbability = 0.1f;
    public float OnBornMutationProbability = 0.2f;

    private int _maxLife;

    void Start () {
        _controller = GameObject.FindGameObjectWithTag("GameController").
            GetComponent<Controller>();
        Life = _maxLife = _controller.MaxLife;

        if (Requirements.Count == 0) {
            for (int i=0; i<QtyRequirements; i++) {
                Requirements.Add(_controller.GetDifferentRandomRequirement(Requirements));
                // Requirements.Add(_controller.GetRandomRequirement());
            }
        } else {
            Mutate(OnBornMutationProbability, Requirements);
        }

        if (Productions.Count == 0) {
            for (int i=0; i<QtyProductions; i++) {
                // Productions.Add(_controller.GetRandomRequirement());
                Productions.Add(_controller.GetDifferentRandomRequirement(Productions));
            }
        } else {
            Mutate(MutationProbability, Productions);
        }

        UpdateRender();
        StartCoroutine(NextGeneration());
    }

    void Update () {
        if (Life == 0) {
            this.gameObject.SetActive(false);
            Destroy(this.gameObject);
        }
    }

    IEnumerator NextGeneration () {
        yield return new WaitForSeconds(_controller.IdleTime);
        if (this.gameObject.activeSelf) {
            // yield return new WaitForSeconds(0);
            UpdateAttributes();
            UpdateMovement();
            UpdateSplit();
            StartCoroutine(NextGeneration());
        }
    }

    public void UpdateAttributes () {
        CheckRequirements();

        WillSplit = RequirementsMet >=
            Requirements.Count - _controller.ReproductionTolerancy;

        if (RequirementsMet == Requirements.Count) {
            Life = Mathf.Min(Life+1, _maxLife);
            WillMove = false;
        } else {
            if (RequirementsMet < Requirements.Count-_controller.PrescindibleRequirements) {
                Life = Mathf.Max(Life - 1, 0);
            }

            WillMove = true;
            if (Life == 0) {
                this.gameObject.SetActive(false);
                Destroy(this.gameObject);
            }
        }
    }

    public void Mutate (float probability, List<string> Resources) {
        for (int i=0; i<Resources.Count; i++) {
            if (CanMutate(probability)) {
                Resources[i] = _controller.GetDifferentRandomRequirement(Resources);
            }
        }
    }

    public bool CanMutate (float probability) {
        return Random.Range(0, 1.0f) < probability;
    }

    public void CheckRequirements () {
        RequirementsMet = 0;
        foreach (string r in Requirements) {
            if (hasNeighbourWith(r)) {
                RequirementsMet++;
            }
        }
    }

    public void UpdateMovement () {
        if (WillMove) {
            List<Vector2> empty = GetEmpties();

            if (empty.Count > 0) {
                Vector2 newPosition = empty[Random.Range(0, empty.Count)];
                _controller.M[(int)Position.x, (int)Position.y] = null;
                Position = newPosition;
                _controller.M[(int)Position.x, (int)Position.y] = this;
                UpdateRender();
            }
        }
    }

    public void UpdateSplit () {
        if (WillSplit) {
            List<Vector2> empty = GetEmpties();

            if (empty.Count > 0) {
                Vector2 sonPosition = empty[Random.Range(0, empty.Count)];
                GameObject son = Instantiate(this.gameObject);
                Cell cell = son.GetComponent<Cell>();
                cell.Position = sonPosition;
                _controller.M[(int)sonPosition.x, (int)sonPosition.y] = cell;
                cell.dad = this.gameObject;
            }
        }
    }

    public List<Vector2> GetEmpties () {
        List<Vector2> empty = new List<Vector2>();

        for (int i=-1; i<2; i++) {
            for (int j=-1; j<2; j++) {
                try {
                    if (_controller.M[(int)Position.x + i, (int)Position.y + j] == null) {
                        empty.Add(new Vector2(Position.x + i, Position.y + j));
                    }
                } catch {}
            }
        }

        return empty;
    }

    public bool hasNeighbourWith (string requirement) {
        for (int i=-1; i<2; i++) {
            for (int j=-1; j<2; j++) { // counts self too.
                try {
                    if ((i != 0 && j != 0) &&
                        _controller.M[(int)Position.x+i, (int)Position.y+j]
                        .Produces(requirement)) {
                        return true;
                    }
                } catch {}
            }
        }
        return false;
    }

    public bool Produces (string requirement) {
        foreach (string p in Productions) {
            if (p == requirement) {
                return true;
            }
        }
        return false;
    }

    public void UpdateRender () {
        transform.position = _controller.BottomLeftCorner + Position * _controller.Offset;
    }
}
