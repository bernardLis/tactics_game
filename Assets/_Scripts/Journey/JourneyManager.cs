using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JourneyManager : MonoBehaviour
{
    public List<Character> playerTroops;
    public bool wasJourneySetUp { get; private set; }
    public int journeySeed { get; private set; } = 0; // TODO: this is a bad idea, probably
    public JourneyNode currentJourneyNode { get; private set; }
    public int obols { get; private set; }
    public JourneyNodeReward reward { get; private set; }

    [Header("Unity Setup")]
    public JourneyEvent[] allEvents;

    List<JourneyEvent> availableEvents;

    [HideInInspector] public List<JourneyPath> journeyPaths = new();
    public List<JourneyNode> visitedNodes = new();

    public static JourneyManager instance;
    void Awake()
    {
        #region Singleton
        // singleton
        if (instance != null && instance != this)
        {
            // TODO: it gets here on scene transitions, should I do something about it?
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

        // copy array to list;
        availableEvents = new(allEvents);
        //LoadPlayerCharacters();
    }

    public void SetPlayerTroops(List<Character> _troops)
    {
        playerTroops = new(_troops);
        // TODO: I imagine that player characters are set-up somewhere and I can load them
        Debug.Log("Loading player characters");
    }

    public void JourneyWasSetUp(bool _was) // TODO: better naming
    {
        wasJourneySetUp = _was;
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

    public void SetObols(int _o)
    {
        obols = _o;
    }

    public void SetNodeReward(JourneyNodeReward _r)
    {
        reward = _r;
    }


    public JourneyEvent ChooseEvent()
    {
        JourneyEvent ev = availableEvents[Random.Range(0, availableEvents.Count)];
        availableEvents.Remove(ev);
        return ev;
    }
}
