using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuitarString : MonoBehaviour
{


    [Header("String Properties")]
    [Tooltip("Kilograms")] public float LinearMassDensity = 0f;
    [Tooltip("Meters")] public float Length = 0.648f;
    [Tooltip("Newton")] public float Tension = 0;
    public float shouldBeFrequency;




    public float frequency;

    public float sampleRate = 44100;
	public float waveLengthInSeconds = 2.0f;
    public List<GameObject> guitarStrings;

	public AudioSource audioSource;
	int timeIndex = 0;

    float amplitude;
    public float multiplier;
    public int fret;

    [HideInInspector] public float id;
    [HideInInspector] public Guitar guitar;

    LineRenderer stringRend;

	void Awake()
	{
        this.transform.position = new Vector2(-3, (-2.5f/5) + (id/5));
        stringRend = this.gameObject.AddComponent<LineRenderer>();
        stringRend.positionCount = 4;
        stringRend.startWidth = 0.0015f;

    }
	
	void Update()
	{

        audioSource.volume = amplitude * multiplier;
        StringRend();

        fret = guitar.CapoNumber;
        frequency = shouldBeFrequency * Mathf.Pow(2f, (float)fret / 12f);
        //frequency = FundementalFrequency();

        if(audioSource.isPlaying){
            amplitude -= Time.deltaTime * 0.5f;
            if(audioSource.volume <= 0){
                audioSource.Stop();
            }
        }
	}

    float NewLength() {
        float d = Length / Mathf.Pow(2f, (float)fret / 12f);
        return d;
    }

    float FundementalFrequency(){    
        float L = NewLength();
        float mu = LinearMassDensity;
        float T = Tension;
        float f = 1f / (2f * Length) * Mathf.Sqrt(T / mu);
        
        Debug.Log($"String {id}: Length={L}, Tension={T}, LinearMassDensity={mu}, Frequency={f} Hz");
        
        return f;
    }

	
    public void PlayString(float newAmplitude){
        audioSource.Stop();
        audioSource.Play();
        amplitude = 1f;
        multiplier = newAmplitude;

    }


    void StringRend(){
        Vector3 stringPos = new Vector2(-(Length * 0.5f), (-2.5f/80) + (id/80));

        stringRend.SetPosition(0, stringPos);
        stringRend.SetPosition(1, stringPos + (Vector3.right*(Length - NewLength())));
        stringRend.SetPosition(2, new Vector2(stringPos.x + (Length * 0.75f), stringPos.y + Mathf.Sin(Time.time * frequency) * (amplitude/150)));
        stringRend.SetPosition(3, new Vector2(stringPos.x + Length, stringPos.y));

    }


    void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i += channels)
        {
            float sample = 0;

            // Fundamental frequency
           // sample = SineWave.CreateSine(timeIndex, frequency, sampleRate);

            sample += 1.0f * SineWave.CreateSine(timeIndex, frequency, sampleRate);
            sample += 0.5f * SineWave.CreateSine(timeIndex + 1, frequency * 2, sampleRate);
            sample += 0.3f * SineWave.CreateSine(timeIndex + 2, frequency * 3, sampleRate);
            sample += 0.2f * SineWave.CreateSine(timeIndex + 3, frequency * 4, sampleRate);

            // Normalize to prevent clipping
            if(sample > 1)
                sample = 1;
            if(sample < -1)
                sample = -1;

            data[i] = sample;
            if (channels == 2)
                data[i + 1] = sample;

            // Increment and reset timeIndex based on frequency
            timeIndex++;
            if (timeIndex >= sampleRate / frequency)
                timeIndex = 0;
        }
    }
}
