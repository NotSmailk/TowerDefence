using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class WarEntity : GameBehaviour
{
    public WarFactory OriginFactory { get; set; }

    public override void Recycle()
    {
        OriginFactory.Reclaim(this);
    }
}
