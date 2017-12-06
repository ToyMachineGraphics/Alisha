using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclePuzzleManager : MonoBehaviour
{
    public GameObject UnlockImg;
    public GameObject Cue;
    private CirclePuzzleBehavior[] Puzzles;

    // Use this for initialization
    private void Start()
    {
        Puzzles = GameObject.FindObjectsOfType<CirclePuzzleBehavior>();
    }

    // Update is called once per frame
    private void Update()
    {
        for (int i = 0; i < Puzzles.Length; i++)
        {
            if (Puzzles[i].CurrentSplits == 0 && i == Puzzles.Length - 1)
                Unlock();
            else if (Puzzles[i].CurrentSplits != 0)
            {
                UnlockImg.SetActive(false);
                break;
            }
        }
    }

    public void PuzzleTrigger(bool Active)
    {
        foreach (var puzzle in Puzzles)
        {
            puzzle.IsActive = Active;
        }
        if (!Active)
        {
            HandBehavior.Instance.Back();
            Cue.SetActive(false);
        }
        else
        {
            Cue.SetActive(true);
        }
    }

    private void Unlock()
    {
        UnlockImg.SetActive(true);
    }
}