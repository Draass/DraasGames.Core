# Changelog

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