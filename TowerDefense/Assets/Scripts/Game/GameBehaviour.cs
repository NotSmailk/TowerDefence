using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class GameBehaviour : MonoBehaviour
{ 
    public virtual bool GameUpdate() => true;

    public abstract void Recycle();
}
