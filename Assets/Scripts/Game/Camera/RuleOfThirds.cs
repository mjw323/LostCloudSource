using UnityEngine;

/// <summary>
/// Exposes helpful constants for dealing with the "Rule of Thirds".
/// </summary>
/// <remarks>
/// Coordinates are in Unity's viewport-space. (0, 0) is bottom-left.
/// </remarks>
public static class RuleOfThirds {
  public static Vector2 TopLeft { get { return topLeft; } }
  public static Vector2 TopRight { get { return topRight; } }
  public static Vector2 BottomLeft { get { return bottomLeft; } }
  public static Vector2 BottomRight { get { return bottomRight; } }
  private static readonly Vector2 topLeft = new Vector2(0.33f, 0.66f);
  private static readonly Vector2 topRight = new Vector2(0.66f, 0.66f);
  private static readonly Vector2 bottomLeft = new Vector2(0.33f, 0.33f);
  private static readonly Vector2 bottomRight = new Vector2(0.66f, 0.33f);
}