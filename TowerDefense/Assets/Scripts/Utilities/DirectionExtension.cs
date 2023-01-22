using UnityEngine;

public static class DirectionExtension
{
    private readonly static Quaternion[] _rotations =
    {
        Quaternion.identity,
        Quaternion.Euler(0f, 90f, 0f),
        Quaternion.Euler(0f, 180f, 0f),
        Quaternion.Euler(0f, 270f, 0f)
    };

    private readonly static Vector3[] halfVectors =
    {
        Vector3.forward * 0.5f,
        Vector3.right * 0.5f,
        Vector3.back * 0.5f,
        Vector3.left * 0.5f
    };

    public static Quaternion GetRotation(this Direction direction)
    {
        return _rotations[(int)direction];
    }

    public static DirectionChange GetDirectionChangeTo(this Direction cur, Direction next)
    {
        if (cur == next)
        {
            return DirectionChange.None;
        }

        if (cur + 1 == next || cur - 3 == next)
        {
            return DirectionChange.TurnRight;
        }

        if (cur - 1 == next || cur + 3 == next)
        {
            return DirectionChange.TurnLeft;
        }

        return DirectionChange.TurnAround;
    }

    public static float GetAngle(this Direction direction)
    {
        return (float) direction * 90;
    }

    public static Vector3 GetHalfVector(this Direction direction)
    {
        return halfVectors[(int) direction];
    }
}

public enum Direction
{
    North,
    East,
    South,
    West
}

public enum DirectionChange
{
    None,
    TurnRight,
    TurnLeft,
    TurnAround
}
