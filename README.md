# MMF CUNI - Unity Programing - Credit & Report

## Table of content 

- [MMF CUNI - Unity Programing - Credit & Report](#mmf-cuni---unity-programing---credit--report)
  - [Table of content](#table-of-content)
  - [Trailer](#trailer)
  - [Assets](#assets)
    - [Free assets](#free-assets)
    - [Paid assets](#paid-assets)
  - [Target platform](#target-platform)
    - [Platform](#platform)
    - [Controlls](#controlls)
  - [R5 - New input system](#r5---new-input-system)
  - [R6 + R7 - Editor extensions](#r6--r7---editor-extensions)
  - [A1 - 12](#a1---12)
    - [A5 - 3D game](#a5---3d-game)
    - [A9 - Non-trivial pathfinding in 3D](#a9---non-trivial-pathfinding-in-3d)
    - [A12 - Extensive use of animations](#a12---extensive-use-of-animations)
  - [Game pitch](#game-pitch)
  - [What did you enjoyed the most?](#what-did-you-enjoyed-the-most)
  - [What was your biggest struggle?](#what-was-your-biggest-struggle)
  - [Are you satisfied with the result?](#are-you-satisfied-with-the-result)
  - [What will you recommend to other people picking the same game?](#what-will-you-recommend-to-other-people-picking-the-same-game)

## Trailer

[![MianenCZ - Beginning of new era](https://img.youtube.com/vi/nQsv_j0VtX4/0.jpg)](https://www.youtube.com/watch?v=nQsv_j0VtX4 "MianenCZ - Beginning of new era")

## Assets

List of used assets.

### Free assets

- [Execution Order Attributes](https://assetstore.unity.com/packages/tools/utilities/execution-order-attributes-105354)
- [Starter Assets - Third Person Character Controller](https://assetstore.unity.com/packages/essentials/starter-assets-third-person-character-controller-196526)

### Paid assets

- [Crafting Mecanim Animation Pack](https://assetstore.unity.com/packages/3d/animations/crafting-mecanim-animation-pack-36545)
- [Basic Motions](https://assetstore.unity.com/packages/3d/animations/basic-motions-157744)
- [Melee Warrior Animations](https://assetstore.unity.com/packages/3d/animations/melee-warrior-animations-151650)
- [Low Poly Fantasy Medieval Village](https://assetstore.unity.com/packages/3d/environments/fantasy/low-poly-fantasy-medieval-village-163701)
- [POLYGON Modular Fantasy Hero Characters - Low Poly 3D Art by Synty](https://assetstore.unity.com/packages/3d/characters/humanoids/fantasy/polygon-modular-fantasy-hero-characters-low-poly-3d-art-by-synty-143468)
- [POLYGON Fantasy Rivals - Low Poly 3D Art by Synty](https://assetstore.unity.com/packages/3d/characters/humanoids/fantasy/polygon-fantasy-rivals-low-poly-3d-art-by-synty-118399)
- [POLYGON Icons Pack - Low Poly 3D Art by Synty](https://assetstore.unity.com/packages/3d/gui/polygon-icons-pack-low-poly-3d-art-by-synty-202117)
- [POLYGON Particle FX - Low Poly 3D Art by Synty](https://assetstore.unity.com/packages/vfx/particles/polygon-particle-fx-low-poly-3d-art-by-synty-168372)

## Target platform

### Platform

PC with support for Xbox and PS4 controller

### Controlls

| **Action** | **Mouse & Keyboard** | **Xbox controller** |
|---|---|---|
| Move forward | W | Left stick up |
| Move left | A | Left stick left |
| Move back | S | Left stick down |
| Move right | D | Left stick right |
| Look | Mouse  | right stick |
| Jump | Space | A |
| Sprint | Left shift | Left trigger |
| Open/Close inventory | I | Y |
| Attack | Left button | right trigger |
| Roll | Left controll | X |
| Crouch | C | B |

## R5 - New input system

![](.\Report\Input.PNG)

## R6 + R7 - Editor extensions

| **Item** | **Recipe** |
|---|---|
| Defining all system and gameplay information of an item. Editor shows its 3D model as shown in game and 3D GUI. | Defining all system and gameplay information of an recipe. Editor shows its components and result also with amounts. |
| ![](.\Report\Editor3.PNG) | ![](.\Report\Editor2.PNG) |
| **Loot table** | **Inventory** |
| Defining all system and gameplay information of an item. Editor shows loot with amount and probability according to loot table type. Fields are also conditioned to make readable view.  | Showing in game inventory with all items and amounts. |
| ![](.\Report\Editor1.PNG) | ![](.\Report\Editor4.PNG) |

## A1 - 12

### A5 - 3D game

I enden up using lowpoly assets with a 3D GUI.

![](.\Report\Inventory.PNG)

### A9 - Non-trivial pathfinding in 3D

I've created an extension for pathfinding defining acces points for destructible objects.

![](.\Report\Pathfinding.PNG)

### A12 - Extensive use of animations

I've created animation controller for both player movement animation and combat.

**Movement state automaton**
![](.\Report\Controll1.PNG)

**Combat state sub-automaton**
![](.\Report\Controll2.PNG)

## Game pitch

Prototype for lowpoly topdown RPG. Game features controlles of character, large set of armor equipment, combat animations, destructable objects, simple enemy AI with pathfinding.

In current state, the player is able to fight and kill enemies attacking village palisades. And ... walk on water.

## What did you enjoyed the most?

Basicaly everything. I stater with large of assets and connect one asset per programing session. Everytime I spent several hours and at the end, it was colorfull and movoing.

## What was your biggest struggle?

I spent a lot of time with animation controller to create a character controll & combat system, I had issues with animation controller automaton and its wierd behaviour (it jumped from state to state randomly _"meaning me beeing an idiot"_ for several hours)

## Are you satisfied with the result? 

As prototype yes.

I will continue with upgrading the combat system. I have already assets for that. I will replace AI completly with Behaviour trees and A* pathfinding. I will move this scene to custom created.

## What will you recommend to other people picking the same game?

I would sudgest doing something more simpler.

I coded this because it was a prototype for another much larger project. I ended up spending almost 100 hours of researching, designing and coding.