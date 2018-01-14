using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SpectrumData : MonoBehaviour
{
    private AudioSource Source;
    public static float[] _samples = new float[512];

    public static float[] _freqBend = new float[8];

    // Use this for initialization
    private void Start()
    {
        Source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        Source.GetSpectrumData(_samples, 0, FFTWindow.Blackman);

        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            float avg = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7)
            {
                sampleCount += 2;
            }
            for (int j = 0; j < sampleCount; j++)
            {
                avg += _samples[count] * (count + 1);
                count++;
            }

            avg /= count;

            _freqBend[i] = avg * 10;
        }
    }
}