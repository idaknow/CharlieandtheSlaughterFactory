using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	// Variables set up early on (unlikely to change state)
	private static GameController gameController;
	private string[] levels = new string[]{
    	"proto_lvl1"
	};
	private User currentPlayer;
	private string gameSlot;
	private List<User> leaderboard  = new List<User>();
	private GameObject usernameInput;
	private GameObject finishedLevel;

	// Variables that describe current state (likely to change)
	private bool isFinished = false;
	private int currentLevel = 0;
	private bool playerDied = false;

	// When the gamecontroller is initially created make sure that only the original one exists.
	public void Awake() {
		usernameInput = GameObject.Find("UsernameInput");
		usernameInput.SetActive(false);
		finishedLevel = GameObject.Find("FinishedLevel");
		finishedLevel.SetActive(false);
		GameObject.Find("PickPlayer").SetActive(false);
		if (gameController != null) {
			DestroyImmediate(gameObject);
			return;
		}
		gameController = this;
		DontDestroyOnLoad(gameObject);
	}

	// Cycle through each game slot button and check if a username exists for this slot: if it does, set the username as the text.
	// Should be called whenever the user asks to see available game slots.
	public void UpdateGameSlots() {
		for (int i = 1; i < 5; i++) {
			GameObject player = GameObject.Find("Player" + i);
			if (PlayerPrefs.GetString("Player" + i) != "") {
				player.GetComponentInChildren<Text>().text = PlayerPrefs.GetString("Player" + i);
			}
		}
	}

	// Set up the data for the leaderboard.
	// Should be called whenever a new score is saved to the current user.
	public void SetUpLeaderboard() {
		// Fetch data for leaderboard
		for (int i = 1; i < 11; i++) {
			leaderboard.Add(new User("", PlayerPrefs.GetString("LeaderboardUsername" + i), PlayerPrefs.GetInt("LeaderboardScore" + i)));
		}
	}

	// Update the strings contained in the leaderboard.
	public void UpdateLeaderboardView(GameObject panel) {
		Text[] textViews = panel.GetComponentsInChildren<Text>();
		panel.SetActive(true);
		string usernames = "";
		string scores = "";
		for (int i = 0; i < leaderboard.Count; i++) {
			usernames = usernames + (i+1) + ". " + leaderboard[i].GetUsername() + "\n";
			scores = scores + leaderboard[i].GetHighScore() + "\n";
		}
		textViews[4].GetComponentInChildren<Text>().text = usernames;
		textViews[5].GetComponentInChildren<Text>().text = scores;
	}

	// Show the supplied popup.
	public void ShowPopup(GameObject panel) {
		panel.SetActive(true);
		if (panel.name == "Leaderboard") {
			UpdateLeaderboardView(panel);
		}
		if (panel.name == "PickPlayer") {
			UpdateGameSlots();
		}
	}

	// Hide the supplied popup
	public void HidePopup(GameObject panel) {
		panel.SetActive(false);
		if (panel.name == "FinishedLevel") {
			NextLevel();
		}
	}

	// If the user selects a game slot, check if it's initialised. If not, set up the new user and then display the player's data page.
	public void SelectGameSlot(GameObject slot) {
		gameSlot = slot.name;
		// If the game slot the user selected has not yet been initialised, ask for a username:
		if (PlayerPrefs.GetString(slot.name) == "") {
			usernameInput.SetActive(true);
		} else {
			// Otherwise go straight to the player's data page.
			// Initialise the user currently playing.
			currentPlayer = new User(gameSlot, PlayerPrefs.GetString(gameSlot), PlayerPrefs.GetInt("Highscore" + gameSlot));
			UnityEngine.SceneManagement.SceneManager.LoadScene("PlayerData");
		}
	}

	// Create a new user (and set at the current player) based on the username (as long as the username is valid).
	public void SaveNewPlayer(Text usernameInput) {
		if (usernameInput.text == "") {
			GameObject.Find("WarningText").GetComponentInChildren<Text>().text = "Please input a valid username.";
		} else {
			// Create new user object.
			currentPlayer = new User(gameSlot, usernameInput.text, 0);
			UnityEngine.SceneManagement.SceneManager.LoadScene("PlayerData");
		}
	}

	// Start game, based on data to do with the current user.
	public void PlayGame(StartGame start) {
		isFinished = false;
		currentPlayer.ResetScore();
		// If the player died, reset their score to zero and let them start from the beginning of their last played level.
		if (playerDied) {
			UnityEngine.SceneManagement.SceneManager.LoadScene(levels[currentLevel]);
		} else {
			// If the user hasn't seen the cut scenes yet, play them.
			if (!currentPlayer.hasViewedCutScene1()) {
				start.PlayCutScene1();
			} else {
				NextLevel();
			}
		}
	}

	// Play the next level, or if all levels have been finished, display the finish screen.
	public void NextLevel() {
		// If the player finishes, make it so they reset from the beginning.
		if (currentLevel >= levels.Length) {
			currentLevel = 0;
			isFinished = true;
			UnityEngine.SceneManagement.SceneManager.LoadScene("PlayerData");
		} else {
			Debug.Log("me?");
			UnityEngine.SceneManagement.SceneManager.LoadScene(levels[currentLevel]);
			currentLevel = currentLevel + 1;
		}
	}

	public void GoToMainMenu() {
		UnityEngine.SceneManagement.SceneManager.LoadScene("Start");
	}


	// Getters and setters
	public bool hasFinished() {
		return isFinished;
	}

	public User getCurrentPlayer() {
		return currentPlayer;
	}

	public GameObject getFinishedLevel() {
		return finishedLevel;
	}
}