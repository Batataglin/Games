using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalSettings
{
    public enum GAME_STATE { INGAME, DEATH, PAUSE};

    public static PlayerController gPlayer;
    public static EnemyController gEnemy;
    public static GUIContoller gGUI;

    public static int gGameState;
    public static int gCurrentScene;

}
