# **CYA - Choose Your Adventure**

A text and audio based interface to laod and play 'interactive fiction' games in the style of 'choose your own adventure' books. The base engine is designed to be extendable through optional modules to extend functionality. This includes some presets, as well as allowing for users to implement their own.

/Occult - Contains the files for a sample game used for testing & as an implementation guide.

## **Implementation**

Game writing and loading depends on a Domain Specific Language (DSL) created for this project.

A tokenizer, parser, and interpreter are implemented to prepare the gamefile to run.

**Key Components**

**Scene** - a 'scene' is a location within the story, equivalent to a page or index within a game book. Any given scene should contain a unique ID, text, and 'Interactables' (choices) for the user to pick betwen.

**Interactable** - on any given scene, a user will be presented with multiple ways to interact with the game. These are broadly categorised into "annotations" which allow the user to access additional content while remaining on the same scene, and "choices" which will move the user to the next scene.

**Event** - an event represents a function call that will either modify the gamestate or produce an IO effect. For example, a "say" function will display additional text to the player.

## **Sample**

scene "bedroom"

name = (ask "What is your name?: ")

(say "Hello " name)

"You awake in your bedroom."

"This is a second string and will be shown as a separate paragraph."

\["Scream" (say "You scream. AAAAAAA")]

\["Go Downstairs" -> "downstairs"]

### **Key**

Strings within a scene are treated as sugar for say calls, with the loose string as the argument. Separate strings are treated as sequential calls here and become distinct paragraphs.

name = (ask "What is your name?: ") : This is a variable assignment that uses an 'ask' function to present the user with a prompt text, and store their response. This allows for personalisation techniques to be implemented.

\["Scream" (say "You scream. AAAAAAA")] : This is an annotation presented to the user. The initial string, "Scream" is presented as the option description. Upon selection, the body - in this case a single function call - is interpreted.

\["Go Downstairs" -> "downstairs"] : This is a choice. Just like the prior annotation, the initial string is presented to the user as the option description. Upon selection, the body is interpreted. In this case, that is a GoTo, moving the player to their next scene, "downstairs" (provided as the argument following the GoTo '->').
