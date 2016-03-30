# RUN FROM ROOT DIRECTORY
remove-item Release/ -recurse

mkdir "Release/Tools/Fallout 4 Character Tracker/"

copy-item "Source/TesSaveLocationTracker/bin/Release/TesSaveLocationTracker.exe" -Destination "Release/Tools/Fallout 4 Character Tracker/"

# copy-item "Source/TesSaveLocationTracker/bin/Release/TesSaveLocationTracker.exe.config" -Destination "Release/Tools/Fallout 4 Character Tracker/Resources/"

copy-item "Source/TesSaveLocationTracker/bin/Release/*.dll"  -Destination "Release/Tools/Fallout 4 Character Tracker/"

copy-item "Source/TesSaveLocationTracker/bin/Release/Resources" -recurse -Destination  "Release/Tools/Fallout 4 Character Tracker/Resources/"

copy-item "LICENSE"  -Destination "Release/Tools/Fallout 4 Character Tracker/" 

copy-item "README.md" -Destination  "Release/Tools/Fallout 4 Character Tracker/"
