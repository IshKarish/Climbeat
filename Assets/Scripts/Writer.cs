using UnityEngine;

public class Writer : MonoBehaviour
{
    private string abcStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private LettersScriptableObject lettersScriptableObject;
    
    public Writer(LettersScriptableObject lettersScriptableObject)
    {
        this.lettersScriptableObject = lettersScriptableObject;
    }
    
    public void Write(string text)
    {
        GameObject parent = Instantiate(new GameObject());
        parent.name = text + " (Text)";
        
        string txtUpper = text.ToUpper();

        float half = 0;
        if (text.Length % 2 == 0) half = (text.Length / 2) - 2;
        else half = (text.Length / 2) - 1;
            
        foreach (char c in txtUpper)
        {
            for (int j = 0; j < lettersScriptableObject.letters.Length; j++)
            {
                if (c == abcStr[j])
                {
                    GameObject newLetter = CreateLetter(c);
                    newLetter.transform.position = new Vector3(half, 0, 0);
                    newLetter.transform.SetParent(parent.transform);
                    half--;
                    break;
                }
            }
        }
    }

    GameObject CreateLetter(char c)
    {
        for (int i = 0; i < abcStr.Length; i++)
        {
            if (c == abcStr[i])
            {
                return Instantiate(lettersScriptableObject.letters[i]);
            }
        }

        return null;
    }
}
