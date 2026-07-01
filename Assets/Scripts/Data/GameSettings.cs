using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Game Loop")]
    [Tooltip("Tempo em segundos para o countdown inicial (3... 2... 1...)")]
    public float startCountdownTime = 3f;

    [Tooltip("Tempo de tolerância com o animal acima da linha antes de dar Game Over")]
    public float deadlineToleranceTime = 3f;

    [Header("Mecânica de Shake")]
    [Tooltip("Quantidade de energia necessária para encher a barra e liberar o Shake")]
    public float maxShakeEnergy = 100f;

    [Tooltip("Força do impulso (Impulse) aplicado nos animais para cima durante o Shake")]
    public float shakeForce = 15f;

    [Tooltip("Sensibilidade do acelerômetro para disparar o evento (sqrMagnitude)")]
    public float accelerometerThreshold = 2.5f;

    [Header("Física Global")]
    [Tooltip("Modificador de gravidade. Jogos Suika costumam ter queda levemente mais rápida.")]
    public float globalGravityScale = 1.2f;
}