using System;
using UnityEngine;

// -------------------------------------------
//  int => float
// -------------------------------------------
[Serializable]
public class DictStringInt : SerializableDict<string, int> { }

// -------------------------------------------
//  GameObject => float
// -------------------------------------------
[Serializable]
public class DictGameObjectFloat : SerializableDict<GameObject, float> { }

// -------------------------------------------
//  EGroundType => AudioClip[]
// -------------------------------------------
[Serializable]
public class DictEGroundTypeAudioClipCollection : SerializableDict<EGroundType, AudioClipCollection> { }

// -------------------------------------------
//  string => GameObject
// -------------------------------------------
[Serializable]
public class DictStringGameObject : SerializableDict<string, GameObject> { }