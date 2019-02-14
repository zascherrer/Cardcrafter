using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public bool isPlayerOneTurn;
    public enum Phases
    {
        START_TURN,
        UPKEEP,
        DRAW,
        ACTION_ONE,
        ATTACK,
        ACTION_TWO,
        CLEANUP,
        END_TURN
    }
    private Phases battlePhases;

    // Start is called before the first frame update
    void Start()
    {
        battlePhases = new Phases();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
