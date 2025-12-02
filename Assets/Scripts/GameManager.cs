using System.Diagnostics;
using UnityEngine;

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class GameManager
{


    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}
