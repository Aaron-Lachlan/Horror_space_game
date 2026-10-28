using UnityEditor;

[CustomEditor(typeof(SlidingDoor))]
public class DoorEditor : Editor
{
    SlidingDoor slidingDoor;

    SerializedProperty m_isButtonDoor;
    SerializedProperty m_collisionAdjustmentNeeded;
    SerializedProperty m_collider;
    SerializedProperty m_colliderSizeClosed;
    SerializedProperty m_colliderSizeOpen;
    SerializedProperty m_colliderCentreClosed;
    SerializedProperty m_colliderCentreOpen;
    SerializedProperty m_isLargeDoor;
    SerializedProperty m_doorMessage;
    //SerializedProperty m_onDoorInteraction;
    SerializedProperty m_animator;
    SerializedProperty m_playerTag;
    SerializedProperty m_openCloseAnimBoolName;
    SerializedProperty m_isOpenOrClosed;
    SerializedProperty m_audioSource;
    SerializedProperty m_audioSourceAlarm;
    SerializedProperty m_openAudio;
    SerializedProperty m_closeAudio;
    SerializedProperty m_doorOpen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        slidingDoor = (SlidingDoor)target;

        m_isButtonDoor = serializedObject.FindProperty("isButtonDoor");
        m_collisionAdjustmentNeeded = serializedObject.FindProperty("collisionAdjustmentNeeded");
        m_collider= serializedObject.FindProperty("collider");
        m_colliderSizeClosed= serializedObject.FindProperty("colliderSizeClosed");
        m_colliderSizeOpen= serializedObject.FindProperty("colliderSizeOpen");
        m_colliderCentreClosed= serializedObject.FindProperty("colliderCentreClosed");
        m_colliderCentreOpen= serializedObject.FindProperty("colliderCentreOpen");
        m_isLargeDoor = serializedObject.FindProperty("isLargeDoor");
        m_doorMessage = serializedObject.FindProperty("doorMessage");
        //m_onDoorInteraction = serializedObject.FindProperty("onDoorInteraction");
        m_animator = serializedObject.FindProperty("animator");
        m_playerTag = serializedObject.FindProperty("playerTag");
        m_openCloseAnimBoolName = serializedObject.FindProperty("openCloseAnimBoolName");
        m_isOpenOrClosed = serializedObject.FindProperty("isOpenOrClosed");
        m_audioSource = serializedObject.FindProperty("audioSource");
        m_audioSourceAlarm = serializedObject.FindProperty("audioSourceAlarm");
        m_openAudio = serializedObject.FindProperty("openAudio");
        m_closeAudio = serializedObject.FindProperty("closeAudio");
        m_doorOpen = serializedObject.FindProperty("doorOpen");
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(m_isButtonDoor);
        
        if(m_isButtonDoor.boolValue)
        {
            EditorGUILayout.PropertyField(m_doorMessage);
            EditorGUILayout.PropertyField(m_collisionAdjustmentNeeded);
            if(m_collisionAdjustmentNeeded.boolValue)
            {
                EditorGUILayout.PropertyField(m_collider);
                EditorGUILayout.PropertyField(m_colliderSizeClosed);
                EditorGUILayout.PropertyField(m_colliderSizeOpen);
                EditorGUILayout.PropertyField(m_colliderCentreClosed);
                EditorGUILayout.PropertyField(m_colliderCentreOpen);
            }
        }

        EditorGUILayout.PropertyField(m_isLargeDoor);

        /*if(m_isLargeDoor.boolValue)
        {
            //EditorGUILayout.PropertyField(m_onDoorInteraction);
        }*/

        EditorGUILayout.PropertyField(m_animator);

        if(!m_isLargeDoor.boolValue)
        {
            EditorGUILayout.PropertyField(m_playerTag);
        }
        
        EditorGUILayout.PropertyField(m_openCloseAnimBoolName);
        EditorGUILayout.PropertyField(m_isOpenOrClosed);
        EditorGUILayout.PropertyField(m_audioSource);
        EditorGUILayout.PropertyField(m_audioSourceAlarm);
        EditorGUILayout.PropertyField(m_openAudio);
        EditorGUILayout.PropertyField(m_closeAudio);
        EditorGUILayout.PropertyField(m_doorOpen);

        if(EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
