using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MenuData", menuName = "Menu", order = 1)]

public class MenuDataSO : ScriptableObject {

	[Header("Character Portraits")]
    [SerializeField] Sprite cyborgSprite;
    [SerializeField] Sprite ninjaSprite;
    [SerializeField] Sprite pirateSprite;
    [SerializeField] Sprite witchHunterSprite;
    [SerializeField] Sprite nullSprite;

    public Sprite GetCharacterPortrait(int c)
    {
        switch (c)
        {
            case 0:
                return cyborgSprite;
            case 1:
                return ninjaSprite;
            case 2:
                return pirateSprite;
            case 3:
                return witchHunterSprite;
            default:
                return nullSprite;
        }
    }
}
