﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Leve2Controller : MonoBehaviour {
    public const int GRID_SIZE = 1;
    public static Sprite RED_CUBE;
    public static Sprite ORANGE_CUBE;
    public static Sprite YELLOW_CUBE;
    public static Sprite GREEN_CUBE;

    public static Sprite CYAN_CUBE;
    public static Sprite BLUE_CUBE;
    public static Sprite PURPLE_CUBE;
    public static Sprite RAINBOW_CUBE;
    public static Sprite CUPHEAD_CUBE;
    public static Sprite DISCO_1;
    public static Sprite DISCO_2;
    public static Sprite DISCO_3;
    public static Sprite ITEM_IN_SLOT;

    public static AudioClip TAP_CLIP;
    public static AudioClip COMPLETE_CLIP;

    public GameObject itemInInvenPrefab;

    public static Leve2Controller instance;

    public static int TOOL_BAR_SIZE = 4;
    public static int TOOL_BAR_ITEM_SIZE = 64;

    private LambdaBehavior[] leftInventory, rightInventory; 
    private List<LambdaBehavior[]> inventoryList;

    private int handLeftCurrPos, handRightCurrPos = 0;
    private GameObject handLeft, handRight, toolbarLeft, toolbarRight;

    private GameObject[] toolBarList;
    private const float inverseMoveTime = 1000;
    private bool leftCoroutineState, rightCoroutineState = false;

    public GameController.PlayableScene sceneToLoad;

    private HashSet<Portal> playerOnPortalCount;


    public enum PlayerSide {
        LEFT, RIGHT
    }

    private string[] sceneStrings = new string[]{"level2room1","level2room2","level2room3"};
    private uint currentLevel = 0;

    private AudioSource audioSrcObj;

    void Awake() {
        if(instance == null) {
            instance = this;
        }
        else if(instance != this) {
            Destroy(gameObject);
            return;
        }

        RED_CUBE = Resources.Load<Sprite>("Sprites/buildable_blocks1");
        ORANGE_CUBE = Resources.Load<Sprite>("Sprites/buildable_blocks2");
        YELLOW_CUBE = Resources.Load<Sprite>("Sprites/buildable_blocks3");
        GREEN_CUBE = Resources.Load<Sprite>("Sprites/buildable_blocks4");
        CYAN_CUBE = Resources.Load<Sprite>("Sprites/buildable_blocks5");
        BLUE_CUBE = Resources.Load<Sprite>("Sprites/buildable_blocks6");
        PURPLE_CUBE = Resources.Load<Sprite>("Sprites/buildable_blocks7");
        RAINBOW_CUBE = Resources.Load<Sprite>("Sprites/buildable_blocks8");
        CUPHEAD_CUBE = Resources.Load<Sprite>("Sprites/buildable_blocks9");
        DISCO_1 = Resources.Load<Sprite>("Sprites/basetile2");
        DISCO_2 = Resources.Load<Sprite>("Sprites/basetile3");
        DISCO_3 = Resources.Load<Sprite>("Sprites/basetile4");
        ITEM_IN_SLOT = Resources.Load<Sprite>("Sprites/white");

        TAP_CLIP = Resources.Load<AudioClip>("Audios/tap");
        COMPLETE_CLIP = Resources.Load<AudioClip>("Audios/complete");
        handLeft = GameObject.Find("HandLeft");
        handRight = GameObject.Find("HandRight");
        toolbarLeft = GameObject.Find("ToolbarLeft");
        toolbarRight = GameObject.Find("ToolbarRight");
        leftInventory = new LambdaBehavior[TOOL_BAR_SIZE];
        rightInventory = new LambdaBehavior[TOOL_BAR_SIZE];
        toolBarList = new GameObject[]{toolbarLeft, toolbarRight};
        inventoryList = new List<LambdaBehavior[]>();
        inventoryList.Add(leftInventory);
        inventoryList.Add(rightInventory);
        audioSrcObj = gameObject.AddComponent<AudioSource>();
        playerOnPortalCount = new HashSet<Portal>();
    }

    public void SetRoomCompleted() {
        //Activate portal
        PlayVictorySoundOneShot();
        var portals = GameObject.FindGameObjectsWithTag("Portal");
        foreach (var portal in portals)
        {
            portal.transform.localPosition = new Vector3(portal.transform.localPosition.x,
            portal.transform.localPosition.y, 0);
        }
    }

    internal void PlaySoundOneShot() {
        audioSrcObj.PlayOneShot(TAP_CLIP);
    }

    internal void PlayVictorySoundOneShot() {
        audioSrcObj.PlayOneShot(COMPLETE_CLIP);
    }

    private void MakeOptOn(PlayerSide side, out LambdaBehavior[] toOptOn, out int n) {
        switch(side) {
            case PlayerSide.LEFT: toOptOn = leftInventory; n = handLeftCurrPos; break;
            default: toOptOn = rightInventory; n = handRightCurrPos; break;
        }

    }
    public bool IsCurrentInventorySlotTaken(PlayerSide side) {
        LambdaBehavior[] toOptOn;
        int n;
        MakeOptOn(side, out toOptOn, out n);
        return toOptOn[n] != null;
    }

/** Get the next empty slot as an int, 
    returns -1 if inventory is full
 */
    public int GetNextEmptySlotForPlayer(PlayerSide side) {
        LambdaBehavior[] toOptOn;
        int n;
        MakeOptOn(side, out toOptOn, out n /*ignored*/);
        for(int i = 0; i < TOOL_BAR_SIZE; i++) {
            if(toOptOn[i] == null) {
                return i;
            }
        }
        return -1;

    }
    /** @Nullable
     */
    public LambdaBehavior GetInventoryNForPlayer(PlayerSide side) {
        LambdaBehavior[] toOptOn;
        int n;
        MakeOptOn(side, out toOptOn, out n);
        var ret = toOptOn[n];
        toOptOn[n] = null;
        if(ret != null) {
            PlaySoundOneShot();
        }
        UpdateUI();
        return ret;
    }

    public bool PutInInventory(PlayerSide side, LambdaBehavior beh) {
        LambdaBehavior[] toOptOn;
        int n;
        MakeOptOn(side, out toOptOn, out n);
        if(toOptOn[n] == null) {
            toOptOn[n] = beh;
            UpdateUI();
            PlaySoundOneShot();
            return true;
        }
        return false;
    }

    /* Overloaded method for PutInInventory */
    public bool PutInInventory(PlayerSide side, LambdaBehavior beh, int slotNumber) {
        LambdaBehavior[] toOptOn;
        int n;
        MakeOptOn(side, out toOptOn, out n /* ignored */); /* Getting the toOptOn only */
        if(toOptOn[slotNumber] == null) {
            toOptOn[slotNumber] = beh;
            UpdateUI();
            PlaySoundOneShot();
            return true;
        }
        return false;
    }

    void UpdateUI() {
        foreach (var a in toolBarList) {
            foreach (RectTransform child in a.gameObject.GetComponentInChildren<RectTransform>())
            {
                Destroy(child.gameObject);
            }
        }

        for (int i = 0; i < TOOL_BAR_SIZE; i++)
        {
            if (leftInventory[i] != null)
            {
                var go = Instantiate(itemInInvenPrefab);
                go.transform.SetParent(toolbarLeft.transform);
                go.transform.localPosition = new Vector3(0, 128 - (i * TOOL_BAR_ITEM_SIZE), 0);
                var c = go.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                c.text = leftInventory[i].desc;
                if(leftInventory[i].extraAction != null) {
                    leftInventory[i].extraAction(c);
                }
            }
        }

        for (int i = 0; i < TOOL_BAR_SIZE; i++)
        {
            if (rightInventory[i] != null)
            {
                var go = Instantiate(itemInInvenPrefab);
                go.transform.SetParent(toolbarRight.transform);
                go.transform.localPosition = new Vector3(0, 128 - (i * TOOL_BAR_ITEM_SIZE), 0);
                var c = go.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                c.text = rightInventory[i].desc;
                if(rightInventory[i].extraAction != null) {
                    rightInventory[i].extraAction(c);
                }
            }
        }
    }

    /* TODO: WARNING: Duplicated code, `eval`ing this would be good */
    public int CycleCursorLeft() {
        if(leftCoroutineState) return -1;
        PlaySoundOneShot();
        handLeftCurrPos++;
        var rtf = handLeft.GetComponent<RectTransform>();
        if(handLeftCurrPos >= TOOL_BAR_SIZE) {
            leftCoroutineState = true;
            StartCoroutine(CycleCoRoutineLeftHand(rtf, rtf.transform.position + Vector3.up * TOOL_BAR_ITEM_SIZE  * (TOOL_BAR_SIZE - 1)));
            handLeftCurrPos = 0;
            return handLeftCurrPos;
        }
        else {
            leftCoroutineState = true;
            StartCoroutine(CycleCoRoutineLeftHand(rtf, rtf.transform.position + Vector3.down * (TOOL_BAR_ITEM_SIZE )));
            return handLeftCurrPos;
        }

    }

    private IEnumerator CycleCoRoutineLeftHand(Transform rtf, Vector3 end) {

        // Only one instace should be running
        float remainingDist = (rtf.position - end).sqrMagnitude;
        
        while(remainingDist > float.Epsilon) {
            Vector3 newPosition = Vector3.MoveTowards(handLeft.GetComponent<Rigidbody2D>().position, end, inverseMoveTime * Time.deltaTime);
            handLeft.GetComponent<Rigidbody2D>().MovePosition(newPosition);
            remainingDist = (rtf.position - end).sqrMagnitude;
            yield return null;
        }
        leftCoroutineState = false;
        yield return null;

    }

    public int CycleCursorRight() {
        if(rightCoroutineState) return -1;
        PlaySoundOneShot();
        handRightCurrPos++;
        var rtf = handRight.GetComponent<RectTransform>();
        if(handRightCurrPos >= TOOL_BAR_SIZE) {
            rightCoroutineState = true;
            StartCoroutine(CycleCoRoutineRightHand(rtf, rtf.transform.position + Vector3.up * TOOL_BAR_ITEM_SIZE  * (TOOL_BAR_SIZE - 1)));
            handRightCurrPos = 0;
            return handRightCurrPos;
        }
        else {
            rightCoroutineState = true;
            StartCoroutine(CycleCoRoutineRightHand(rtf, rtf.transform.position + Vector3.down * (TOOL_BAR_ITEM_SIZE )));
            return handRightCurrPos;
        }

    }

    private IEnumerator CycleCoRoutineRightHand(Transform rtf, Vector3 end) {
        // Only one instace should be running
        float remainingDist = (rtf.position - end).sqrMagnitude;
        
        while(remainingDist > float.Epsilon) {
            Vector3 newPosition = Vector3.MoveTowards(handRight.GetComponent<Rigidbody2D>().position, end, inverseMoveTime * Time.deltaTime);
            handRight.GetComponent<Rigidbody2D>().MovePosition(newPosition);
            remainingDist = (rtf.position - end).sqrMagnitude;
            yield return null;
        }
        rightCoroutineState = false;
        yield return null;
    }

    private void StartNextLevel() {
        SceneManager.LoadScene(GameController.GetFileName(sceneToLoad));
    }


    public void AddPlayerEnterPortal(Portal portal)
    {
        playerOnPortalCount.Add(portal);
        if (playerOnPortalCount.Count >= 2)
        {
            playerOnPortalCount.Clear();
            StartNextLevel();
        }
    }


public void DecreasePlayerEnterPortal(Portal portal) {
    playerOnPortalCount.Remove(portal);
}

}
