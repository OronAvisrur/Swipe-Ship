# Swipe Ship

Swipe Ship is a casual game developed using Unity, where the player navigates unsafe waters filled with mines and his mission is to find the correct path to the treasure. This document provides an overview of the game's code, focusing on key aspects such as emphasizing its structure, functionality, and design principles.

## Features

- **Data Saving**: Swipe Ship includes a data saving system to ensure that player progress and game states are preserved across sessions. The key components of this system are: Player Preferences (PlayerPrefs): Used to store simple data such as high scores, settings, and other player preferences, Serialization: Complex data structures, such as level progress and unlocked ships, are serialized into binary format and saved to the local storage and Save and Load Mechanism: The game includes functions to save the current game state and load it when the game is started, ensuring a seamless player experience.
- **Mine Detection**: Collision detection is handled using Unity's physics engine. Scripts are designed to detect proximity to mines and trigger appropriate responses, such as game over states or animation sequences.
- **Animations**: Smooth and engaging animations are integral to the game's visual appeal. The animation logic is handled using Unity's Animator component, with scripts controlling the transitions and states based on gameplay events.
- **User Interface**: The game's UI is intuitive and responsive, created using Unity's UI system. Scripts managing the UI are designed to handle player input, update scores, and display game states dynamically. 

## Code Structure and Design

- **Clean and Up-to-Date Code**: The codebase adheres to modern programming standards and best practices. Each script is meticulously organized and well-commented, ensuring readability and maintainability.
- **Object-Oriented Programming (OOP)**: The game leverages OOP principles, encapsulating functionality within classes to promote reusability and scalability. Core game components like the ship, mines, and paths are implemented as distinct classes with clear responsibilities.
- **Modular Design**: The code is modular, breaking down complex functionalities into smaller, manageable pieces. This approach not only simplifies debugging and testing but also allows for easy feature extensions in the future.

## Key Directories

- **Scripts**: Contains all the C# scripts, including algorithms, game logic, data management, and UI handling.
- **Prefabs**: Contains pre-configured game objects used in the game, such as the ship, mines, and grid cells.
- **Scenes**: Includes different game scenes like the main menu, levels, and game over screens.
- **Sprites**: Contains assets such as images, tiles, animation, and other resources used in the game.
