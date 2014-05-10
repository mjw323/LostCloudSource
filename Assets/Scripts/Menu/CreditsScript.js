// Credits.js
// Controls the multiplayer menu

// GameObject with the MainMenu.js script attached (Main Camera)
var mainMenuController : GameObject;

// Returns to main menu
function Back() {
	mainMenuController.GetComponent(MainMenu2).MainMenu();
}