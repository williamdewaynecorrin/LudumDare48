using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// =========================================================================================================
// -- Various delegate definitions 
// =========================================================================================================

// -- standard
public delegate void FunctionPointerV();
public delegate void FunctionPointerF(float v1);
public delegate void FunctionPointerI(int v1);
public delegate void FunctionPointerI2(int v1, int v2);
public delegate void FunctionPointerB(bool v1);

// -- specific
public delegate void FunctionPointerPlayerDeath(int playeridx);
public delegate void FunctionPointerPlayerHit(int shooteridx, int hitidx);
public delegate void FunctionPointerRoundOver(int playeridx);
public delegate void FunctionPointerPlayerSpawn(int playeridx);
public delegate void FunctionPointerRoundStart();
public delegate void FunctionPointerBallScored(int goalidx, Vector3 pos);
