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
- Zenject
- OdinInspector
- UniTask
- DOTween
- NSubstitute - for unit testing 

# Installation
1. Install required dependencies
2. Import source code or (preferred) use package manager
- For installing package from git url paste https://github.com/Draass/DraasGames.Core.git?path=/Assets/Packages/DraasGames.Core
3. Enjoy modules that are implemented and do not use modules that are not :)

# Modules that are implemented
1. ViewRouter is usable except of Preloading functionality and addressables support, which will be added later
2. Effects
3. SlideCarousel (will be moved to UI Extensions later)
4. StateMachineAsync

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