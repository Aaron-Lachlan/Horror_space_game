using UnityEditor;

[CustomEditor(typeof(Weapon))]
public class WeaponEditor : Editor
{
    Weapon weapon;

    SerializedProperty m_isDropped;
    SerializedProperty m_isEquiped;
    /*SerializedProperty m_worldModel;
    SerializedProperty m_firstPersonModel;*/
    SerializedProperty m_weaponPickUpText;
    SerializedProperty m_weaponAnimator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        weapon = (Weapon)target;

        /*m_worldModel = serializedObject.FindProperty("worldModel");
        m_firstPersonModel= serializedObject.FindProperty("firstPersonModel");*/
        m_isDropped = serializedObject.FindProperty("isDropped");
        m_isEquiped = serializedObject.FindProperty("isEquiped");
        m_weaponPickUpText = serializedObject.FindProperty("weaponPickUpText");
        m_weaponAnimator = serializedObject.FindProperty("weaponAnimator");
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        /*EditorGUILayout.PropertyField(m_worldModel);
        EditorGUILayout.PropertyField(m_firstPersonModel);*/

        EditorGUILayout.PropertyField(m_isDropped);
        EditorGUILayout.PropertyField(m_isEquiped);

        if(m_isDropped.boolValue)
        {
            EditorGUILayout.PropertyField(m_weaponPickUpText);
        }
        
        if(m_isEquiped.boolValue)
        {
            EditorGUILayout.PropertyField(m_weaponAnimator);
        }

        if(EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
