# chose an implementation, depending on os
#~ if sys.platform == 'cli':
#~ else:
if os.name == 'nt':  # sys.platform == 'win32':
    from serial.tools.list_ports_windows import comports
elif os.name == 'posix':
    from serial.tools.list_ports_posix import comports
#~ elif os.name == 'java':
else:
    raise ImportError("Sorry: no implementation for your platform ('{}') available".format(os.name))

class Port:
    def __init__(self, port, desc, hwid) -> None:
        self.port = port
        self.desc = desc
        self.hwid = hwid
    def __repr__(self) -> str:
        return f"{self.port:20}\n{self.desc} - HWID: {self.hwid}"