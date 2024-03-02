# v1.0.6
* Fix game attempting to load save twice
* Fix credits scroll speed at higher framerates
* Fix level select scroll speed at higher framerates
* Fix pump animation at higher framerates
* Fix pump particles at higher framerates (TODO: it's still too fast)
* Fix candy bubble speed at higher framerate
* Fix game updating touch location twice
* Fix render target resource leak
* Add AOT support and build release binaries for windows and linux with AOT
* Improve code by using resource variables instead of resource IDs
* Improve code by removing usage of `global::` and shortening function calls
* Improve code by refactoring the codebase
* Remove more unused code
* Update FNA and fnalibs to 24.03

# v1.0.5
* Fix FNA always building in the debug configuration
* Improve logic to draw the cursor
* Improve fixed framerate logic
* Remove window size limit
* Remove unused variables from `MouseState.cs` and  `ScreenSizeManager.cs`
* Remove unused branding code
* Remove ability to skip videos using mouse click
* Update FNA and fnalibs to 24.02

# v1.0.4
* Fix build warnings
* Update FNA and FNALibs

# v1.0.3
* Fix broken window resizing

# v1.0.2
* Improve window initialization code (fixes cut the rope not launching in fullscreen properly on wayland)
* Improve metadata by renaming `Cut The Rope` to `Cut The Rope FNA Edition`
* Remove unrequired 500ms delay after toggling fullscreen
* Remove pdb files from release binaries
* Remove `Cut The Rope.bmp` from windows build

# v1.0.1
* Remove unused files
* Remove unused functions
* Add x86 windows and arm64 linux builds

# v1.0.0
* Import decompiled code
* Migrate code to FNA
* Change audio and video format
* Change cursor format and drawing method
* Fix fullscreen
* Remove some unneeded code
