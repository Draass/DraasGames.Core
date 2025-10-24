# Project Overview
This is a set of scripts that I commonly use in my projects.

## Table of Contents
- [Dependencies](#dependencies)
- [Installation](#installation)
- [Modules that are implemented](#modules-that-are-implemented)
    - [ViewRouter](#viewrouter)
    - [StateMachineAsync](#statemachineasync)
    - [PresenterNavigationSerivce](#presenternavigationservice)
    - [AssetLoader and Lifetime](#iassetloader-and-ilifetime)
    - [Effects](#effects)
- [TODO](#todo)
- [License](#license)

# Dependencies
- Extenject
- OdinInspector
- UniTask
- DOTween

# Installation
1. Install required dependencies
2. Import source code or (preferred) use package manager
- For installing package from git url paste https://github.com/Draass/DraasGames.Core.git?path=/Assets/Packages/DraasGames.Core
3. Enjoy modules that are implemented and do not use modules that are not :)

## ViewRouter
ViewRouter is used for handling view switching. It is designed with 3 types of views in mind:
1) Regular views. Previous regular view will be hidden by default. You can combine them with modal and persistent views
2) Modal views. They are designed for popups
3) Persistent views. You can use them to show overlays. They won't be hidden unless you hide them explicitly

### How to use
1. Create a ViewContainer from context menu (DraasGames/UI/...) and add there views you would like to have.
2. Create you View scripts. You can use base View class or create your own. It should implement IView and inherit MonoBehaviour to be used.
3. Add your view script to view prefab.
4. Use ResourcesViewInstaller or AddressablesViewInstaller. It will inject everything necessary to subcontainers.
5. Inject IVIewRouter and you are done.

### API
``` csharp
IViewRouter.ShowAsync<T>(IView view);
```

## PresenterNavigationService
You can use IPresenterNavigationService to remove logic from your views almost completely and extract it to presenters,
thus creating some View-Presenter like flow.
If your 

## IAssetLoader and ILifetime
IAssetLoader is designed to handle operations with adressables assets in a simple way and unload them in a less forced manner.
ILifetime serves as a marker that asset should be unloaded, so you do not have to keep asset references, disposing Lifetime is enough.

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
1. Add support for preloading views
2. Import and refactor ListView functionality. Now it is a mess of legacy shit mixed with refactored stuff
3. Add more effects to Effects module
4. Write tests for other modules
5. Create a couple of example mini-apps with most of the modules used

# License
This project is licensed under the MIT License. See the LICENSE.md file for details.