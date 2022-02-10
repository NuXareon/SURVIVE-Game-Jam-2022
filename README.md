# SURVIVE-Game-Jam-2022
Buff Frog Games

## Game Mechanics MVP brief
Player:
    * Can move right and left
    * Has a simple jump
    * Has a color
    * Is affected by the rules on the sequence
    * Must reach the end of the level to win

Walls:
    * Walls have a color
        - White walls are neutral: the player collides with them
        - Colored walls interact with the player depending on their relationship:
            = Same color: the player goes through the wall
            = Different color: the player dies
    * The map is alway enclosed by walls
        - White walls if we want the player to collide
        - Unique color not in the sequence if we want the player to die

Rule Sequence:
    * Each level has one sequence
    * Each sequence has several Rules
    * The sequence starts paused, and begins execution once the player moves
    * Rules are executed in order, and wrap around after executing the last rule in the sequence
    * Each rules is execute on a beat (e.g. every second)
    * Each rule modifies one of the game mechanics
        - Several identical rules in a row can be used to give more time to the player
    * There are two types of rules:
        - Gravity changes (up, down, left, right)
        - Color changes (Red, Blue, Purple, ...)
