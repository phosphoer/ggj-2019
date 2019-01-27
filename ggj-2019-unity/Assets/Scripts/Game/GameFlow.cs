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

    yield return null;
  }

  private IEnumerator IntroState()
  {
    TVSlideShow[] slideShows = FindObjectsOfType<TVSlideShow>();
    Coroutine waitRoutine = null;
    foreach (TVSlideShow slideShow in slideShows)
    {
      waitRoutine = slideShow.StartSlideShow();
    }

    yield return waitRoutine;
  }

  private IEnumerator GameState()
  {
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
      if (timer < warnAtTimeLeft)
      {

      }

      yield return null;
    }

    // End of round 
    // Disable everyone's controls
    roundTimerRoot.SetActive(false);
    foreach (PlayerController player in PlayerManager.Instance.JoinedPlayers)
    {
      player.ControlsEnabled = false;
    }

    // Tally up scores 

    yield return null;
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
    player.ControlsEnabled = false;
  }
}