# Redline Racing

A small VR racing prototype made in Unity. It was the prototype for the car handling later used in [GIS VR Driving](https://github.com/mateir-7/GIS-VR-Driving).

You sit in a car with a grabbable steering wheel and gear shift and race AI opponents around a track built from Kenney Race Track pieces.

## Features

- Car physics built on Unity's `WheelCollider`, with front-, rear- or all-wheel drive selectable in the Inspector
- VR steering wheel and gear shift you grab with the controller grip
- Dashboard, lap counter and checkpoints
- AI race cars that follow a waypoint loop around the track

## Requirements

- Unity 6
- Meta Quest 3. The game runs as a standalone Android build; it can also be played from the Editor over Quest Link.
- [Stylized Vehicles Pack - FREE](https://assetstore.unity.com/packages/3d/vehicles/land/stylized-vehicles-pack-free-150318) from the Asset Store. It isn't included in this repo because of its license, so import it into the project before opening the scene.

## Running it

```bash
git lfs install
git clone https://github.com/mateir-7/RedlineRacing.git
```

Open the folder in Unity Hub, import the vehicle pack, open `BasicScene.unity`, connect the headset and press Play.

## Credits

Track pieces by [Kenney](https://kenney.nl) (CC0).
