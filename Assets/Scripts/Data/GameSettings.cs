using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/GameSettings")]
public class GameSettings : ScriptableObject
{
    public float startCountdownTime = 3f;

   
    public float deadlineToleranceTime = 3f;

    
    public float maxShakeEnergy = 100f;

    
    public float shakeForce = 15f;

   
    public float accelerometerThreshold = 2.5f;

    
    public float globalGravityScale = 1.2f;
}