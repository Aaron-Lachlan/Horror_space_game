using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instace { get; private set;}

    private Vector2 cursorHotspot;

    [SerializeField] List<CursorAnimation> cursorAnimationList;

    private CursorAnimation cursorAnimation;
    private int currentFrame;
    private float frameTimer;
    [SerializeField] int frameCount;
    
    public enum CursorType
    {
        Arrow,
        ButtonHover,
        ButtonClicked,
        Loading,
    }

    void Start()
    {
        //cursorHotspot = new Vector2(cursorTextureArray[0].width / 2, cursorTextureArray[0].height / 2);
        SetActiveCursorType(CursorType.Arrow);
    }

    void Awake()
    {
        Instace = this;
    }

    // Update is called once per frame
    void Update()
    {
        frameTimer -= Time.deltaTime;
        if(frameTimer <= 0f)
        {
            frameTimer += cursorAnimation.frameRate;
            currentFrame = (currentFrame + 1) % frameCount;
            Cursor.SetCursor(cursorAnimation.textureArray[currentFrame], cursorAnimation.offset, CursorMode.Auto);
        }
    }

    public void SetActiveCursorType(CursorType cursorType)
    {
        SetActiveCursorAnimation(GetCursorAnimation(cursorType));
    }

    CursorAnimation GetCursorAnimation(CursorType cursorType)
    {
        foreach(CursorAnimation cursorAnimation in cursorAnimationList)
        {
            if(cursorAnimation.cursorType == cursorType)
            {
                return cursorAnimation;
            }
        }
        //If faild to find CursorType
        return null;
    }

    void SetActiveCursorAnimation(CursorAnimation cursorAnimation)
    {
        this.cursorAnimation = cursorAnimation;
        currentFrame = 0;
        frameTimer = cursorAnimation.frameRate;
        frameCount = cursorAnimation.textureArray.Length;
    }

    [System.Serializable]
    public class CursorAnimation
    {
        public CursorType cursorType;
        public Texture2D[] textureArray;
        public float frameRate;
        public Vector2 offset;
    }
}
