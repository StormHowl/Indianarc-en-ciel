using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct KeyToFreq
{
    public KeyCode code;
    public float value;
}

public class Oscillator : MonoBehaviour
{
    public static Oscillator instance = null;

    public float Freq = 440f;

    private float _increment = 1f;

    private float sampleFreq;

    public float gain = 0.2f;

    public float playing = -1f;

    public AudioSource source;
    public AudioSource bonus;

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            source = GetComponent<AudioSource>();
            sampleFreq = AudioSettings.outputSampleRate;
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(string note)
    {
        string val = note[0].ToString();
        int transpose;
        if (note.Length > 1 && note[1] == '#')
        {
            val += '#';
            transpose = note[2];
        }
        else
        {
            transpose = note[1];
        }

        source.pitch = Mathf.Pow(2, (float) ((GetNoteVal(val) + transpose) / 12.0))/10;
        source.Play();
    }

    public void PlayBonus()
    {
        bonus.Play();
    }

    private int GetNoteVal(string note)
    {
        int val = 0;
        switch (note[0])
        {
            case 'A': val = 9;
                break;
            case 'B': val = 11; break;
            case 'C': val = 12; break;
            case 'D': val = 14; break;
            case 'E': val = 4; break;
            case 'F': val = 5; break;
            case 'G': val = 7; break;
        }

        if (note.Length > 1 && note[1] == '#')
        {
            val += 1;
        }

        return val;
    }

    public void StopSound()
    {
        playing = -1f;
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        /*double samplesPerOccilation = (sampleFreq / playing);

        if (playing > 0f)
        {
            if (gain < 0f)
                gain = 0f;

            if (gain >= 1f)
                gain = 1f;

            for (int i = 0; i < data.Length; i += channels)
            {
                data[i] = gain * i * _increment * Mathf.Sin((float)(((200 % samplesPerOccilation) / samplesPerOccilation) * (Mathf.PI * 2)));
                if (channels == 2)
                {
                    data[i + 1] = data[i];
                }
            }
        }*/
    }
}