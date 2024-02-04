using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AutoHandNaming))]
public class HandVisualAutoNamer : Editor
{
    private SerializedProperty targetGameObjectProp;
    private static readonly string[] _fbxHandSidePrefix = { "l_", "r_" };
    private static readonly string _fbxHandBonePrefix = "b_";

    private readonly string[] _boneNames =
    {
        "index", "middle", "pinky", "ring", "thumb"
    };
    
    private int _selectedHandIndex = 0;
    private int _currentHandIndex = 0;
    private string[] _handOptions = new string[] { "Left Hand", "Right Hand" };

    private void OnEnable()
    {
        targetGameObjectProp = serializedObject.FindProperty("Wrist");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();
        
        EditorGUILayout.LabelField("Hand Type", EditorStyles.boldLabel);
        
        _selectedHandIndex = EditorGUILayout.Popup("Select Hand", _selectedHandIndex, _handOptions);

        if (GUILayout.Button("Auto Name Mapping"))
        {
            AutoNameMapping(_selectedHandIndex);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void AutoNameMapping(int handIndex)
    {
        GameObject targetGameObject = targetGameObjectProp.objectReferenceValue as GameObject;
        if (targetGameObject == null)
        {
            Debug.LogError("Target GameObject is not set.");
            return;
        }

        Transform rootTransform = targetGameObject.transform;
        rootTransform.gameObject.name = _fbxHandBonePrefix + _fbxHandSidePrefix[handIndex] + "wrist";
        _currentHandIndex = 0;
        foreach (Transform child in rootTransform)
        {
            rootTransform.GetChild(0).name = _fbxHandBonePrefix + _fbxHandSidePrefix[handIndex] + "forearm_stub";
           
            NameFingerBones(child, _fbxHandSidePrefix[handIndex], _boneNames[_currentHandIndex> 4 ? 4 : _currentHandIndex], 0);
        }
        
        rootTransform.GetChild(0).name = _fbxHandBonePrefix + _fbxHandSidePrefix[handIndex] + "forearm_stub";
        EditorUtility.SetDirty(targetGameObject);
    }

    private void NameFingerBones(Transform bone, string sidePrefix, string boneName, int depth)
    {
        
        if (bone.childCount == 0)
        {
            if (bone.name == "b_r_forearm_stub" || bone.name == "b_l_forearm_stub")
            {
                
            }
            else
            {
                Debug.Log("큰일2");
                _currentHandIndex++;
                if(_currentHandIndex >= 4) return;
            
                bone.name = _fbxHandBonePrefix + sidePrefix + boneName + "_finger_tip_marker";
            }
        }
        else
        {
            if(bone.name != "b_l_wrist" 
               || bone.name != "b_r_wrist" 
               || bone.name != "b_l_forearm_stub" 
               || bone.name != "b_r_forearm_stub")
            {
             
                bone.name = _fbxHandBonePrefix + sidePrefix + boneName + depth;
            }
            
            foreach (Transform child in bone)
            {
                
                NameFingerBones(child, sidePrefix, _boneNames[_currentHandIndex > 4 ? 4 : _currentHandIndex], depth + 1);
            }
        }
    }

}