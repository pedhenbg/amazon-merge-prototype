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

    [Header("VFX & Audio")]
    public GameObject mergeVFXPrefab;
    public AudioClip mergeSound;
    public float shakeEnergyReward = 5f;
}