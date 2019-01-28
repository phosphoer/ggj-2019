using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameFlow : MonoBehaviour
{
  [SerializeField]
  private float roundTime = 60.0f;

  [SerializeField]
  private float warnAtTimeLeft = 15.0f;

  [SerializeField]
  private GameObject waitingForPlayersPrompt = null;

  [SerializeField]
  private GameObject startGamePrompt = null;

  [SerializeField]
  private GameObject roundTimerRoot = null;

  [SerializeField]
  private Transform roundTimer1PlayerRoot = null;

  [SerializeField]
  private Text roundTimeText = null;

  [SerializeField]
  private SoundBank musicGame = null;

  [SerializeField]
  private SoundBank musicWaitForPlayers = null;

  [SerializeField]
  private SoundBank musicIntro = null;

  [SerializeField]
  private SoundBank playerJoinSound = null;

  private int lastRoundedTime;

  private void Awake()
  {
    waitingForPlayersPrompt.SetActive(false);
    startGamePrompt.SetActive(false);
    roundTimerRoot.SetActive(false);
  }

  private IEnumerator Start()
  {
    yield return StartCoroutine(JoinState());
    yield return StartCoroutine(IntroState());
    yield return StartCoroutine(GameState());
    yield return StartCoroutine(EndState());
  }

  private IEnumerator JoinState()
  {
    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;

    AudioManager.Instance.FadeInSound(gameObject, musicWaitForPlayers, 2.0f);

    // Wait for a minimum number of players to join
    waitingForPlayersPrompt.SetActive(true);
    PlayerManager.PlayerJoined += OnPlayerJoined;
    PlayerManager.Instance.IsJoiningEnabled = true;
    while (PlayerManager.Instance.JoinedPlayers.Count == 0)
    {
      yield return null;
    }

    // Wait for someone to press start 
    waitingForPlayersPrompt.SetActive(false);
    startGamePrompt.SetActive(true);
    bool startPressed = false;
    while (!startPressed)
    {
      foreach (PlayerController player in PlayerManager.Instance.JoinedPlayers)
      {
        if (player.PlayerInput.GetButtonDown(InputConsts.Action.Start))
        {
          startPressed = true;
        }
      }

      yield return null;
    }

    startGamePrompt.SetActive(false);
    PlayerManager.Instance.IsJoiningEnabled = false;
    PlayerManager.PlayerJoined -= OnPlayerJoined;

    AudioManager.Instance.FadeOutSound(gameObject, musicWaitForPlayers, 2.0f);

    yield return null;
  }

  private IEnumerator IntroState()
  {
    AudioManager.Instance.FadeInSound(gameObject, musicIntro, 2.0f);

    // Start TV intros and wait for them to complete
    Coroutine waitRoutine = null;
    foreach (TVSlideShow slideShow in TVSlideShow.Instances)
    {
      waitRoutine = slideShow.StartIntroSlides();
    }

    yield return waitRoutine;

    AudioManager.Instance.FadeOutSound(gameObject, musicIntro, 2.0f);
  }

  private IEnumerator GameState()
  {
    AudioManager.Instance.FadeInSound(gameObject, musicGame, 2.0f);
    CommentaryManager.Instance.StartCommentary();

    // Enable everyone's controls
    foreach (PlayerController player in PlayerManager.Instance.JoinedPlayers)
    {
      player.ControlsEnabled = true;
    }

    // Show round timer
    roundTimerRoot.SetActive(true);
    if (PlayerManager.Instance.JoinedPlayers.Count == 1)
      roundTimerRoot.transform.localPosition = roundTimer1PlayerRoot.transform.localPosition;

    // Loop while round timer is going 
    bool warned = false;
    for (float timer = roundTime; timer > 0; timer -= Time.deltaTime)
    {
      // Update timer text 
      int roundedTime = Mathf.RoundToInt(timer);
      if (roundedTime != lastRoundedTime)
      {
        lastRoundedTime = roundedTime;
        roundTimeText.text = roundedTime.ToString();
      }

      // Time limit warning 
      if (!warned && timer < warnAtTimeLeft)
      {
        WarningLight.TurnOn();
        warned = true;
        roundTimeText.color = Color.red;
      }

      yield return null;
    }

    // End of round 
    // Disable everyone's controls
    roundTimerRoot.SetActive(false);
    foreach (PlayerController player in PlayerManager.Instance.JoinedPlayers)
    {
      player.ControlsEnabled = false;
      player.ResetToStartPosition();
    }

    AudioManager.Instance.FadeOutSound(gameObject, musicGame, 2.0f);
    AudioManager.Instance.FadeInSound(gameObject, musicIntro, 2.0f);
    CommentaryManager.Instance.StopCommentary();

    // Show outro 
    Coroutine waitRoutine = null;
    foreach (TVSlideShow slideShow in TVSlideShow.Instances)
    {
      waitRoutine = slideShow.StartOutroSlides();
    }

    yield return waitRoutine;
  }

  private IEnumerator EndState()
  {
    yield return null;

    // Reload scene
    // yield return SceneManager.UnloadSceneAsync(gameObject.scene.name);
    // yield return null;
    SceneManager.LoadScene(gameObject.scene.name, LoadSceneMode.Single);
  }

  private void OnPlayerJoined(PlayerController player)
  {
    AudioManager.Instance.PlaySound(playerJoinSound);
    player.ControlsEnabled = false;
  }
}