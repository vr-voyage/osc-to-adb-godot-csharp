# OSC to ADB (Clicker only at the moment)

This software was made to connect VRChat avatars parameters change
sent through OSC, to automated clicks on an Android Phone screen
using ADB.

So, this is basically an OSC to ADB clicker at the moment.

![OscAdbGodotSharp](https://github.com/user-attachments/assets/2c1708d2-c035-489d-87d7-d461bf995425)

# Usage

## First launch

On first launch, you'll be requested to provide ADB.exe
You can try to click on "Search for ADB.exe". This will try to search
"C:\Program Files\Unity" for an ADB.exe executable.

Once found, double click on the entry and click the upper right icon.

### Why C:\Program Files\Unity ?

Because this software is made for VRChat users, who generally have
the Unity SDK installed with Android support enabled.

## Main UI

* **Take a screenshot**  
  The upper left icon allows you to take a screenshot of your phone.  
  This allows you to configure clickers locations.

* **List of scanned OSC values**  
  The right panel provides a list of OSC values currently received
  (from VRChat generally).  
  You can double-click a value to add it as a clicker condition.

* **The OSC clicker condition**
  The bottom of the middle pane allows you to Add or Edit the
  conditions of a clicker.  
  Don't forget to click the 'Add' or 'Edit' button at the bottom
  once finished.  
  Setup the :
  * OSC path name (e.g. : /avatar/parameters/YourWonderfulToggle)
  * Condition (Equal, Different, Greather than, Lower than)
  * Threshold (Always a floating number. False is 0. True is 1.)

* **The list of OSC clickers**  
  Clicking an added OSC clicker will allow you to :
  * Edit the conditions at the bottom
  * Edit the position at the left, but only if you already took a screenshot

* **Setting the click position**
  Once a screenshot taken, you can click on an added OSC clicker to define
  its position on the screenshot. Just left click and hold to move the clicker
  position, which, by default, is at the upper left of the screenshot.

* **Enabling an OSC clicker**
  In the list of added OSC clickers, at the center of the screen, you can
  check a clicker checkbox to mark it as Enabled.
