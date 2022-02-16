using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JourneyManager : MonoBehaviour
{
    public int journeySeed { get; private set; } = 0; // TODO: this is a bad idea, probably
    public JourneyNode currentJourneyNode { get; private set; }

    public List<JourneyPath> journeyPaths = new();
    public List<JourneyNode> visitedNodes = new();

    public static JourneyManager instance;
    void Awake()
    {
        #region Singleton
        // singleton
        if (instance != null && instance != this)
        {
            Debug.LogWarning("More than one instance of JourneyManager found");
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        #endregion
    }

    public void SetJourneySeed(int _s)
    {
        journeySeed = _s;
    }

    public void SetCurrentJourneyNode(JourneyNode _n)
    {
        visitedNodes.Add(_n);
        currentJourneyNode = _n;
    }



}
