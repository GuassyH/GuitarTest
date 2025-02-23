using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Guitar : MonoBehaviour
{

    [Header("Strings")]
    public List<GuitarString> guitarStrings;
    public List<AudioSource> audioSources;
    private static readonly float[] linearMassDensities = { 0.0065f, 0.0052f, 0.0037f, 0.0025f, 0.0017f, 0.0012f };
    private static readonly float[] stringTensions = { 77.0f, 72.0f, 66.0f, 59.0f, 53.0f, 48.0f };
    public readonly float[] baseFrequencies = { 82f, 110, 147f, 196, 247, 330 };
    public Material stringMat;
    public float stringLength = 0.648f;

    [Header("Frets")]
    public int CapoNumber = 0;
    public int numFrets = 12;
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

        // Create 6 strings and add guitar strings components and give them the properties they should have
        for (int i = 0; i < numStrings; i++){
            GameObject tempString = new GameObject();
            tempString.name = "Guitar String " + (i+1);
            GuitarString guitarString = tempString.AddComponent<GuitarString>();
            
            guitarString.id = i;
            guitarString.LinearMassDensity = linearMassDensities[i];
            guitarString.Tension = stringTensions[i];
            guitarString.shouldBeFrequency = baseFrequencies[i];
            guitarString.guitar = this;

            guitarString.audioSource = tempString.AddComponent<AudioSource>();
            guitarString.audioSource.playOnAwake = false;
            guitarString.audioSource.spatialBlend = 0.5f;
            guitarString.audioSource.Stop();

            guitarString.Length = stringLength;
            guitarString.stringMat = stringMat;

            tempString.transform.parent = this.transform;

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

        // Check if you press key 1-6 and plays strings accordingly
        if(Input.GetKeyDown(KeyCode.Alpha1)){ guitarStrings[0].PlayString(); };
        if(Input.GetKeyDown(KeyCode.Alpha2)){ guitarStrings[1].PlayString(); };
        if(Input.GetKeyDown(KeyCode.Alpha3)){ guitarStrings[2].PlayString(); };
        if(Input.GetKeyDown(KeyCode.Alpha4)){ guitarStrings[3].PlayString(); };
        if(Input.GetKeyDown(KeyCode.Alpha5)){ guitarStrings[4].PlayString(); };
        if(Input.GetKeyDown(KeyCode.Alpha6)){ guitarStrings[5].PlayString(); };

        // Play all strings with W
        if(Input.GetKeyDown(KeyCode.W)){ 
            StartCoroutine(PlayAllStrings());
        };
        // Mute all strings with Space
        if(Input.GetKeyDown(KeyCode.Space)){
            foreach (GuitarString guitarString in guitarStrings){
             guitarString.MuteString();   
            }    
        }

    }

    IEnumerator PlayAllStrings(){

        foreach (GuitarString guitarString in guitarStrings){
            guitarString.PlayString();   
            yield return new WaitForSeconds(0.05f);
        }
        yield return null;
    }

    void VolumeAdjust(){
        numPlaying = 0;

        // add get number of playing strings
        foreach (AudioSource source in audioSources){
            if(source.isPlaying){   numPlaying++;   }
        }

        // Change newAmplitude (3 strings out of 6 is 0.5f then multiply by effect)
        newAmplitude = 1f - (((float)numPlaying / (float)numStrings) * 0.5f);
        
        // Set multiplier to newAmplitude
        foreach (GuitarString guitarString in guitarStrings){
            guitarString.multiplier = newAmplitude;
        }

    }


    void CreateFrets(){
        // Create 13 Frets (including neck bridge)
        CapoSlider.maxValue = numFrets;

        GameObject nut = Instantiate(FretObject, this.transform);
        nut.transform.position = new Vector2(-stringLength*0.5f, 0);
        nut.name = "Nut";

        for (int i = 1; i < numFrets+1; i++){
            float d = stringLength / Mathf.Pow(2f, (float)i / 12f);

            GameObject newFret = Instantiate(FretObject, this.transform);
            newFret.transform.position = new Vector2((-stringLength*0.5f) + (stringLength - d), 0);
            newFret.name = "Fret: " + i;
        }
        GameObject bridge = Instantiate(FretObject, this.transform);
        bridge.transform.position = new Vector2((-stringLength*0.5f) + stringLength, 0);
        bridge.name = "Bridge";

    }
}
