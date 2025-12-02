using System.Diagnostics;
using UnityEngine;

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class GameManager : MonoBehaviour
{


    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}
