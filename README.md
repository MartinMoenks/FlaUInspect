# FlaUInspect
There are various tools around which help inspecting application that should be ui tested or automated. Some of them are:
* VisualUIAVerify
* Inspect
* UISpy
* and probably others
Most of them are old and sometimes not very stable and (if open source), a code mess to maintain.

FlaUInspect is supposed to be a modern alternative, based on [FlaUI](https://github.com/Roemer/FlaUI).

On startup, you can choose if you want to use UIA2 or UIA3 (see [FAQ](https://github.com/Roemer/FlaUI/wiki/FAQ) why you can't use both at the same time).
###### Choose Version Dialog
![Choose Version](https://raw.githubusercontent.com/wiki/FlauTech/FlaUInspect/images/choose_version.png)

###### Main Screen
![Main Screen](https://raw.githubusercontent.com/wiki/FlauTech/FlaUInspect/images/main_screen.png)

In the ```Mode``` menu, you can choose a few different options:

| Mode | Description |
| ---- | ----------- |
| Hover Mode | Enable this mode to select the item the mouse is over immediately in FlaUInspect when control is pressed |
| Focus Tracking | Enable this mode that the focused element is always automatically selected in FlaUInspect |
| Show XPath | Enable this option to show a simple XPath to the current selected element in the StatusBar of FlaUInspect|
