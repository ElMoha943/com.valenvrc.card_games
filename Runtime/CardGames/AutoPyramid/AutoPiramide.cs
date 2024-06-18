using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;
using UnityEngine.UI;
using TMPro;

//This script will take an 52 amount of cards and make a pyramid of Y height with them
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class AutoPiramide : UdonSharpBehaviour
{
    [Header("Settings")]
    public float separation = 0.30f; //The distance between the cards
    [UdonSynced] int pisos = 9; //3-9
    
    [SerializeField] VRCObjectPool cards;
    [SerializeField] Slider floorSlider;
    [SerializeField] TextMeshProUGUI floorText;

    //The position where the first card will be placed, this will be the middle top card, the rest will be placed below it
    public Transform initialPosition;

    void Start(){
        if(Networking.IsOwner(gameObject)){
            pisos = (int)floorSlider.value;
            RequestSerialization();
        }
    }

    public override void Interact(){
        CreatePyramid();
    }

    public void updateFloors(){
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        pisos = (int)floorSlider.value;
        floorText.text = pisos.ToString();
        RequestSerialization();
    }

    public override void OnDeserialization(){
        floorSlider.value = pisos;
        floorText.text = pisos.ToString();
    }

    public void CreatePyramid(){
        Networking.SetOwner(Networking.LocalPlayer, cards.gameObject);
        //Reset Cards
        foreach(GameObject card in cards.Pool){
            Networking.SetOwner(Networking.LocalPlayer, card);
            cards.Return(card);
        }
        //Suffle cards
        cards.Shuffle();
        //Place cards
        for(int i = 0; i<pisos;i++){
            for(int j = 0; j<=i;j++){
                GameObject card = cards.TryToSpawn();
                card.transform.localPosition = new Vector3(initialPosition.position.x + (j*separation) - (i*separation/2), initialPosition.position.y, initialPosition.position.z - (i*separation));
            }
        }
    }
}
