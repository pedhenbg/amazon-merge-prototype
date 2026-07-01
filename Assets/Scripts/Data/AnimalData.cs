    using UnityEngine;

    [CreateAssetMenu(fileName = "AnimalData", menuName = "Scriptable Objects/AnimalData")]
    public class AnimalData : ScriptableObject
    {
        [Header("Identity")]
        public int level;
        public string speciesName;

        [Header("Visuals and Physics")]
        public Sprite sprite;
        public float visualScale = 1f;
        public float mass = 1f;

        [Header("Evolution")]
        public AnimalData nextEvolution;
        public int scoreValue = 10;

        [Header("Juice & Feedback (Arquitetura Profissional)")]
        [Tooltip("Partícula instanciada quando este animal é CRIADO a partir de um merge")]
        public GameObject mergeVFXPrefab;

        [Tooltip("Som tocado no momento do merge para este nível específico")]
        public AudioClip mergeSound;

        [Tooltip("Quanto este animal enche a barra de Shake ao fazer merge")]
        public float shakeEnergyReward = 5f;
    }