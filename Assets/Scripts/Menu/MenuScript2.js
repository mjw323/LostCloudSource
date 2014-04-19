// MainMenu.js
// Controls the various screens of the main menu

// List of GUI elements
var mainMenuElements : GameObject;
var newGameElements : GameObject;
var optionsElements : GameObject;
var creditsElements : GameObject;

// Enables the main menu
function MainMenu() {
	// Disable everything
	Disable();
	// Enable the elements we need
	mainMenuElements.SetActive(true);
}

function NewGame() {
	// Disable everything
	Disable();
	// Enable the elements we need
	newGameElements.SetActive(true);
}

function Options() {
	// Disable everything
	Disable();
	// Enable the elements we need
	optionsElements.SetActive(true);
}

function Credits() {
	// Disable everything
	Disable();
	// Enable the elements we need
	creditsElements.SetActive(true);
}

function Quit() {
	// Quit the game.  This will not work in WebPlayer builds
	Application.Quit();
	Application.LoadLevel(0);
}

// Hides every GUI element that is listed above
function Disable() {
	mainMenuElements.SetActive(false);
	newGameElements.SetActive(false);
	optionsElements.SetActive(false);
	creditsElements.SetActive(false);
}