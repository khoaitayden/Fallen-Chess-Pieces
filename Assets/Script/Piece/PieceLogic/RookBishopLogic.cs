using System.Collections.Generic;
using UnityEngine;

// A Rook + Bishop is functionally a Queen. We can just reuse the Queen's logic.
public class RookBishopLogic : QueenLogic 
{
    // By inheriting from QueenLogic, we get all of its methods for free.
    // We don't need to write any code here at all!
}