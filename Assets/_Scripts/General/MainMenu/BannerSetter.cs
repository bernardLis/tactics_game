using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerSetter : MonoBehaviour
{
    [SerializeField] List<GameObject> _poles;
    [SerializeField] List<GameObject> _flags;
    [SerializeField] List<Material> _colors;


    public void SetBanner(int poleIndex, int flagIndex, int colorIndex)
    {
        _poles.ForEach(p => p.SetActive(false));
        _flags.ForEach(f => f.SetActive(false));
        
        _poles[poleIndex].SetActive(true);
        _flags[flagIndex].SetActive(true);
        _flags[flagIndex].GetComponent<Renderer>().material = _colors[colorIndex];
    }
}
