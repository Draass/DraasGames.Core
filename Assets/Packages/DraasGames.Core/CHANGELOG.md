# Changelog

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