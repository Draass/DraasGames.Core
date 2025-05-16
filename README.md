# Project Overview
This is a set of scripts that I commonly use in my projects.

## Table of Contents
- [Dependencies](#dependencies)
- [Installation](#installation)
- [Modules that are implemented](#modules-that-are-implemented)
    - [ViewRouter](#viewrouter)
    - [StateMachineAsync](#statemachineasync)
    - [Effects](#effects)
- [TODO](#todo)
- [License](#license)

# Dependencies
There are 2 type of dependencies: absolute must and optional
## Absolute must dependencies
They are must to have. These are libraries I use in all my projects, and without them nothing will work (except for odin inspector)
- Extenject
- OdinInspector
- UniTask
## Optional dependencies
These dependencies are optional and depend on what modules you will activate
- DOTween - for Effects module. Abstract effects are still here, but if you'd want to use some ready ones, DOTween is neccessary
- Addressables - obviously for addressables integration 

# Installation
1. Install required dependencies
2. Import source code or (preferred) use package manager
- For installing package from git url paste https://github.com/Draass/DraasGames.Core.git?path=/Assets/Packages/DraasGames.Core
- Or download and import unity package from releases
3. Enjoy 

# Modules that are implemented
1. ViewRouter is usable except of Preloading functionality and addressables support, which will be added later
2. CustomButton and CustomToggle for enabling use of custom effects and enhanching toggle functionality
3. Effects
4. SlideCarousel (will be moved to UI Extensions later)
5. StateMachineAsync

## ViewRouter
ViewRouter is used for handling view switching.

### How to use
1. Create a ViewContainer script. Choose implementation of IViewContainer interface that suits your needs.
2. Create you view scripts. They must implement IView interface and inherit from MonoBehaviour (or just inherit from View class).
3. Use predefined ViewsInstaller to inject views infrastructure
4. Inject IVIewRouter where you need it
5. Done!

### API
``` csharp
IViewRouter.Show<T>(IView view);
```

## CustomButton and CustomToggle
These are both inherited from selectables, so they support all native elemnts features and give you even more

### How to use
- For CustomButton - just subscribe to OnClick (not unity event btw)
- For CustomToggle - same goes for OnValueChanged, not unity event again

### API


## StateMachineAsync
StateMachineAsync is a state machine implementation that uses UniTask to handle async operations.

### How to use

### API

## Effects
just a bunch of simple (mostly ui) effect.

### API
``` csharp
Effect.Play(bool reverse = false);
Effect.Pause();
Effect.Stop();
Effect.Reset();
```

# TODO
1. Add support for addressables in ViewRouter
2. Add support for preloading views
3. Import and refactor ListView functionality. Now it is a mess of legacy shit mixed with refactored stuff
4. Add more effects to Effects module
5. Write tests for other modules
6. Create a couple of example mini-apps with most of the modules used

# License
This project is licensed under the MIT License. See the LICENSE.md file for details.
