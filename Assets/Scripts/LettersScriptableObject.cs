using UnityEngine;

[CreateAssetMenu(fileName = "Letters", menuName = "Letters")]
public class LettersScriptableObject : ScriptableObject
{
    public GameObject[] letters = new GameObject[26];
}
