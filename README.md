# Antpocalypse - A Roguelike Survival
Showcase of a simple project I worked on in my spare time.

Greetings,

Firstly, I would like to say my thanks to all who showed interest in this project. Welcome.

I will try to keep this as clear and concise as I can, as not to waste your precious time. Here I will present a simple outline of the projects main features and its general structure. At the bottom of the page you will find a video showcasing short and simple gameplay. The game is unpolished to say the least but most of the planned features work as intended.
If you do choose to download the project for yourself, the credentials for the demo account are demo/demo (user/pass). Also, my apologies for the state of the art in this game. All art was done by me in Photoshop and well, you can see why I'm a programmer...

Vision

  The idea is a very simple game that involves dodging enemies, picking up orbs (for health and experience) and leveling up, but set in the context of a truly simulated ant enviorment. As he levels up, the player will choose from a random selection of improvements and skills which will help him to deal with overwhelmingly hostile ants. The skills are all automatic and on a timer so that the simplicity of the game is retained.
  The goal is simple: Destroy the ant hive before ants steal a set amount of food.
 
Structure

  The project is divided into two relatively seperate parts, that is, the simulation of the ants, and the actual gameplay. In addition to that there is, of course, the main menu scene and corresponding sql database scripts. These elements are not as important, but included nontheless.
  
   The Simulation:
   
        The reference paper for implementation of ant behaviour can be found here: https://uwe-repository.worktribe.com/output/980579 
        The basic data structure and sense/move logic of the agents is relatively unchanged from paper to implementation.
        References to additional simulation technique, that is, approximation of the ant pheromone spread can be found in the scripts.
        The scripts that handle ant/hive simulation are as follows: HiveModel, HiveController and Agent. HiveView is used for visualizations.
        
   The Gameplay:
   
        Data generated from the simulation is then used for the generation of the level elements such as walls, food points and the hive. 
        From Agent data AntAgent gameobjects are initialized and updated with a simple optimization. All this is done in the WorldController script.
        PlayerController class handles player movement/controlls and orb pickups.
        Each Class (Elementalist and Commando are the only two implemented at the moment) is given one active skill ("Ultimate Ability") and a set of passive skills (automatic/on a timer).
        Every third level the player will choose from a set of three talents that either enhance the already learned skills or give the player a new one. 
        At all other levels the player will choose from a set of General Talents which simply enhance base stats of the class.
        Most of the other scripts not mentioned here are for the implementation of skills and their various helper scripts (Thunderbolt, Bomb, and so on..)
    
  


