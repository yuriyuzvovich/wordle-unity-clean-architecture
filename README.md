<img width="405" height="825" alt="image" src="https://github.com/user-attachments/assets/eb4c858e-321c-4405-959e-004b4656b463" /> <img width="411" height="825" alt="image" src="https://github.com/user-attachments/assets/976b52d9-a219-43d1-a03a-1ea07dff8b69" />


# Wordle Unity Architecture & Feature Guide

## Overview
This project implements Wordle in Unity using a clean, four-layer architecture that keeps core game rules independent from platform-specific concerns. The solution is split into Core (domain), Application (use cases), Infrastructure (framework and service adapters), and Presentation (Unity UI) assemblies under `Assets/_Project`. These layers are wired together at runtime by a Bootstrapper and a lightweight dependency injection container so that dependencies always point inward and gameplay logic remains testable and modular.

## Bootstrapping & Dependency Management
`Bootstrapper` is the composition root executed from the Bootstrap scene. It instantiates the dependency container, registers cross-cutting services (logging, lifecycle, event bus, object pooling), sets up configuration, command processing, and infrastructure modules, and then loads the main scene once asynchronous persistence initialization finishes. The custom `DependencyContainer` supports singleton, transient, and factory registrations plus constructor/field injection, enabling plain C# classes to receive interfaces without depending on Unity components.

A `UnityLifecycleProvider` MonoBehaviour abstracts Unity frame events (Update, FixedUpdate, application focus/quit) into the `IEngineLifecycle` interface, letting non-MonoBehaviour services subscribe to frame ticks or lifecycle callbacks without relying on Unity APIs.

## Cross-Cutting Patterns
- **Command Queue & Processor** – Gameplay interactions are expressed as commands enqueued by priority. `CommandQueue` keeps per-priority FIFO queues, while `CommandProcessor` runs a configurable number of commands per frame, guarding against race conditions and providing deterministic sequencing.
- **Event Bus** – A thread-safe `EventBus` delivers both synchronous and UniTask-based asynchronous events between layers, allowing the application and presentation tiers to react to domain changes without tight coupling.
- **Object Pooling** – `ObjectPoolService` centralizes GameObject pooling so the board can reuse tile instances efficiently, reducing allocations during gameplay.
- **Addressables Asset Service** – `AddressablesAssetService` wraps Unity Addressables for async loading, instantiation, and release, providing infrastructure for dynamic content like word lists and modals.

## Core Domain Layer
Domain classes are pure C# objects that model Wordle rules:
- `GameState` tracks the target word, attempts, status, and ensures valid transitions when guesses are added.
- `GuessEvaluator` compares guesses to the target word, handling duplicate letters accurately and producing per-letter evaluations for the UI.
These entities and services depend only on domain interfaces, making them straightforward to unit test and reuse outside of Unity.

## Application Layer
Use cases orchestrate domain logic and persistence. `StartGameUseCase` validates inputs, creates a new `GameState`, saves it, and publishes a `GameStartedEvent` so presentation components can react. The application layer exposes interfaces for command queues, configuration, repositories, analytics, and more, allowing infrastructure to inject platform-specific implementations.

## Infrastructure Layer
Infrastructure bridges Unity and external systems to the pure application contracts:
- **Configuration** – `ProjectConfigService` exposes tunable settings (word length, attempts, addressable keys) and logs initialization for traceability.
- **Persistence** – `GameStateRepository` and `StatisticsRepository` serialize data into PlayerPrefs through an `ILocalStorageService`, logging failures and providing clear operations for save, load, and clear flows.
- **Word Lists** – `WordRepository` loads target and valid guess lists via Addressables, supports language switching, and emits `WordsReloadedEvent` when new lists arrive.
- **Localization & Themeing** – `LocalizationService` loads JSON dictionaries, persists the selected language, and publishes `LanguageChangedEvent` notifications. `ThemeService` retrieves stored theme preferences, broadcasts `ThemeChangedEvent`, and exposes current colors for the UI.
- **Analytics** – `PlaceholderAnalyticsService` logs tracking calls, acting as a drop-in analytics provider for future replacement.

## Presentation Layer
The presentation tier applies MVP and controller patterns to keep MonoBehaviours slim:
- `BoardView` is an `InjectableMonoBehaviour` that builds the tile grid from an Addressables prefab using the object pool and delegates behavior to `BoardPresenter` for state loading, animations, and event handling.
- `KeyboardPresenter` generates localized layouts, restores key states from saved games, and routes button presses to the shared input services.
- `InputController` composes both `KeyboardInputService` and `TouchInputService`, tracks the active guess locally, and converts input events into command queue operations and use case executions.
- `GameController` demonstrates a non-MonoBehaviour orchestrator that queues `StartGameCommand` instances, reacts to domain events, and restarts when word lists change.
- `ModalManager` asynchronously instantiates win/lose modals through the Addressables service and releases them on application quit, ensuring UI resources are managed centrally.

## Gameplay Features
- **Persistent Game Sessions** – Game state and statistics survive restarts via the repositories, enabling resume and long-term progress tracking.
- **Rich Input Support** – Desktop keyboard input is polled every frame via `IEngineLifecycle`, while virtual keys call into `TouchInputService`, giving consistent command generation across devices.
- **Animated Board Feedback** – The board presenter sequences tile flips, shakes, and victory animations after each submission, leveraging UniTask delays and event callbacks to coordinate UI timing.
- **Dynamic Localization & Keyboard Layouts** – Changing language reloads words, rebuilds the keyboard, updates labels, and triggers a fresh random word selection through the word repository and keyboard presenter workflows.
- **Theme Switching** – `ThemeService` stores the selected theme and notifies listeners with the latest color scheme so UI components can refresh styling immediately.
- **Analytics Hooks** – Commands and controllers can log gameplay events through the analytics interface, currently backed by a placeholder service that writes diagnostic output.
