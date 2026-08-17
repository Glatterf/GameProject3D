# GameProject3D

A Unity 3D character controller project built as part of a series of Unity activities, covering core third-person movement mechanics and Mixamo-driven animation.

## Overview

This project implements a fully animated third-person character using Unity's `CharacterController` component, a custom orbit camera, and an Animator Controller driven by a Mixamo model.

## Features

- **Third-person movement** using `CharacterController` (WASD, camera-relative direction)
- **Orbit camera** with mouse look, pitch clamping, and smooth follow
- **Jumping** with gravity-based vertical velocity
- **Crouching** with dynamically resized collider (`height`/`center` synced to avoid ground clipping)
- **Sprinting** with adjustable speed multiplier
- **Mixamo-based character model** with a full animation state machine:
  - Idle
  - Walking
  - Sprinting
  - Jumping
  - Crouching
  - Walking while crouched
  - Landing (stationary) vs. Rolling (landing while moving), branched by speed at touchdown
- **Stairs and ramps** for testing `Step Offset` and `Slope Limit` behavior
- **Moving obstacle** using a ping-pong motion script

## Controls

| Action        | Key           |
|---------------|---------------|
| Move          | W / A / S / D |
| Look          | Mouse         |
| Jump          | Space         |
| Crouch        | Left Ctrl     |
| Sprint        | Left Shift    |

## Project Structure

```
Assets/
├── Scripts/
│   ├── movementPlayer.cs     # Character Controller movement, jump, crouch, sprint, animator hooks
│   ├── CameraMovement.cs     # Orbit/follow camera with mouse look
│   └── MovingObstacle.cs     # Ping-pong moving obstacle
├── Models/
│   └── Player/                # Mixamo character model + animation clips
├── Animations/
│   └── PlayerAnimator.controller
└── Scenes/
```

## Setup

1. Clone the repo:
   ```
   git clone https://github.com/Glatterf/GameProject3D.git
   ```
2. Open the project folder in **Unity Hub** (Add project from disk).
3. Let Unity reimport assets on first open (may take a few minutes).
4. Open the main scene under `Assets/Scenes/` and press Play.

## Requirements

- Unity Editor (version matching `ProjectSettings/ProjectVersion.txt`)
- Git (for cloning)

## Notes

- Animation clips were sourced from [Mixamo](https://www.mixamo.com/) and retargeted to Humanoid rig.
- Movement-type animations (Walking, Sprinting, Crouched Walking) use Mixamo's "In Place" export option since horizontal movement is handled entirely by `CharacterController`, not baked into the clips.
