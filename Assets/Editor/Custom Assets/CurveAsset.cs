using UnityEngine;
using UnityEditor;

public class CurveAsset {
  [MenuItem("Assets/Create/Curve")]
  public static void Create() {
    CustomAsset.Create<Curve>();
  }
}