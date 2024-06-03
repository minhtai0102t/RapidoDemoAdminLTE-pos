
# dev notes

issue 

```
Failed to register URL for site application. Error description: The process cannot access the file because it is being used by another process. (0x80070020)

```


solve

```bash

netsh interface ipv4 show excludedportrange protocol=tcp
net stop winnat
net start winnat

```
