# **CYA - Choose Your Adventure**

A text and audio based interface to laod and play 'interactive fiction' games in the style of 'choose your own adventure' books. The base engine is designed to be extendable through optional modules to extend functionality. This includes some presets, as well as allowing for users to implement their own.

/Occult - Contains the files for a sample game used for testing & as an implementation guide.

## **Implementation**

The tokenizer and parser process game files to construct an object then interpreted by the game engine.

**Key Components**

**Scene** - a 'scene' is a location within the story, equivalent to a page or index within a game book. Any given scene should contain a unique ID, text, and choices for the user to pick betwen. A scene can also take 'actions'.

**Choice** - on any given scene, a user will be presented with multiple choices for the path/ action for them to take. Any 'choice' should contain text and a 'target' location (the scene to which the user should be sent after the choice is successfully processed). A scene can also take 'actions'. Choices should only exist within scenes.

**Action** - an action represents a modification to state, or IO effect - for example a sound effect, or modification to the player's inventory. When a player selects a choice, the actions attached to it should first be evaluated (to assure all can be completed given the current state), then processed such that the appropriate changes are made. Actions should only exist within scenes or choices.

## **Sample** (from Occult)

1

"You awake in your bedroom."

"This is a second string as a feature test."

\> Go Downstairs -> 2 \[i.need slippers, socks]\[s.play ./sounds/world/door.wav]

\> Scream -> 3

\> Try to eat your laptop -> 4

\> Roll a dice HIGH -> 1 \[if d.2d6 >6 then \[i.add sword] otherwise -> 17]

\> Put on slippers -> 5 \[i.add slippers]

### **Key**

Plaintext identifies scene id (here, 1). Everything between here and the next scene id will be added to this scene.

"" -string text is registered as scene text, and is the first thing displayed to the user. 

\> - lines beginning with a > are registered as choices. The text following is the choice text presented to the user, ending at the arrow, '->'. The arrow is used to identify the target, which proceeds it.

[] - actions are shown in braces.
