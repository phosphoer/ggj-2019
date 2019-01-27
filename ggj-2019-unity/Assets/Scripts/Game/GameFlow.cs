using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameFlow : MonoBehaviour
{
  [SerializeField]
  private float roundTime = 60.0f;

  [SerializeField]
  private float warnAtTimeLeft = 15.0f;

  private IEnumerator Start()
  {
    yield return StartCoroutine(JoinState());
    yield return StartCoroutine(IntroState());
    yield return StartCoroutine(GameState());
    yield return StartCoroutine(GameState());
  }

  private IEnumerator JoinState()
  {
    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;

    // Wait for a minimum number of players to join
    PlayerManager.PlayerJoined += OnPlayerJoined;
    while (PlayerManager.Instance.JoinedPlayers.Count == 0)
    {
      yield return null;
    }

    // Wait for someone to press start 
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

    PlayerManager.PlayerJoined -= OnPlayerJoined;

    yield return null;
  }

  private IEnumerator IntroState()
  {
    yield return null;
  }

  private IEnumerator GameState()
  {
    // Enable everyone's controls
    foreach (PlayerController player in PlayerManager.Instance.JoinedPlayers)
    {
      player.ControlsEnabled = true;
    }

    // Count down round timer 
    for (float timer = roundTime; timer > 0; timer -= Time.deltaTime)
    {
      // Time limit warning 
      if (timer < warnAtTimeLeft)
      {

      }

      yield return null;
    }

    // End of round 
    // Disable everyone's controls
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
    SceneManager.LoadScene(gameObject.scene.name, LoadSceneMode.Single);
  }

  private void OnPlayerJoined(PlayerController player)
  {
    player.ControlsEnabled = false;
  }
}