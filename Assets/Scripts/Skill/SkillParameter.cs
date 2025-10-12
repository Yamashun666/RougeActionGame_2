using UnityEngine;

public enum HitShapeType
{
    Box,
    Capsule,
    Ray
}

[System.Serializable]
public class SkillParameter
{
    public string SkillName;
    public float Power = 10f;
    public HitShapeType HitShape = HitShapeType.Box;

    // Box用
    public Vector3 BoxSize = new Vector3(1, 1, 1);
    public Vector3 Position = new Vector3(1, 1, 1);


    // Capsule用
    public float CapsuleRadius = 0.5f;
    public float CapsuleHeight = 2f;

    // Ray用
    public float RayDistance = 3f;

    // 共通
    public float Cooldown = 1f;
    public LayerMask TargetLayer;
}