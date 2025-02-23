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
    public int fret;

	int timeIndex = 0;



	[HideInInspector] public AudioSource audioSource;
    [HideInInspector] public float multiplier;
    [HideInInspector] public float amplitude;
    [HideInInspector] public Material stringMat;
    [HideInInspector] public float id;
    [HideInInspector] public Guitar guitar;



    LineRenderer stringRend;
    
    Vector3 stringPos;

	void Awake()
	{
        this.transform.position = new Vector2(-3, (-2.5f/5) + (id/5));
        stringRend = this.gameObject.AddComponent<LineRenderer>();
        stringRend.positionCount = 5;
        stringRend.startWidth = 0.0015f;
        stringRend.materials[0] = stringMat;

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
        // Get the length of the string from the fret to th bridge
        float d = Length / Mathf.Pow(2f, (float)fret / 12f);
        return d;
    }

    float FundementalFrequency(){
        // Calculate the frequency it should have based on tension, LMD, and length (calculated above). Currently works WIP
        // Debug.Log($"String {id}: Length={L}, Tension={T}, LinearMassDensity={mu}, Frequency={f} Hz");
        float L = NewLength();
        float mu = LinearMassDensity;
        float T = Tension;
        float f = 1f / (2f * Length) * Mathf.Sqrt(T / mu);
        
        return f;
    }


    public void PlayString(){
        // Stop audio then play. With new amplitude (changes to reduce clipping)
        audioSource.Stop();
        audioSource.Play();
        amplitude = .8f;
    }

    public void MuteString(){
        audioSource.Stop();
        amplitude = 0f;
    }

    void StringRend(){

        // Set the positions of the line renderer. From the nut, to the neck bridge, to the fret, to the mid point between fret and bridge, to bridge
        stringPos = new Vector2(-(Length * 0.5f), (-2.5f/100) + (id/100));

        if(id<=2){  stringRend.SetPosition(0,  new Vector2(stringPos.x - ((id + 1) * 0.04f), -3.5f/100));   }
        else{  stringRend.SetPosition(0,  new Vector2(stringPos.x - ((3 - (id - 3)) * 0.04f), 3.5f/100));  }

        stringRend.SetPosition(1, stringPos);
        stringRend.SetPosition(2, stringPos + (Vector3.right*(Length - NewLength())));
        stringRend.SetPosition(3, new Vector2((stringRend.GetPosition(2).x + stringRend.GetPosition(4).x)/2, stringPos.y + Mathf.Sin(Time.time * frequency) * (amplitude/250)));
        stringRend.SetPosition(4, new Vector2(stringPos.x + Length, stringPos.y));

    }


    void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i += channels)
        {
            float sample = 0;

            // Fundamental frequency as well as harmonics
            sample += 1.0f * SineWave.CreateSine(timeIndex, frequency, sampleRate);
            sample += 0.5f * SineWave.CreateSine(timeIndex + 1, frequency * 2, sampleRate);
            sample += 0.3f * SineWave.CreateSine(timeIndex + 2, frequency * 3, sampleRate);
            sample += 0.2f * SineWave.CreateSine(timeIndex + 3, frequency * 4, sampleRate);
            sample -= 0.5f * SineWave.CreateSine(timeIndex + 4, frequency * 5, sampleRate);
            sample -= 0.3f * SineWave.CreateSine(timeIndex + 5, frequency * 6, sampleRate);
            sample -= 0.2f * SineWave.CreateSine(timeIndex + 6, frequency * 7, sampleRate);

            // Normalize to prevent clipping
            sample *= 0.5f;
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
