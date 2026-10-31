using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DeathPanelController : MonoBehaviour
{
    [Header ("Global Reference")]
    [SerializeField] Globals globals;

    [Header("Trigger")]
    [Tooltip("Set this true (from code or the Inspector) to show the death panel.")]
    public bool isDead = false;

    [Header("Panel")]
    [Tooltip("The death panel GameObject. Title and buttons must be CHILDREN of it to fade.")]
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private float fadeDuration = 1.5f;
    [Range(0, 255)]
    [SerializeField] private int targetAlpha = 250;

    [Header("Title Font Flash")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_FontAsset fontA;
    [SerializeField] private TMP_FontAsset fontB;
    [Tooltip("Seconds between font swaps.")]
    [SerializeField] private float flashInterval = 0.15f;

    [Header("Buttons")]
    [Tooltip("Button GameObjects (Retry, Quit, etc.). Hidden until the delay passes.")]
    [SerializeField] private GameObject[] buttons;
    [Tooltip("Seconds after the panel turns on before the buttons appear.")]
    [SerializeField] private float buttonDelay = 3f;
    [Tooltip("Clicking a button changes its label from Font A to Font B.")]
    [SerializeField] private bool buttonClickFontSwap = true;

    [Header("Restart")]
    [Tooltip("Seconds to wait before reloading, so the Font B change is visible. 0 = instant.")]
    [SerializeField] private float restartDelay = 0.25f;

    [Header("Timing")]
    [Tooltip("Use unscaled time so it still animates if Time.timeScale is 0 on death.")]
    [SerializeField] private bool useUnscaledTime = true;

    [Header("Debug")]
    [Tooltip("Logs setup problems and fade progress to the Console.")]
    [SerializeField] private bool debugLogs = true;

    private CanvasGroup panelGroup;
    private TMP_Text[] buttonLabels;
    private bool lastState;
    private bool restarting;
    private Coroutine fadeRoutine;
    private Coroutine flashRoutine;
    private Coroutine buttonRoutine;

    private float DeltaTime => useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

    private void Awake()
    {
        if (deathPanel != null)
        {
            panelGroup = deathPanel.GetComponent<CanvasGroup>();
            if (panelGroup == null) panelGroup = deathPanel.AddComponent<CanvasGroup>();
        }

        if (debugLogs) CheckSetup();
        SetupButtonClicks();

        HidePanel();
        lastState = isDead;
        if (isDead) ShowPanel();
    }

    private void Update()
    {
        if(globals == null)
        {
            globals = GameObject.Find("GlobalObject").GetComponent<Globals>();
        }

        // Watch the bool for changes so it works from the Inspector or any script.
        if (isDead != lastState)
        {
            lastState = isDead;
            if (isDead)
            {
                globals.enableMovement = false;
                Cursor.lockState = CursorLockMode.None;
                ShowPanel();
            }
            else HidePanel();
        }
    }

    /// <summary>Call this from your player/health script, e.g. canvas.TriggerDeath();</summary>
    public void TriggerDeath() => isDead = true;

    /// <summary>Call this on respawn/restart.</summary>
    public void ResetDeath() => isDead = false;

    /// <summary>
    /// Reloads the current scene. Select this in the Restart button's OnClick list.
    /// </summary>
    public void RestartScene()
    {
        if (restarting) return; // ignore double clicks
        restarting = true;
        StartCoroutine(RestartRoutine());
    }

    private IEnumerator RestartRoutine()
    {
        if (restartDelay > 0f)
        {
            if (useUnscaledTime) yield return new WaitForSecondsRealtime(restartDelay);
            else yield return new WaitForSeconds(restartDelay);
        }

        Time.timeScale = 1f; // in case the game was paused on death
        if (debugLogs) Debug.Log("DeathPanel: reloading scene", this);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ShowPanel()
    {
        if (deathPanel == null || panelGroup == null)
        {
            Debug.LogWarning("DeathPanelController: deathPanel not assigned.", this);
            return;
        }

        StopRoutines();

        deathPanel.SetActive(true);
        panelGroup.alpha = 0f;
        panelGroup.interactable = false;
        panelGroup.blocksRaycasts = false;
        SetButtonsActive(false);
        ResetButtonFonts();

        if (debugLogs) Debug.Log($"DeathPanel: showing, fading 0 -> {targetAlpha} over {fadeDuration}s", this);

        fadeRoutine = StartCoroutine(FadeIn());
        buttonRoutine = StartCoroutine(ShowButtonsAfterDelay());
        if (titleText != null && fontA != null && fontB != null)
            flashRoutine = StartCoroutine(FlashFonts());
    }

    private void HidePanel()
    {
        StopRoutines();

        if (panelGroup != null)
        {
            panelGroup.alpha = 0f;
            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;
        }
        SetButtonsActive(false);
        ResetButtonFonts();
        if (titleText != null && fontA != null) titleText.font = fontA;

        // Don't deactivate ourselves if this script was put on the panel by mistake.
        if (deathPanel != null && deathPanel != gameObject) deathPanel.SetActive(false);
    }

    private IEnumerator FadeIn()
    {
        float target = targetAlpha / 255f;

        if (fadeDuration <= 0f)
        {
            if (debugLogs) Debug.LogWarning("DeathPanel: Fade Duration is 0, so it snaps instead of fading.", this);
        }
        else
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                t += DeltaTime;
                panelGroup.alpha = Mathf.Lerp(0f, target, t / fadeDuration);
                yield return null;
            }
        }

        panelGroup.alpha = target;
        panelGroup.interactable = true;
        panelGroup.blocksRaycasts = true;
        if (debugLogs) Debug.Log($"DeathPanel: fade finished, alpha = {panelGroup.alpha:F2}", this);
        fadeRoutine = null;
    }

    private IEnumerator ShowButtonsAfterDelay()
    {
        if (useUnscaledTime) yield return new WaitForSecondsRealtime(buttonDelay);
        else yield return new WaitForSeconds(buttonDelay);

        panelGroup.interactable = true;
        panelGroup.blocksRaycasts = true;
        SetButtonsActive(true);
        if (debugLogs) Debug.Log("DeathPanel: buttons shown", this);
        buttonRoutine = null;
    }

    private IEnumerator FlashFonts()
    {
        bool useA = true;
        while (true)
        {
            titleText.font = useA ? fontA : fontB;
            useA = !useA;

            if (useUnscaledTime) yield return new WaitForSecondsRealtime(flashInterval);
            else yield return new WaitForSeconds(flashInterval);
        }
    }

    // Hooks each button's onClick so its label switches to Font B when clicked.
    // onClick fires for mouse clicks and keyboard/controller submit.
    private void SetupButtonClicks()
    {
        if (buttons == null) { buttonLabels = new TMP_Text[0]; return; }

        buttonLabels = new TMP_Text[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            GameObject b = buttons[i];
            if (b == null) continue;

            buttonLabels[i] = b.GetComponentInChildren<TMP_Text>(true);

            if (debugLogs && buttonLabels[i] == null)
                Debug.LogWarning($"DeathPanel SETUP: button '{b.name}' has no TextMeshPro label, so its font can't change.", b);

            Button btn = b.GetComponent<Button>();
            if (btn == null)
            {
                if (debugLogs) Debug.LogWarning($"DeathPanel SETUP: '{b.name}' has no Button component.", b);
                continue;
            }

            TMP_Text label = buttonLabels[i];
            btn.onClick.AddListener(() =>
            {
                if (buttonClickFontSwap && label != null && fontB != null) label.font = fontB;
            });
        }
    }

    private void ResetButtonFonts()
    {
        if (buttonLabels == null || fontA == null) return;
        foreach (TMP_Text label in buttonLabels)
            if (label != null) label.font = fontA;
    }

    private void SetButtonsActive(bool active)
    {
        if (buttons == null) return;
        foreach (GameObject b in buttons)
            if (b != null) b.SetActive(active);
    }

    private void StopRoutines()
    {
        if (fadeRoutine != null) { StopCoroutine(fadeRoutine); fadeRoutine = null; }
        if (flashRoutine != null) { StopCoroutine(flashRoutine); flashRoutine = null; }
        if (buttonRoutine != null) { StopCoroutine(buttonRoutine); buttonRoutine = null; }
    }

    // Reports common setup problems. Changes nothing.
    private void CheckSetup()
    {
        if (deathPanel == null)
        {
            Debug.LogWarning("DeathPanel SETUP: Death Panel field is empty.", this);
            return;
        }

        if (deathPanel == gameObject)
            Debug.LogWarning("DeathPanel SETUP: this script is on the death panel itself. Move it to the Canvas " +
                             "(or another always-active object), otherwise hiding the panel switches the script off.", this);

        Transform panel = deathPanel.transform;

        if (titleText != null && !titleText.transform.IsChildOf(panel))
            Debug.LogWarning($"DeathPanel SETUP: title '{titleText.name}' is NOT a child of '{deathPanel.name}', " +
                             "so it won't fade and will show straight away. Drag it under the panel.", titleText);

        if (buttons == null || buttons.Length == 0)
            Debug.LogWarning("DeathPanel SETUP: Buttons array is empty, so buttons won't wait for the delay.", this);
        else
            foreach (GameObject b in buttons)
                if (b != null && !b.transform.IsChildOf(panel))
                    Debug.LogWarning($"DeathPanel SETUP: button '{b.name}' is NOT a child of '{deathPanel.name}', " +
                                     "so it won't fade. Drag it under the panel.", b);

        foreach (CanvasGroup cg in deathPanel.GetComponentsInChildren<CanvasGroup>(true))
            if (cg != panelGroup && cg.ignoreParentGroups)
                Debug.LogWarning($"DeathPanel SETUP: '{cg.name}' has a CanvasGroup with Ignore Parent Groups ticked, " +
                                 "so it won't fade. Untick it.", cg);

        Animator anim = deathPanel.GetComponentInParent<Animator>();
        if (anim != null)
            Debug.LogWarning($"DeathPanel SETUP: Animator on '{anim.name}' may be overriding alpha/active state.", anim);

        Image img = deathPanel.GetComponent<Image>();
        if (img != null && img.color.a < 0.05f)
            Debug.LogWarning("DeathPanel SETUP: the panel's Image colour alpha is almost 0, so the background " +
                             "stays invisible even when the fade works. Set its colour alpha to 255.", img);
    }
}
