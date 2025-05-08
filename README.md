## VR Player Notes : 

If you encounter many errors after cloning this repository relating to VR, **PLEASE READ THIS SECTION!**

1. Import [VRIF package](https://assetstore.unity.com/packages/templates/systems/vr-interaction-framework-161066)  from Unity Asset Store except **BNG Framework/Scripts** folder or you can import all of them but recheck **ToggleGravity(bool gravityOn)** function on **BNG Framework/Scripts/Core** folder, the code is supposed to be like this:
```cs
public void ToggleGravity(bool gravityOn) {
    Debug.Log("Toggle Gravity " + gravityOn);
    GravityEnabled = gravityOn;

    if (gravityOn) {
        Gravity = _initialGravityModifier;
    }
    else {
        // store gravity effect before update
        _previousGravityModifier = Gravity;
        Gravity = Vector3.zero;
    }
}
```
2. Import [SteamVR Plugin package](https://assetstore.unity.com/packages/tools/integration/steamvr-plugin-32647) from Package Manager.