using System;
using UnityEngine;

public interface ISide
{
    Side MirrorSide();

    Side RotateSide();

    void Add(Vector2 v);
}