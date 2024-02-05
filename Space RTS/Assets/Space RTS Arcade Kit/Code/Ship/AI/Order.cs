using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Order
{
    public string Name;

    public abstract void UpdateState(ShipAI controller);
    public abstract void Destroy();

}