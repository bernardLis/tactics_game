using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using System.IO;

public class TempCharacterManager : MonoBehaviour
{
    public List<Character> characters;

    public void CharactersToResources()
    {
        string path = "Assets/Resources/Characters";
        // clear the characters folder
        if (Directory.Exists(path))
            Directory.Delete(path, true);
        Directory.CreateDirectory(path);

        foreach (Character c in characters)
        {

        }

    }
}
