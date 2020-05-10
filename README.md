# SSHMan

A Windows Terminal launcher for SSH connections defined in your ssh config.

![screenshot](screenshot.PNG)

It basically parses your user SSH config and creates a list of all entries. On connect it will open the new Windows Terminal and connect to the selected Server.

This is an easy alternative to creating a profile inside your Terminal config for every remote connection you already have in your SSH config.

## Installation

This project depends on

-   The new Windows Terminal
-   PowerShell 6+

If you have PowerShell 6 or 7 installed and the Windows Terminal it will automatically make sure that you also have the included ReadNamedPipe Module and the launcher script installed. 

All you need to do is define a Profile inside your Terminal config with the name `Remote`. Here you can choose what ever settings you like for your remote connections like font or color scheme.

This is the profile I use, you can use this as an example or just use it as is

```json
            {
                "guid": "{8954ce30-29ac-4335-997a-4a96c00ded5a}",
                "hidden": false,
                "name": "Remote",
                "fontFace": "Hack NF",
                "colorScheme":"Lab Fox",
                "useAcrylic": false,
                "fontSize": 9
            }
```



## How this works

After parsing you press „Connect“ it will run st.exe with the new-tab action and the Remote Profile. From there it will run [sshchild.ps1](https://github.com/SirJson/SSHMan/blob/master/sshchild.ps1)

At the sametime the launcher will provide a named pipe with a guid that it passed to sshchild.ps1 and waits for a connection. If both are connected the script will receive the parameter it need to launch your ssh session.