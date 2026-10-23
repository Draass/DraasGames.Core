# 1.0.0 (2026-10-23)


### Bug Fixes

* fix build error in ResourcesViewContainer ([de1d276](https://github.com/Draass/DraasGames.Core/commit/de1d276725e240a9ff2bfba86a445c4c0a1d07e8))
* fix custom toggle effect not playing ([8d411e2](https://github.com/Draass/DraasGames.Core/commit/8d411e260d05c2bd3f0b4c308ce5102a47189b4c))
* fix domain reload ([f764643](https://github.com/Draass/DraasGames.Core/commit/f764643d8ceebee113a16742fb48e36c2a587d7f))
* fix logger ([1ddb557](https://github.com/Draass/DraasGames.Core/commit/1ddb557dda4b61c14246179feeff1d23f88efa62))
* scoped by default ([ece435b](https://github.com/Draass/DraasGames.Core/commit/ece435b8eca06db54b26749cf0669d335faa6f42))
* Unity 6.6 issues ([e0bae12](https://github.com/Draass/DraasGames.Core/commit/e0bae125e014bbf742d3e063ec0bc7c7c445a16c))


* feat!: extract DLogger into standalone com.draasgames.dlogger package ([7cf8ea8](https://github.com/Draass/DraasGames.Core/commit/7cf8ea8c63ee1ab2d1d431eeb5e4d332126da1aa))


### Features

* add events to view router ([e0c3d98](https://github.com/Draass/DraasGames.Core/commit/e0c3d980bb1fcd5d1281760a979352596775b973))
* add none toggle mode to custom toggle ([7e81709](https://github.com/Draass/DraasGames.Core/commit/7e817095f41a41390e51c89a556959fcd13e80d0))
* add parametrick Show for ViewRouter flow ([8d13149](https://github.com/Draass/DraasGames.Core/commit/8d131497e3867db720c9677cb6399d6e0ffe4f1b))
* add tryc to AssetLoader ([8593327](https://github.com/Draass/DraasGames.Core/commit/859332790348c2e3b4736933b02ce22c1de837cd))
* add VContainer support ([923a47c](https://github.com/Draass/DraasGames.Core/commit/923a47c24004bd352e707fb85fce835611d52570))
* add view caching ([1d61b45](https://github.com/Draass/DraasGames.Core/commit/1d61b453efa58dfcbd1a76a05ecae94f7b794d8f))
* added auto assembly detection, added HideAsync for views ([69293c1](https://github.com/Draass/DraasGames.Core/commit/69293c116d613b556e308baa9503b61f6a0e4803))
* added custom console window ([03f8da6](https://github.com/Draass/DraasGames.Core/commit/03f8da6786a7b6a823acb0acf99d1f3143104ae4))
* alternate show mode wip ([3637505](https://github.com/Draass/DraasGames.Core/commit/3637505f4107db811e2be19125480246e880b8fc))
* DLogger tags and console window ([efbd054](https://github.com/Draass/DraasGames.Core/commit/efbd0549a474c0d56bbde3c11782bf8d29709395))
* DLogger update ([9b10abf](https://github.com/Draass/DraasGames.Core/commit/9b10abf0046ec632f7adbb2a35d031b458060eca))
* make core DI-agnostic, add Reflex support ([b1fc737](https://github.com/Draass/DraasGames.Core/commit/b1fc737b51f0e006f39b0a1cb44e8c6974af40fb))
* rebuild addressables view container in OnValidate ([a847759](https://github.com/Draass/DraasGames.Core/commit/a84775943dfe0a8d66be4774ab0a00c137c08081))
* test, fix ([644b79c](https://github.com/Draass/DraasGames.Core/commit/644b79c3e09eb2b4b8d0a3d9f88f7c137f1a1a78))
* up version, update package.json and changelog ([154ed75](https://github.com/Draass/DraasGames.Core/commit/154ed752f1b5a7f32971c71c0698cb9247fb2b08))
* update sample ([2357c5b](https://github.com/Draass/DraasGames.Core/commit/2357c5b5ef2689cfd1e48ea868eac07e2eaa2c2a))
* update sync api to use async ([c2225bf](https://github.com/Draass/DraasGames.Core/commit/c2225bf12169fe7d73d7a6d13e259f5cf9604e5d))


### BREAKING CHANGES

* DLogger types moved to the DraasGames.Logging namespace in
the com.draasgames.dlogger package; install it to keep using DLogger.

# Changelog

## [0.4.1] - 2026-09-28

### Fixed
- Fixed UAC1012 compilation error in Unity 6.6 and newer by explicitly using Odin serialization for the `ResourcesViewContainer` view-path dictionary. Earlier Unity versions retain `[SerializeField]`; the existing field name and data format are preserved.

## [0.4.0] - 2026-06-11

### Changed
- BREAKING: DLogger extracted to a standalone package `com.draasgames.dlogger` (https://github.com/Draass/DLogger). Install it to restore DLogger-backed logging in Core (detected automatically via the DRAASGAMES_DLOGGER define); without it Core falls back to UnityEngine.Debug. DLogger namespaces changed from `DraasGames.Core.Runtime.Infrastructure.Logger` to `DraasGames.Logging`.
- Editor and test assembly definitions renamed (NewAssembly -> DraasGames.Core.Editor, Tests1 -> DraasGames.Core.Tests.EditMode)

## [0.3.8] - 2026-04-27

### Added
- Parameterized ViewRouter flow with IView<TParam> and parameterized ShowAsync, ShowModalAsync, and ShowPersistentAsync
- ViewRouter OnViewShown and OnViewHidden events
- Optional ViewRouter view caching through ICacheableView and ResetStateAsync
- ViewRouter cache management API: IsCached, PreloadAsync, ReleaseCached, ReleaseAllCached
- IDestroyableView lifecycle contract with default ViewBase implementation
- Pooling infrastructure with IPoolService, IPoolable, PoolOptions, and Addressables-based pool registration
- Reflex integration installers and instantiator adapter
- VContainer integration installers and instantiator adapter
- Zenject integration asmdef, instantiator adapter, and suffixed installer aliases
- DLogger settings asset, minimum log level filtering, DLogLevel.None, and editor settings provider
- CustomToggle None toggle mode

### Changed
- Core package is now DI-agnostic, with container-specific code moved under Runtime/Integrations
- ViewRouter public generic constraints now target view interfaces instead of MonoBehaviour
- ViewRouter transition handling now serializes concurrent show requests
- Hidden ViewBase instances now disable GraphicRaycaster
- VContainer Addressables installers now use scoped lifetime by default

### Fixed
- Repeated rapid ShowAsync calls for the same view no longer create duplicate active views
- Switching between regular views no longer leaves the previous regular view active

## [0.3.7] - 2025-09-12

### Fixed
- ObjectDispodesException in ProjectLifetimeHolder when reload domain turned off

## [0.3.6] - 2025-08-10

### Fixed
- DLogger NullReferenceExpection on attemp to use any method
- Auto register basic loggers in DLoggers

## [0.3.5] - 2025-29-07

### Added
- HideAsync to ViewRouter
- Auto assembly detection

## [0.3.4] - 2025-22-07

### Fixed
- CustomToggleGroup behaviour

## [0.3.3] - 2025-27-06

### Added
- - Add some dependencies to package.json

### Changed
- Sample is now downloadable

### Fixed
- LibraryPackageCache Addressables error

## [0.3.2] - 2025-04-06

### Fixed
- Build error

## [0.3.1] - 2025-03-06

### Added
- Addressables infrastructure installer

### Changed 
- Modified installing view logic via installers

## [0.3.0] - 2025-01-06

### Breaking changes
- Divided ViewInstaller into 2 distinct installers for resources and addressables flow. 
Be aware that they are now designed to be kept on project installer. All dependencies will go to the subcontainers. 

### Added
- Added Addressables support to ViewRouter and a system for handling loading assets through Addressables 
- Added Presenter navigation flow to remove logic from Views if desired
- Introduced async api to view router. Sync api is now deprecated and will be removed in future
- Added custom formatted logger
- Added StartGame button to launch game from specified scene and return to initial one on play finished

### Fixed
- Editor settings view is now available even if there are some compilation errors

## [0.2.3] - 2025-16-05

### Fixed
- Fixed using subsequent Show and ShowModal in ViewRouter.cs causing exception
- Not interactable toggles are now actually not interactable

## [0.2.1] - 2025-10-01

### Fixed
- Not interactable toggles are now actually not interactable

## [0.2.0] - 2024-21-12

### Added
- Custom Button and Toggle functionality with custom effects and option to use native ones

### Changed
- Awake in View is now virtual method. So no nullref exception in case you created another Awake in you inherited view class

## [0.1.2] - 2024-16-12

### Fixed
- Fix build error in ResourcesViewContainer

## [0.1.1] - 2024-13-12

### Added
- An editor window to enable or disable different modules. These are:
  - Addressables (though they are not implemented)
  - Effects

### Changed
- Removed a lot of unfinished or obsolete modules, like quiz, ListView (currently wip) etc.

## [0.1.0] - 2024-11-12

### Added
- Initial release with core features:
    - ViewRouter
    - StateMachineAsync
    - Effects
