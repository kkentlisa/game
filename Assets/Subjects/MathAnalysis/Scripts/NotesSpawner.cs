using UnityEngine;
using System.Collections.Generic;

public class NotesSpawner : MonoBehaviour
{
    public MathLevelController levelController;
    public GameObject[] allNoteLocations;

    private List<int> availableIndices = new List<int>();
    private int activeOnScreen = 0;
    private int totalSpawnedCount = 0;
    private int goal;

    private RandomWanderingAI teacherAI;

    void Start()
    {
        if (levelController == null)
            levelController = Object.FindFirstObjectByType<MathLevelController>();

        teacherAI = Object.FindFirstObjectByType<RandomWanderingAI>();

        if (levelController != null)
        {
            levelController.ApplyDifficultySettings();
            goal = levelController.notesToCollect;
            Debug.Log("Спавнер: Цель уровня — " + goal);
        }

        availableIndices.Clear();
        for (int i = 0; i < allNoteLocations.Length; i++)
        {
            if (allNoteLocations[i] != null)
            {
                allNoteLocations[i].SetActive(false);
                availableIndices.Add(i);
            }
        }

        SpawnNextBatch();
    }

    void Update() { }

    public void SpawnNextBatch()
    {
        if (totalSpawnedCount < goal && activeOnScreen < 3 && availableIndices.Count > 0)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            int locationIndex = availableIndices[randomIndex];

            if (allNoteLocations[locationIndex] != null)
            {
                allNoteLocations[locationIndex].SetActive(true);

                if (teacherAI != null)
                    teacherAI.SetTargetNote(allNoteLocations[locationIndex].transform);

                NoteInteraction ni = allNoteLocations[locationIndex].GetComponent<NoteInteraction>();
                if (ni != null) ni.spawner = this;

                availableIndices.RemoveAt(randomIndex);
                activeOnScreen++;
                totalSpawnedCount++;
            }
        }
        else if (activeOnScreen == 0 && totalSpawnedCount >= goal)
        {
            WinGame();
        }
    }

    public void OnNoteCollected()
    {
        activeOnScreen--;

        if (teacherAI != null)
            teacherAI.ClearTargetNote();

        if (levelController != null)
        {
            levelController.notesToCollect--;
            if (levelController.notesToCollect <= 0)
            {
                WinGame();
                return;
            }
        }

        SpawnNextBatch();
    }

    void WinGame()
    {
        Debug.Log("<color=green>ПОБЕДА!</color>");
        if (LevelBridgeManager.instance != null)
        {
            LevelBridgeManager.instance.finishLevel(true);
        }
    }
}