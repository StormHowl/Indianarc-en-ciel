using System;
using System.Collections;
using System.Collections.Generic;
using NAudio.Midi;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static Vector3 lastPosition = new Vector3(0, 0, 0);
    public static int MinimumXGapBetweenPlatforms = 3;
    public static int MinimumYGapBetweenPlatforms = 1;
    public static int MaxJumpYGain = 3;
    public static int MaxJumpXGain = 5;

    public string FilePath;
    public GameObject PlatformPrefab;
    public GameObject PlayerPrefab;

    public Material Red;
    public Material Orange;
    public Material Yellow;
    public Material Greenlight;
    public Material Green;
    public Material Cyan;
    public Material Blue;
    public Material Black;

    private MidiFile _piece;

    private List<GameObject> _platforms;

    private GameObject Player;

	private int distanceToTheNextBonus;
	public GameObject bonus;
	public GameObject enemy;

    // Use this for initialization
    void Start()
    {
        Init();
        InitObjects();
    }

    void Init() { 

    _piece = new MidiFile(FilePath, false);
		distanceToTheNextBonus = (int) UnityEngine.Random.Range(5.0f, 8.0f);
        _platforms = new List<GameObject>();

        if (_piece != null)
        {
            for (int n = 0; n < 1; ++n)
            {
                foreach (var midiEvent in _piece.Events[n])
                {
                    if (MidiEvent.IsNoteOn(midiEvent))
                    {
                        var note = (NoteOnEvent) midiEvent;

                        _platforms.Add(
                            UpdateNewPlatformPositionAndScale(
                                Instantiate(PlatformPrefab,
                                    DeterminePlatformPosition(note),
                                    Quaternion.identity),
                                note)
                        );

                        // Check if platform is accessible
                        if (_platforms.Count > 1 && !CheckIfPossibleToGoForward())
                        {
                            var tempPlat = _platforms[_platforms.Count - 1];
                            tempPlat.transform.position = new Vector3(
                                tempPlat.transform.position.x +
                                _platforms[_platforms.Count - 2].transform.localScale.x / 2.0f,
                                tempPlat.transform.position.y);
                        }

                        lastPosition = _platforms[_platforms.Count - 1].transform.position;
                        lastPosition.x += _platforms[_platforms.Count - 1].transform.localScale.x / 2.0f;
                    }
                }
            }
            Player = Instantiate(PlayerPrefab, _platforms[0].transform.position + new Vector3(0, 10),
                Quaternion.identity);
        }
    }

    void InitObjects()
    {
        int previousType = -1;
        foreach (GameObject plat in _platforms)
        {
            if (distanceToTheNextBonus == 0)
            {
                GameObject b = Instantiate(bonus, plat.transform.position + new Vector3(0, 2),
                    Quaternion.identity);

                int type = (int)UnityEngine.Random.Range(0.0f, 5.0f);
                while (type == previousType)
                    type = (int)UnityEngine.Random.Range(0.0f, 5.0f);

                previousType = type;
                switch (type)
                {
                    case 0:
                        b.tag = "Red";
                        break;
                    case 1:
                        b.tag = "Yellow";
                        break;
                    case 2:
                        b.tag = "Pink";
                        break;
                    case 3:
                        b.tag = "Green";
                        break;
                    case 4:
                        b.tag = "Orange";
                        break;
                }

                distanceToTheNextBonus = (int)UnityEngine.Random.Range(5.0f, 8.0f);
            }
            else
            {
                distanceToTheNextBonus--;
                float placeMonster = UnityEngine.Random.Range(0.0f, 3.0f);
                if (placeMonster < 1.0f)
                {
                    Instantiate(enemy, plat.transform.position + new Vector3(0, 5),
                        Quaternion.identity);
                }
            }
        }
    }

    Vector3 DeterminePlatformPosition(NoteOnEvent note)
    {
        return new Vector3(MinimumXGapBetweenPlatforms + lastPosition.x -4f, note.NoteNumber * 0.25f);
    }

    GameObject UpdateNewPlatformPositionAndScale(GameObject platform, NoteOnEvent note)
    {
        platform.transform.localScale = new Vector3(note.NoteLength * (note.NoteLength < 50 ? 0.3f : 0.03f), platform.transform.localScale.y);

        // Check if it is not too far in Y, then X
        if (Math.Abs(platform.transform.position.y - lastPosition.y) > MaxJumpYGain)
        {
            platform.transform.position = new Vector3(platform.transform.position.x,
                lastPosition.y + (lastPosition.y > platform.transform.position.y ? -MaxJumpYGain : MaxJumpYGain));
        }

        if (Math.Abs(platform.transform.position.x - lastPosition.x) > MaxJumpXGain)
        {
            platform.transform.position = new Vector3(lastPosition.x + MaxJumpXGain, platform.transform.position.y);
        }

        // Adapt material according to note
        var comp = platform.gameObject.GetComponentInChildren<MeshRenderer>();
        comp.material = GetMaterialOfNote(note.NoteName);
        comp.material.shader.name = "UI/Default";
        platform.GetComponent<PlatformEffect>().Tag = "PP" + note.NoteNumber;
        
        return platform;
    }

    bool CheckIfPossibleToGoForward()
    {
        var lastAdded = _platforms[_platforms.Count - 1];
        var lastBefore = _platforms[_platforms.Count - 2];

        for (float i = 0; i < MaxJumpYGain-1.0f; i += 0.5f)
            if (lastAdded.GetComponent<BoxCollider2D>().OverlapPoint(new Vector2(lastBefore.transform.position.x,
                lastBefore.transform.position.y + i)))
                return true;
        return false;
    }

    private Material GetMaterialOfNote(string note)
    {
        switch (note[0])
        {
            case 'A':
                return Red;
            case 'B':
                return Orange;
            case 'C':
                return Yellow;
            case 'D':
                return Greenlight;
            case 'E':
                return Green;
            case 'F':
                return Cyan;
            case 'G':
                return Blue;
        }

        return Black;
    }

    void Update()
    {
        if (PlayerControl.life <= 0 || Player.transform.position.y < -5)
        {
            Player.transform.position = _platforms[0].transform.position + new Vector3(0, 10);
            Player.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);
            PlayerControl.timeStamp = Time.time + PlayerControl.invulnerabilityFrame;
            PlayerControl.life--;
            if (PlayerControl.life < 0)
            {
                PlayerControl.life = 3;
                InitObjects();
            }
        }
    }
}