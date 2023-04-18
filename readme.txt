You'll need to add a reference to the supplied Z80dotNet.dll
in this folder. Also install the NuGet NAudio package.

For legal reasons I have not included the ROM file.
You'll have to get that from elsewhere.

Note that in order for this to work, you need some kind of audio recording device that is active.
This is because the audio recording system in NAudio is used for the timing loop of the CPU
emulation. If there is no default recording device active, the emulator won't run.
