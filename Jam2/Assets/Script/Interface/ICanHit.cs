using System.Collections.Generic;
using UnityEngine;

public interface ICanHit
{
    public void OnTouch(List<Collider2D> hits);
}
