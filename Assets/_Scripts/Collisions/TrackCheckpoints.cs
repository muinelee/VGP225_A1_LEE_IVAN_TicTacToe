using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
    private List<CheckpointSingle> checkpointSingleList;
    private int nextCheckpointSingleIndex;

    private void Awake()
    {
        Transform checkpointsTransform = transform.Find("Checkpoints");

        checkpointSingleList = new List<CheckpointSingle>();

        foreach (Transform checkpointSingleTransform in checkpointsTransform)
        {
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();
            checkpointSingle.SetTrackCheckpoints(this);
            checkpointSingleList.Add(checkpointSingle);
        }

        nextCheckpointSingleIndex = 0;
    }

    public void PlayerThroughCheckpoint(CheckpointSingle checkpointSingle)
    {
        int checkpointIndex = checkpointSingleList.IndexOf(checkpointSingle);

        if (checkpointSingleList.IndexOf(checkpointSingle) == nextCheckpointSingleIndex)
        {
            GameManager.Instance.PlayerThroughCheckpoint(checkpointIndex);
            nextCheckpointSingleIndex = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count;

            // Check if the player has passed the last checkpoint in the list
            if (nextCheckpointSingleIndex == 0)
            {
                GameManager.Instance.LapCompleted();
            }
        }
        else
        {
            Debug.Log("Wrong checkpoint");
        }
    }

    public void SetTotalLaps(int laps)
    {
        GameManager.Instance.totalLaps = laps;
    }
}
