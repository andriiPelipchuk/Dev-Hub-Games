using System;

public static class GameEvents
{
    public static event Action OnBubblePopped;
    public static event Action OnGameOver;

    public static void TriggerBubblePopped() => OnBubblePopped?.Invoke();
    public static void TriggerGameOver() => OnGameOver?.Invoke();
}