using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;
using UnityEngine.UI;

public class Guitar : MonoBehaviour
{

    public List<GuitarString> guitarStrings;
    public List<AudioSource> audioSources;
    private static readonly float[] linearMassDensities = { 0.0065f, 0.0052f, 0.0037f, 0.0025f, 0.0017f, 0.0012f };
    private static readonly float[] stringTensions = { 77.0f, 72.0f, 66.0f, 59.0f, 53.0f, 48.0f };

    [Header("Frets")]
    public int CapoNumber = 0;
    public GameObject FretObject;

    [Header("UI")]
    public Slider CapoSlider;
    public TextMeshProUGUI CapoNumberText;

    int numStrings = 6;
    int numPlaying;
    float newAmplitude;
    float semitone = 1.05946f; 

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numStrings; i++){
            GameObject tempString = new GameObject();
            tempString.name = "Guitar String " + (i+1);
            GuitarString guitarString = tempString.AddComponent<GuitarString>();
            
            guitarString.id = i;
            guitarString.LinearMassDensity = linearMassDensities[i];
            guitarString.Tension = stringTensions[i];
            guitarString.guitar = this;

            guitarString.audioSource = tempString.AddComponent<AudioSource>();
            guitarString.audioSource.playOnAwake = false;
            guitarString.audioSource.spatialBlend = 0.5f;
            guitarString.audioSource.Stop();

            audioSources.Add(guitarString.audioSource);

            guitarStrings.Add(guitarString);
        }

        CreateFrets();
    }

    // Update is called once per frame
    void Update()
    {

        VolumeAdjust();

        CapoNumber = (int)CapoSlider.value;
        CapoNumberText.text = CapoNumber.ToString();

        if(Input.GetKeyDown(KeyCode.Alpha1)){ guitarStrings[0].PlayString(newAmplitude); };
        if(Input.GetKeyDown(KeyCode.Alpha2)){ guitarStrings[1].PlayString(newAmplitude); };
        if(Input.GetKeyDown(KeyCode.Alpha3)){ guitarStrings[2].PlayString(newAmplitude); };
        if(Input.GetKeyDown(KeyCode.Alpha4)){ guitarStrings[3].PlayString(newAmplitude); };
        if(Input.GetKeyDown(KeyCode.Alpha5)){ guitarStrings[4].PlayString(newAmplitude); };
        if(Input.GetKeyDown(KeyCode.Alpha6)){ guitarStrings[5].PlayString(newAmplitude); };


        if(Input.GetKeyDown(KeyCode.Space)){ 
            guitarStrings[0].PlayString(newAmplitude);
            guitarStrings[1].PlayString(newAmplitude);
            guitarStrings[2].PlayString(newAmplitude);
            guitarStrings[3].PlayString(newAmplitude);
            guitarStrings[4].PlayString(newAmplitude);
            guitarStrings[5].PlayString(newAmplitude); 
        };

    }


    void VolumeAdjust(){
        numPlaying = 0;

        foreach (AudioSource source in audioSources)
        {
            if(source.isPlaying){
                numPlaying++;
            }
        }

        newAmplitude = 1f - (((float)numPlaying / (float)numStrings) * 0.75f);
        
        foreach (GuitarString guitarString in guitarStrings)
        {
            guitarString.multiplier = newAmplitude;
        }

        
        
    }


    void CreateFrets(){
        for (int i = 0; i < 12; i++)
        {
            float d = 0.65f / Mathf.Pow(2f, (float)i / 12f);


            GameObject newFret = Instantiate(FretObject, this.transform);
            newFret.transform.position = new Vector2((-0.65f*0.5f) + (0.65f - d), 0);
        }
    }
}
