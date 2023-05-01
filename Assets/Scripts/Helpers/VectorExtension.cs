using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtension {

    public static float AngleByAxis(this Vector3 v1, Vector3 v2, Vector3 axis) {
        Vector3 right = Vector3.Cross(v2, axis);
        v2 = Vector3.Cross(axis, right);
        return Mathf.Atan2(Vector3.Dot(v1, right), Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }

    public static Vector2 Rotate(this Vector2 v, float radDelta) {
        return new Vector2(
            v.x * Mathf.Cos(radDelta) - v.y * Mathf.Sin(radDelta),
            v.x * Mathf.Sin(radDelta) + v.y * Mathf.Cos(radDelta)
        );
    }
    public static Vector3 Rotate2D(this Vector3 v, float radDelta) {
        return new Vector2(
            v.x * Mathf.Cos(radDelta) - v.y * Mathf.Sin(radDelta),
            v.x * Mathf.Sin(radDelta) + v.y * Mathf.Cos(radDelta)
        );
    }

    public static Vector3 SetY(this Vector3 v, float y = 0) {
        v.y = y;
        return v;
    }

    public static void SetDirection(this Transform t, Vector2 dir) {
        t.up = dir;
        if (t.eulerAngles.x != 0 || t.eulerAngles.y != 0)
            t.eulerAngles = new Vector3(0, 0, -t.eulerAngles.z);
    }

    public static Vector3 GetClampedDirection(this Vector3 source, ref Vector3 target, float maxDistance) {
        var direction = target - source;
        if (direction.magnitude > maxDistance) {
            direction = direction.normalized;
            target = source + direction * maxDistance;
        }
        return direction;
    }

    public static float InverseLerp(this Vector2 value, Vector2 a, Vector2 b) {
        Vector2 AB = b - a;
        Vector2 AV = value - a;
        return Vector2.Dot(AV, AB) / Vector2.Dot(AB, AB);
    }

    public static float InverseLerp(this Vector3 value, Vector3 a, Vector3 b) {
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
    }

    public static Vector2Int GetSpiral(int index) {
        Vector2Int pos = Vector2Int.zero;
        int stepSize = 1;
        int xDirection = 1;
        int yDirection = -1;
        int diff;
        for (int i = 0; i < index; i++) {
            diff = index - (i + stepSize);
            if (diff < 0) {
                pos.x += (index + diff) * xDirection;
                break;
            }
            pos.x += stepSize * xDirection;
            i += stepSize;
            xDirection *= -1;

            diff = index - (i + stepSize);
            if (diff < 0) {
                pos.y += (index + diff) * yDirection;
                break;
            }
            pos.y += stepSize * yDirection;
            i += stepSize;
            yDirection *= -1;

            stepSize++;
        }
        return pos;
    }

    public static Vector2 GetTriangleFormation(int index) {
        int x = 0;
        int y = 0;
        int i = 0;
        for (; i < index; i++) {
            x = 0;
            for (int i2 = 0; i2 < y + 1; i2++) {
                if (i + i2 == index - 1) break;
                if (x > 0) {
                    x -= i2 + 1;
                } else {
                    x += i2 + 1;
                }
            }
            y++;
            i += y;
        }
        if (y % 2 == 0) {
            return new Vector2(x, -y);
        } else {
            return new Vector2(x - .5f, -y);
        }
    }

}
