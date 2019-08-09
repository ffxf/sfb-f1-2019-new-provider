# sfb-f1-2019-new-provider
SimfeedBack provider for F1 2019 (and F1 2018) new UDP format

See (releases)[https://github.com/ffxf/sfb-f1-2019-new-provider/releases] for installable files.

The current version assumes the standard UDP port 20777 of F1-2018 and F1-2019.

The effects `Rumble Left` and `Rumble Right` are disabled intentionally in the template profile because they do not work right.
`Traction Loss` is also a work in progress.
There is no RPM data in the standard car motion packet delivered by the games. RPMs are in another packet and it would complicate the provider code consume this packet as well. So RPMs are currently not supported.

## For Developers

Change the target assembly name to `F12019NewTelemetryProvider.dll` if building for F1-2019 or to `F12018NewTelemetryProvider.dll` if building for F1-2018.
The provider code is identical but there is a small difference in the packets delivered by the game. The assembly name is how the provider differentiates the package to be expected.
