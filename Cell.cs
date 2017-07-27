using UnityEngine;
using System.Collections;

public struct Cell 
{
    public bool cellState;
    public short cellAge;

    public Cell(bool state, short age)
    {
        this.cellState = state;
        this.cellAge = age;
    }

}
