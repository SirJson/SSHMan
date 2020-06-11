# SSHMan

A Windows Terminal launcher for SSH connections defined in your local SSH config. `(~/.ssh/config)`

![screenshot-a](other/screenshot-a.PNG)

It basically parses your user SSH config and creates a list of all entries. On connect it will open the new Windows Terminal and connect to the selected Server.

This is an easy alternative to creating a profile inside your Terminal config for every remote connection you already have in your SSH config. 

Don't have a SSH config yet? Create one directly from the settings menu! If you change something it will automatically be applied in SSHMan as well.

Not fancy another program just to start your SSH sessions? SSHMan can export all Hosts from your SSH config into new Windows Terminal Profiles or update your Profile List if changes are detected.

To use this feature open the settings menu

![configbnt](other/configbnt.PNG)

and press the "Create Terminal Profiles" Button

![export](other/export.PNG)

## Installation

This project depends on

-   The new Windows Terminal
-   PowerShell 6+

If you have PowerShell 6 or 7 installed and the Windows Terminal it will automatically make sure that you also have the included `ReadNamedPipe` Module and the launcher script installed. 

## How this works

After parsing you press „Connect“ it will run st.exe with the new-tab action and the Remote Profile. From there it will run [sshchild.ps1](https://github.com/SirJson/SSHMan/blob/master/sshchild.ps1)

At the same time the launcher will provide a named pipe with a GUID that it passed to sshchild.ps1 and waits for a connection. If both are connected the script will receive the parameter it need to launch your SSH session.