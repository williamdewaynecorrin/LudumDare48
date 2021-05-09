using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

// -------------------------------------------
//  string => int
// -------------------------------------------
[UnityEditor.CustomPropertyDrawer(typeof(DictStringInt))]
public class StringIntDictionaryDrawer : SerializableDictDrawer<string, int>
{
    protected override SerializableKeyValueTemplate<string, int> GetTemplate()
    {
        return GetGenericTemplate<SerializableStringIntTemplate>();
    }
}
internal class SerializableStringIntTemplate : SerializableKeyValueTemplate<string, int> { }

// -------------------------------------------
//  GameObject => float
// -------------------------------------------
[UnityEditor.CustomPropertyDrawer(typeof(DictGameObjectFloat))]
public class GameObjectFloatDictionaryDrawer : SerializableDictDrawer<GameObject, float>
{
    protected override SerializableKeyValueTemplate<GameObject, float> GetTemplate()
    {
        return GetGenericTemplate<SerializableGameObjectFloatTemplate>();
    }
}
internal class SerializableGameObjectFloatTemplate : SerializableKeyValueTemplate<GameObject, float> { }

// -------------------------------------------
//  EGroundType => Audioclip[]
// -------------------------------------------
[UnityEditor.CustomPropertyDrawer(typeof(DictEGroundTypeAudioClipCollection))]
public class DictEGroundTypeAudioClipCollectionDictionaryDrawer : SerializableDictDrawer<EGroundType, AudioClipCollection>
{
    protected override SerializableKeyValueTemplate<EGroundType, AudioClipCollection> GetTemplate()
    {
        return GetGenericTemplate<SerializableEGroundTypeAudioClipCollectionTemplate>();
    }
}
internal class SerializableEGroundTypeAudioClipCollectionTemplate : SerializableKeyValueTemplate<EGroundType, AudioClipCollection> { }

// -------------------------------------------
//  string => GameObject
// -------------------------------------------
[UnityEditor.CustomPropertyDrawer(typeof(DictStringGameObject))]
public class DictStringGameObjectDictionaryDrawer : SerializableDictDrawer<string, GameObject>
{
    protected override SerializableKeyValueTemplate<string, GameObject> GetTemplate()
    {
        return GetGenericTemplate<SerializableDictStringGameObjectTemplate>();
    }
}
internal class SerializableDictStringGameObjectTemplate : SerializableKeyValueTemplate<string, GameObject> { }