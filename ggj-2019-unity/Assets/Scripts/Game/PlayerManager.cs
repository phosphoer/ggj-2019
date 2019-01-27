using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
  public static PlayerManager Instance { get; private set; }
  public static event System.Action<PlayerController> PlayerJoined;

  public IReadOnlyList<PlayerController> JoinedPlayers { get { return joinedPlayers; } }

  [SerializeField]
  private PlayerController[] players = null;

  [SerializeField]
  private Camera defaultCamera = null;

  private SplitscreenLayout splitscreenLayout = new SplitscreenLayout();
  private List<PlayerController> joinedPlayers = new List<PlayerController>();
  private List<Rewired.Player> joinedPlayerInputs = new List<Rewired.Player>();

  private void Awake()
  {
    Instance = this;
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
      if (playerInput.GetButtonDown(InputConsts.Action.Interact) && !joinedPlayerInputs.Contains(playerInput))
      {
        AddPlayer(playerInput);
      }
    }
  }

  private void AddPlayer(Rewired.Player playerInput)
  {
    // Activate the player object and mark the joined state
    PlayerController player = players[joinedPlayers.Count];
    joinedPlayers.Add(player);
    joinedPlayerInputs.Add(playerInput);
    player.PlayerInput = playerInput;
    player.gameObject.SetActive(true);

    // If this was the keyboard player, remove any joystics they have have stolen
    Rewired.Controller controller = playerInput.controllers.GetLastActiveController();
    if (controller.type == Rewired.ControllerType.Mouse || controller.type == Rewired.ControllerType.Keyboard)
    {
      if (playerInput.controllers.joystickCount > 0)
        AssignJoystickToNextOpenPlayer(playerInput.controllers.Joysticks[0]);
    }

    // Set up the player viewport 
    splitscreenLayout.AddCamera(player.ViewportCamera);
    defaultCamera.gameObject.SetActive(joinedPlayers.Count == 0);

    PlayerJoined?.Invoke(player);
  }

  private void RemovePlayer(Rewired.Player playerInput)
  {
    // Disable the player object
    PlayerController player = null;
    foreach (PlayerController p in joinedPlayers)
    {
      if (player.PlayerInput == playerInput)
      {
        player = p;
        break;
      }
    }

    if (player != null)
    {
      joinedPlayers.Remove(player);
      joinedPlayerInputs.Remove(playerInput);
      player.gameObject.SetActive(false);

      // Remove player's camera
      splitscreenLayout.RemoveCamera(player.ViewportCamera);
      defaultCamera.gameObject.SetActive(joinedPlayers.Count == 0);
    }
  }

  private void AssignJoystickToNextOpenPlayer(Rewired.Joystick j)
  {
    foreach (Rewired.Player p in Rewired.ReInput.players.Players)
    {
      if (p.controllers.joystickCount > 0) continue;
      p.controllers.AddController(j, true);
      return;
    }
  }
}