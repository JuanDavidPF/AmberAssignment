This project implements a custom AssetBundle-based spawning workflow without using Unity Addressables, as requested in the assignment.

## Overview

The goal is to allow an artist to configure a `Spawner` by dragging an asset into the Inspector while making sure the scene does **not** serialize a direct reference to that asset. Instead, the asset gets loaded at runtime from an AssetBundle.

The implementation consists of three main components:

* **Spawner** – Stores only lightweight metadata about the linked asset and instantiates it at runtime.
* **SpawnerEditor** – Provides the editor workflow for linking, updating and unlinking assets.
* **AssetBundleService** – Loads and caches AssetBundles and assets asynchronously.

## Editor Workflow

1. Add a `Spawner` component to a GameObject.
2. Enter the desired AssetBundle name.
3. Drag any Project asset into the **Drop Asset To Link** field.
4. The editor:

    * Stores the asset GUID, asset name and AssetBundle name.
    * Assigns the asset to the selected AssetBundle.
    * Builds the AssetBundles.
    * Removes the need of having a direct asset reference serialized in the scene.

The Inspector also displays the currently linked asset and allows you to:

* Rebuild the AssetBundle after changing the linked asset.
* Unlink the asset.

This implementation was mainly done to centralize in the `Spawner` all the controls needed for the artist to add assets into bundles, without having to worry about the AssetBundle workflow or using Unity Addressables. The idea was to make the process as simple as possible for whoever is setting up content.

## Runtime Workflow

At runtime, the `Spawner`:

1. Reads the stored metadata.
2. Requests the AssetBundle from `AssetBundleService`.
3. Loads the asset asynchronously from the AssetBundle.
4. Instantiates the loaded asset at the `Spawner` position.

The `AssetBundleService` caches both loaded AssetBundles and asset loading operations, avoiding loading the same bundle multiple times if several Spawners request the same asset. This also helps reducing unnecessary disk operations.

## Why GUIDs?

The editor stores the asset GUID instead of the asset path.

Using the GUID makes the reference survive if the asset gets:

* Renamed.
* Moved to another folder.

Whenever an editor operation needs the asset again, the GUID is resolved back into its current path using Unity's `AssetDatabase`. This way the workflow keeps working even if the project structure changes over time.
