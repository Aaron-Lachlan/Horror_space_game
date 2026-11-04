using UnityEditor;

[CustomEditor(typeof(SlidingDoor))]
public class DoorEditor : Editor
{
    SlidingDoor slidingDoor;

    SerializedProperty m_isButtonDoor;
    SerializedProperty m_interactCollider;
    SerializedProperty m_isLargeDoor;
    SerializedProperty m_doorOpenMessage;
    SerializedProperty m_doorCloseMessage;
    SerializedProperty m_animator;
    SerializedProperty m_playerTag;
    SerializedProperty m_enemyTag;
    SerializedProperty m_openCloseAnimBoolName;
    SerializedProperty m_isOpenOrClosed;
    SerializedProperty m_audioSource;
    SerializedProperty m_audioSourceAlarm;
    SerializedProperty m_openAudio;
    SerializedProperty m_closeAudio;
    SerializedProperty m_doorOpen;
    SerializedProperty m_animLayer;
    SerializedProperty m_animPlaying;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        slidingDoor = (SlidingDoor)target;

        m_isButtonDoor = serializedObject.FindProperty("isButtonDoor");
        m_interactCollider= serializedObject.FindProperty("interactCollider");
        m_isLargeDoor = serializedObject.FindProperty("isLargeDoor");
        m_doorOpenMessage = serializedObject.FindProperty("doorOpenMessage");
        m_doorCloseMessage = serializedObject.FindProperty("doorCloseMessage");
        m_animator = serializedObject.FindProperty("animator");
        m_playerTag = serializedObject.FindProperty("playerTag");
        m_enemyTag = serializedObject.FindProperty("enemyTag");
        m_openCloseAnimBoolName = serializedObject.FindProperty("openCloseAnimBoolName");
        m_isOpenOrClosed = serializedObject.FindProperty("isOpenOrClosed");
        m_audioSource = serializedObject.FindProperty("audioSource");
        m_audioSourceAlarm = serializedObject.FindProperty("audioSourceAlarm");
        m_openAudio = serializedObject.FindProperty("openAudio");
        m_closeAudio = serializedObject.FindProperty("closeAudio");
        m_doorOpen = serializedObject.FindProperty("doorOpen");
        m_animLayer = serializedObject.FindProperty("animLayer");
        m_animPlaying = serializedObject.FindProperty("animPlaying");
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(m_isButtonDoor);
        EditorGUILayout.PropertyField(m_isLargeDoor);
        EditorGUILayout.PropertyField(m_animator);
        
        if(m_isButtonDoor.boolValue)
        {
            EditorGUILayout.PropertyField(m_interactCollider);
            EditorGUILayout.PropertyField(m_doorOpenMessage);
            EditorGUILayout.PropertyField(m_doorCloseMessage);
        }

        if(!m_isButtonDoor.boolValue)
        {
            EditorGUILayout.PropertyField(m_playerTag);
            EditorGUILayout.PropertyField(m_enemyTag);
        }
        
        EditorGUILayout.PropertyField(m_openCloseAnimBoolName);
        EditorGUILayout.PropertyField(m_isOpenOrClosed);
        EditorGUILayout.PropertyField(m_audioSource);
        EditorGUILayout.PropertyField(m_audioSourceAlarm);
        EditorGUILayout.PropertyField(m_openAudio);
        EditorGUILayout.PropertyField(m_closeAudio);
        EditorGUILayout.PropertyField(m_doorOpen);
        EditorGUILayout.PropertyField(m_animLayer);
        EditorGUILayout.PropertyField(m_animPlaying);

        if(EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
