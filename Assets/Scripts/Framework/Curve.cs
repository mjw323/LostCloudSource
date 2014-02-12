using UnityEngine;
using System;

/// <summary>
/// Custom asset type wrapping Unity's AnimationCurve.
/// </summary>
[Serializable]
public class Curve : ScriptableObject {
  /// <summary>
  /// Evaluates the curve at 't'.
  /// </summary>
  public float Evaluate(float t) {
    return curve.Evaluate(t);
  }

  [SerializeField] private AnimationCurve curve;
}