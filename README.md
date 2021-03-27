# DesertTanks
 DesertTanks is a Real-Time Strategy multiplayer project based on
 [multiplayer course by Gamedev.TV](https://www.gamedev.tv/p/unity-multiplayer).

## What more is here?
So you might be wondering:
>What more can be found here that is not in the [GameDev.Tv repo](https://gitlab.com/GameDevTV/UnityMultiplayer/RealTimeStrategy)? 

Good question. I have followed the course closely all the way to the end, so you 
should be able to find everything from the course somehow represented in my code.
However, there were things I did not like or spots that I felt like doing a little
differently. On top of that, I added a few features that were not present in the 
course at all. These changes include but are not limited to:

  * Fully implementing new InputSystem including LeftClick and RightClick mapped actions.
  * [Utility](https://github.com/Stopa42/DesertTanks/tree/main/DesertTanks_Unity/Assets/Scripts/Utility)
    classes which help either write cleaner code or ease work in the inspector.
  * Thinner classes. I have separated many components into multiple ones to better respect the single responsibility principle. 
    The [original RTSPlayer.cs](https://gitlab.com/GameDevTV/UnityMultiplayer/RealTimeStrategy/-/blob/master/Assets/Scripts/Networking/RTSPlayer.cs) 
    class has 275 lines and handles everything from registering spawned units and buildings, over choosing player color to validating and executing new building placement. 
    In my solution this class is split into 
    [RTSPlayer.cs](https://github.com/Stopa42/DesertTanks/blob/main/DesertTanks_Unity/Assets/Scripts/Player/RTSPlayer.cs),
    [PartyMember.cs](https://github.com/Stopa42/DesertTanks/blob/main/DesertTanks_Unity/Assets/Scripts/Player/PartyMember.cs), 
    [BuilderBehaviour.cs](https://github.com/Stopa42/DesertTanks/blob/main/DesertTanks_Unity/Assets/Scripts/Player/BuilderBehaviour.cs), 
    [ResourceHandler.cs](https://github.com/Stopa42/DesertTanks/blob/main/DesertTanks_Unity/Assets/Scripts/Player/ResourceHandler.cs), 
    each with less than 100 lines of code.
  * The separation and abstraction mentioned above lead to easy implementation of additional features such as:
    * Destructible terrain
    * Player can build more bases
    * Buildings can be placed with chosen orientation
    * More customizable team colors for objects
    
## Is this a playable game?
Sadly, no. I mean, yes, kind of, but not really...

What I am trying to say is that you can definitely download it and play it, but it will probably not be that much fun. 
Throughout the course I have focused mostly on the core code and thus there is very little content and it is not balanced in any way.
Hopefully, I will be able to build up on it and make it into a real proper game some time in the future!

![screenshot](https://github.com/Stopa42/DesertTanks/blob/main/screenshot.png "Screenshot of DesertTanks")
