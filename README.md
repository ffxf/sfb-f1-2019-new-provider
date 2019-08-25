# sfb-f1-2019-new-provider
SimfeedBack provider for F1 2019 (and F1 2018) new UDP format

Before you start, be sure you have F1-2019 or F1-2018 set to use the new UDP format (F1-2019 or F1-2018 respectively). 

By default the provider expects the F1 game to use port `20777` to publish its telemetry data. To change that number, create a file in the `provider` subdirectory of your SimFeedback installation that is called `f12019new_port.txt`. This file must contain only the port number (e.g. `40555`) and no other characters such as newlines. If the file is missing then thedefault of `20777` is used.

See [releases](https://github.com/ffxf/sfb-f1-2019-new-provider/releases) for installable files.

**Be sure you run `remove_blocking.bat` after installing the provider!**

## Status

The provider only has been tested with F1-2018 thus far although it should work with F1-2019 as well
