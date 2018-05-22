using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "GameData", order = 2)]

public class NPersistentGameDataSO : ScriptableObject {

    [Header("Player Setup Data")]
    [SerializeField] public int[] playerCharacterChoices;
    [SerializeField] public NCharacterTemplate[] characterTemplates;

    [Header("Match Data")]
    [SerializeField] public int neededWins;
    [SerializeField] public int[] playerWins;
}
