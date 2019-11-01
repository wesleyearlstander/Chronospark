# Chronospark scripts

This repository contains scripts that I coded and used for the game Chronospark for reference of my ability.

Some notables are: 
- Human Controller
- Movie Controller
- Anything with Manager in it

All systems were designed and implemented modularly so cards could be used to add functionality such as a new achievement, new Trait, new Human bonus, new tutorial command etc. 

Physics had to be hard coded to work towards the center of the planet.

The game features a dynamic breeding system where everything from skin/eye/hair colour to stats of the individual people are bred through a recursive breeding system that uses the original root parents of every entity to create an amalgamated child from the genetics instead of the parents. This is how I assume actual genetics work and I feel may go unnoticed as it is a subtle system but effective nevertheless as breeding sometimes doesn't make sense because one of the grandparents or great grandparents down the line wasn't educated and skills improved like the rest. This causes a generational issue that get's passed down the family line and can be hard to rectify many generations later.

The game also features a modular tutorial system which was required for the development of the game. All actions were designed in a way that they can be readjusted and swapped out with each other as the tutorial was developed by the designer. The system is also modular as it could be evolved for actions that weren't originally planned and the order could be changed easily. The cards feature: speech between entities, movement, assignment, actions in game and testing player actions.
