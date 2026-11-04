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
    SerializedProperty m_weaponSwapText;
    SerializedProperty m_pickedUpWeapon;
    SerializedProperty m_droppedWeapon;
    SerializedProperty m_loadedAmmo;
    SerializedProperty m_reserveAmmo;
    SerializedProperty m_storedLoadedAmmo;
    SerializedProperty m_storedReserveAmmo;
    SerializedProperty m_fireAudioSource;
    SerializedProperty m_fireSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        weapon = (Weapon)target;

        /*m_worldModel = serializedObject.FindProperty("worldModel");
        m_firstPersonModel= serializedObject.FindProperty("firstPersonModel");*/
        m_isDropped = serializedObject.FindProperty("isDropped");
        m_isEquiped = serializedObject.FindProperty("isEquiped");
        m_weaponPickUpText = serializedObject.FindProperty("weaponPickUpText");
        m_weaponSwapText = serializedObject.FindProperty("weaponSwapText");
        m_pickedUpWeapon = serializedObject.FindProperty("pickedUpWeapon");
        m_droppedWeapon = serializedObject.FindProperty("droppedWeapon");
        m_loadedAmmo = serializedObject.FindProperty("loadedAmmo");
        m_reserveAmmo = serializedObject.FindProperty("reserveAmmo");
        m_storedLoadedAmmo = serializedObject.FindProperty("storedLoadedAmmo");
        m_storedReserveAmmo = serializedObject.FindProperty("storedReserveAmmo");
        m_fireAudioSource = serializedObject.FindProperty("fireAudioSource");
        m_fireSound = serializedObject.FindProperty("fireSound");
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(m_pickedUpWeapon);
        EditorGUILayout.PropertyField(m_droppedWeapon);

        EditorGUILayout.PropertyField(m_isDropped);
        EditorGUILayout.PropertyField(m_isEquiped);

        if(m_isDropped.boolValue)
        {
            EditorGUILayout.PropertyField(m_weaponPickUpText);
            EditorGUILayout.PropertyField(m_weaponSwapText);
            EditorGUILayout.PropertyField(m_storedLoadedAmmo);
            EditorGUILayout.PropertyField(m_storedReserveAmmo);
        }

        if(m_isEquiped.boolValue)
        {
            EditorGUILayout.PropertyField(m_loadedAmmo);
            EditorGUILayout.PropertyField(m_reserveAmmo);
            EditorGUILayout.PropertyField(m_fireAudioSource);
            EditorGUILayout.PropertyField(m_fireSound);
        }

        if(EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
