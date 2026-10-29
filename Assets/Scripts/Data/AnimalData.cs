    using UnityEngine;

    [CreateAssetMenu(fileName = "AnimalData", menuName = "Scriptable Objects/AnimalData")]
    public class AnimalData : ScriptableObject
    {
        
        public int level;
        public string speciesName;

        public Sprite sprite;
        public float visualScale = 1f;
        public float mass = 1f;

       
        public AnimalData nextEvolution;
        public int scoreValue = 10;

        
        public GameObject mergeVFXPrefab;

        
        public AudioClip mergeSound;

        
        public float shakeEnergyReward = 5f;
    }