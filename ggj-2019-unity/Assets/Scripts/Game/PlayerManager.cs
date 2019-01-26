using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
  [SerializeField]
  private PlayerController[] players = null;

  [SerializeField]
  private Camera defaultCamera = null;

  private SplitscreenLayout splitscreenLayout = new SplitscreenLayout();
  private List<PlayerController> joinedPlayers = new List<PlayerController>();

  private void Awake()
  {
    defaultCamera.gameObject.SetActive(true);
    foreach (PlayerController player in players)
    {
      player.gameObject.SetActive(false);
    }
  }

  private void Update()
  {
    if (!Rewired.ReInput.isReady)
      return;

    // Listen for input from all players and join them when they press something
    for (int i = 0; i < Rewired.ReInput.players.playerCount; ++i)
    {
      Rewired.Player playerInput = Rewired.ReInput.players.GetPlayer(i);
      PlayerController player = players[i];
      if (playerInput.GetAnyButtonDown() && !joinedPlayers.Contains(player))
      {
        AddPlayer(playerInput);
      }
    }
  }

  private void AddPlayer(Rewired.Player playerInput)
  {
    // Activate the player object and mark the joined state
    PlayerController player = players[playerInput.id];
    joinedPlayers.Add(player);
    player.PlayerInput = playerInput;
    player.gameObject.SetActive(true);

    // Set up the player viewport 
    splitscreenLayout.AddCamera(player.Camera);
    defaultCamera.gameObject.SetActive(joinedPlayers.Count == 0);
  }

  private void RemovePlayer(Rewired.Player playerInput)
  {
    // Disable the player object
    PlayerController player = players[playerInput.id];
    joinedPlayers.Remove(player);
    player.gameObject.SetActive(false);

    // Remove player's camera
    splitscreenLayout.RemoveCamera(player.Camera);
    defaultCamera.gameObject.SetActive(joinedPlayers.Count == 0);
  }
}