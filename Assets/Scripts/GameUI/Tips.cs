using UnityEngine;

public class Tips
{
    public static string[] tips =
    {
        "One bird is worth eight fish!", "Chests contains 3 different effects, scores and hunger points",
        "You can find invisible, speed or killer effect in chests", "Avoid mines"
    };

    public static string GetRandomTip()
    {
        return tips[Random.Range(0, tips.Length)];
    }
}