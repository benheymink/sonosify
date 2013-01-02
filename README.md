sonosify
========

A C# based app to control Sonos Devices, loosely based on [rahims SoCo python code](https://github.com/rahims/SoCo).

Soloution is Visual Studio 12, .net 4.5.

Still to do:
The SonosDevice class needs to be updated with all the various actions available (play, pause, skip, etc.)
The code for this should/could follow the SoCo example above - every action method simply creates the 'action'
SOAP xml and passes it down to a 'send_action' method.
